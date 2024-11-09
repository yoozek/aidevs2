import json
import argparse
import ast
import operator
from openai import OpenAI
import sys

class MathEvaluator:
    def __init__(self):
        # Define allowed operators
        self.operators = {
            ast.Add: operator.add,
            ast.Sub: operator.sub,
            ast.Mult: operator.mul,
            ast.Div: operator.truediv,
        }

    def eval_expr(self, expr_str):
        """Safely evaluate a mathematical expression string."""
        try:
            # Parse the expression
            node = ast.parse(expr_str, mode='eval').body
            return self.eval_node(node)
        except Exception as e:
            raise ValueError(f"Invalid expression: {expr_str}")

    def eval_node(self, node):
        """Recursively evaluate an AST node."""
        if isinstance(node, ast.Num):
            return node.n
        elif isinstance(node, ast.BinOp):
            op_type = type(node.op)
            if op_type not in self.operators:
                raise ValueError(f"Unsupported operator: {op_type}")
            left = self.eval_node(node.left)
            right = self.eval_node(node.right)
            return self.operators[op_type](left, right)
        else:
            raise ValueError(f"Unsupported node type: {type(node)}")

def process_json_file(input_path, output_path):
    try:
        # Read input JSON file
        with open(input_path, 'r') as file:
            data = json.load(file)

        # Initialize OpenAI client
        client = OpenAI(api_key='xxxxx')
        math_evaluator = MathEvaluator()

        # Process test-data array
        for item in data['test-data']:
            if 'question' in item and 'answer' in item:
                # Calculate correct answer
                try:
                    correct_answer = math_evaluator.eval_expr(item['question'])
                    item['answer'] = correct_answer
                except ValueError as e:
                    print(f"Warning: Could not evaluate expression '{item['question']}': {e}")

                # Process test field if present
                if 'test' in item and isinstance(item['test'], dict):
                    try:
                        # Get answer from GPT-4
                        response = client.chat.completions.create(
                            model="gpt-4",
                            messages=[
                                {"role": "user", "content": item['test']['q']}
                            ]
                        )
                        item['test']['a'] = response.choices[0].message.content.strip()
                    except Exception as e:
                        print(f"Warning: Error getting GPT response: {e}")
                        item['test']['a'] = "Error: Could not get answer from GPT"

        # Write output JSON file
        with open(output_path, 'w') as file:
            json.dump(data, file, indent=4)

        print(f"Processing complete. Output written to {output_path}")

    except json.JSONDecodeError as e:
        print(f"Error: Invalid JSON in input file: {e}")
        sys.exit(1)
    except FileNotFoundError as e:
        print(f"Error: File not found: {e}")
        sys.exit(1)
    except Exception as e:
        print(f"Error: An unexpected error occurred: {e}")
        sys.exit(1)

def main():
    # Set up argument parser
    parser = argparse.ArgumentParser(description='Process JSON file with Q&A data')
    parser.add_argument('input_path', help='Path to input JSON file')
    parser.add_argument('output_path', help='Path to output JSON file')

    # Parse arguments
    args = parser.parse_args()

    # Process the file
    process_json_file(args.input_path, args.output_path)

if __name__ == "__main__":
    main()