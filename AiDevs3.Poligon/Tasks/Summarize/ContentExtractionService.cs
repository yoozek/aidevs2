using System.Text.RegularExpressions;
using OpenAI.Chat;

namespace AiDevs3.Poligon.Tasks.Summarize
{
    public class ExtractionType
    {
        public string Key { get; set; }
        public string Description { get; set; }
    }

    public class ExtractionResult
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
    }

    public class ContentProcessor(ChatClient chatClient, string outputDirectory)
    {
        /// <summary>
        /// Main entry point for processing content. Orchestrates the entire extraction and summary generation process.
        /// </summary>
        public async Task ProcessContent(string articlePath, string title)
        {
            var article = await File.ReadAllTextAsync(articlePath);
            var extractedData = await ExtractAllInformation(title, article);

            await SaveExtractionResults(extractedData);

            var draft = await GenerateDraftSummary(title, article, extractedData);
            var critique = await GenerateCritique(draft, article, extractedData["context"]);
            var finalSummary = await GenerateFinalSummary(draft, extractedData, critique);

            await SaveFinalResults(draft, critique, finalSummary);
        }

        private List<ExtractionType> GetExtractionTypes()
        {
            return
            [
                new ExtractionType
                {
                    Key = "topics",
                    Description =
                        "Main subjects covered in the article. Focus here on the headers and all specific topics discussed in the article."
                },
                new ExtractionType
                {
                    Key = "entities",
                    Description =
                        "Mentioned people, places, or things mentioned in the article. Skip the links and images."
                },
                new ExtractionType
                {
                    Key = "keywords",
                    Description =
                        "Key terms and phrases from the content. You can think of them as hastags that increase searchability of the content for the reader."
                },
                new ExtractionType
                {
                    Key = "links",
                    Description = "Complete list of the links and images mentioned with their 1-sentence description."
                },
                new ExtractionType { Key = "resources", Description = "Tools, platforms, resources mentioned in the article." },
                new ExtractionType { Key = "takeaways", Description = "Main points and valuable lessons learned." },
                new ExtractionType { Key = "context", Description = "Background information and setting." }
            ];
        }

        private async Task<Dictionary<string, string>> ExtractAllInformation(
            string title,
            string article)
        {
            var extractionTypes = GetExtractionTypes();
            var extractionTasks = extractionTypes.Select(async type =>
            {
                var content = await ExtractInformation(title, article, type.Key, type.Description);
                return new ExtractionResult
                {
                    Type = type.Key,
                    Description = type.Description,
                    Content = content ?? $"No {type.Key} found"
                };
            });

            var results = await Task.WhenAll(extractionTasks);
            return results.ToDictionary(r => r.Type, r => r.Content);
        }

        private async Task<string> ExtractInformation(string title, string text, string extractionType, string description)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(
                    $@"Extract {extractionType}: {description} from user message under the context of ""{title}"". 
                    Transform the content into clear, structured yet simple bullet points without formatting except links and images. 
                    Format link like so: - name: brief description with images and links if the original message contains them.
                    Keep full accuracy of the original message."
                ),
                new UserChatMessage(
                    $"Here's the articles you need to extract information from: {text}"
                )
            };

            ChatCompletion completion = await chatClient.CompleteChatAsync(messages);

            return completion.Content[0].Text;
        }

        private async Task<string> GenerateDraftSummary(string title, string article, Dictionary<string, string> extractedData)
        {
            ChatCompletion completion = await chatClient.CompleteChatAsync(CreateDraftPrompt(title, article, extractedData));

            return completion.Content[0].Text;
        }

        private async Task<string> GenerateCritique(string summary, string article, string context)
        {
            ChatCompletion completion = await chatClient.CompleteChatAsync(CreateCritiquePrompt(summary, article, context));

            return completion.Content[0].Text;
        }

        private async Task<string> GenerateFinalSummary(string refinedDraft, Dictionary<string, string> extractedData, string critique)
        {
            ChatCompletion completion = await chatClient.CompleteChatAsync(CreateFinalSummaryPrompt(refinedDraft, extractedData, critique));

            return completion.Content[0].Text;
        }

        private string? ExtractTagContent(string content, string tagName)
        {
            var regex = new Regex($@"<{tagName}>(.*?)</{tagName}>", RegexOptions.Singleline);
            var match = regex.Match(content);
            return match.Success ? match.Groups[1].Value.Trim() : null;
        }

        private async Task SaveExtractionResults(Dictionary<string, string> extractedData)
        {
            var saveTask = extractedData.Select(async (kvp, index) =>
            {
                var filePath = Path.Combine(outputDirectory, $"{index + 1}_{kvp.Key}.md");
                await File.WriteAllTextAsync(filePath, kvp.Value);
            });

            await Task.WhenAll(saveTask);
        }

        private async Task SaveFinalResults(string draft, string critique, string finalSummary)
        {
            var draftContent = ExtractTagContent(draft, "final_answer") ?? string.Empty;
            var finalSummaryContent = ExtractTagContent(finalSummary, "final_answer") ?? string.Empty;

            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "8_draft_summary.md"), draftContent);
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "9_summary_critique.md"), critique);
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "10_final_summary.md"), finalSummaryContent);
        }

        private string CreateDraftPrompt(string title, string article, Dictionary<string, string> extractedData)
        {
            return $@"As a copywriter, create a standalone, fully detailed article based on ""{title}"" that can be understood without reading the original. Write in markdown format, incorporating all images within the content...
                     <context>{extractedData["context"]}</context>
                     <entities>{extractedData["entities"]}</entities>
                     <links>{extractedData["links"]}</links>
                     <topics>{extractedData["topics"]}</topics>
                     <key_insights>{extractedData["takeaways"]}</key_insights>
                     <original_article>{article}</original_article>";
        }

        private string CreateCritiquePrompt(string summary, string article, string context)
        {
            return $@"Analyze the provided compressed version of the article critically...
                     <original_article>{article}</original_article>
                     <context desc=""It may help you to understand the article better."">{context}</context>
                     <compressed_version>{summary}</compressed_version>";
        }

        private string CreateFinalSummaryPrompt(string refinedDraft, Dictionary<string, string> extractedData, string critique)
        {
            return $@"Create a final compressed version of the article...
                     <refined_draft>{refinedDraft}</refined_draft>
                     <topics>{extractedData["topics"]}</topics>
                     <key_insights>{extractedData["takeaways"]}</key_insights>
                     <critique>{critique}</critique>
                     <context>{extractedData["context"]}</context>";
        }
    }
}