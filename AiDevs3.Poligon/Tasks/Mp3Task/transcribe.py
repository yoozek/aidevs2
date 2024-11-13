import os
from pathlib import Path
import openai
import glob
from pydub import AudioSegment
import argparse
import sys
from openai import OpenAI
from datetime import datetime

def convert_m4a_to_mp3(m4a_path):
    """Convert M4A file to MP3 since OpenAI requires MP3 format"""
    audio = AudioSegment.from_file(m4a_path, format="m4a")
    mp3_path = m4a_path.replace(".m4a", ".mp3")
    audio.export(mp3_path, format="mp3")
    return mp3_path

def transcribe_audio(client, file_path, model="whisper-1"):
    """Transcribe an audio file using OpenAI's API"""
    try:
        with open(file_path, "rb") as audio_file:
            transcript = client.audio.transcriptions.create(
                model=model,
                file=audio_file
            )
            return transcript.text
    except Exception as e:
        return f"Error transcribing {file_path}: {str(e)}"

def create_separator(filename):
    """Create a formatted separator for the transcription"""
    return f"""
{'='*80}
SOURCE FILE: {filename}
TRANSCRIPTION TIME: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}
{'='*80}

"""

def parse_arguments():
    """Parse command line arguments"""
    parser = argparse.ArgumentParser(
        description='Transcribe M4A audio files using OpenAI Whisper API',
        formatter_class=argparse.ArgumentDefaultsHelpFormatter
    )
    
    parser.add_argument(
        '-i', '--input',
        required=True,
        help='Input directory containing M4A files or path to single M4A file'
    )
    
    parser.add_argument(
        '-o', '--output',
        required=True,
        help='Output file path for combined transcriptions (e.g., transcriptions.txt)'
    )
    
    parser.add_argument(
        '-k', '--api-key',
        help='OpenAI API key. If not provided, will look for OPENAI_API_KEY environment variable'
    )
    
    parser.add_argument(
        '-m', '--model',
        default='whisper-1',
        help='OpenAI Whisper model to use'
    )
    
    parser.add_argument(
        '--keep-mp3',
        action='store_true',
        help='Keep temporary MP3 files after transcription'
    )
    
    parser.add_argument(
        '--append',
        action='store_true',
        help='Append to existing output file instead of overwriting'
    )

    return parser.parse_args()

def main():
    # Parse command line arguments
    args = parse_arguments()
    
    # Initialize OpenAI client
    api_key = args.api_key or os.getenv("OPENAI_API_KEY")
    if not api_key:
        print("Error: OpenAI API key not provided. Either use --api-key or set OPENAI_API_KEY environment variable")
        sys.exit(1)
    
    client = OpenAI(api_key=api_key)

    # Create output directory if it doesn't exist
    output_dir = os.path.dirname(args.output)
    if output_dir:
        Path(output_dir).mkdir(parents=True, exist_ok=True)

    # Get list of M4A files to process
    if os.path.isfile(args.input):
        if not args.input.lower().endswith('.m4a'):
            print("Error: Input file must be an M4A file")
            sys.exit(1)
        m4a_files = [args.input]
    else:
        m4a_files = glob.glob(os.path.join(args.input, "*.m4a"))
        if not m4a_files:
            print(f"No M4A files found in {args.input}")
            sys.exit(1)

    # Sort files for consistent ordering
    m4a_files.sort()

    # Open output file in append mode if specified, otherwise write mode
    mode = 'a' if args.append else 'w'
    with open(args.output, mode, encoding="utf-8") as out_file:
        # If we're starting a new file, write a header
        if not args.append:
            out_file.write(f"COMBINED TRANSCRIPTIONS\nStarted at: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n")
            out_file.write(f"Input source: {args.input}\n\n")

        # Process each file
        total_files = len(m4a_files)
        for index, m4a_file in enumerate(m4a_files, 1):
            print(f"Processing [{index}/{total_files}]: {m4a_file}")
            
            # Convert M4A to MP3
            mp3_file = convert_m4a_to_mp3(m4a_file)
            
            # Get transcription
            transcript = transcribe_audio(client, mp3_file, args.model)
            
            # Write to combined file with separator
            out_file.write(create_separator(os.path.basename(m4a_file)))
            out_file.write(transcript)
            out_file.write("\n\n")
            
            # Clean up temporary MP3 file if not keeping
            if not args.keep_mp3:
                os.remove(mp3_file)
            
            print(f"Transcription completed for: {m4a_file}")

        # Write footer
        out_file.write(f"\nTranscription completed at: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n")
        
    print(f"\nAll transcriptions have been saved to: {args.output}")

if __name__ == "__main__":
    main()