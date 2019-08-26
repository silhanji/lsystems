using System;
using System.Collections.Generic;
using System.Text;

namespace LSystems.Parsers
{
	#region IDENTIFIERS
	
	public delegate T Expression<T>(T[] parameters);
	
	public abstract class Identifier
	{
		public readonly string Representation;

		protected Identifier(string representation)
		{
			if(string.IsNullOrWhiteSpace(representation))
				throw new ParserException("representation cannot be null or empty");

			foreach (var c in representation)
			{
				if(c == ',' || c == '.' || c == '(' || c == ')')
					throw new ParserException("representation cannot contain comma, point or bracket");
			}

			Representation = representation;
		}
	}

	public class UnaryOperator<T> : Identifier
	{
		public readonly Func<Expression<T>, Expression<T>> Handler;
		
		public UnaryOperator(string representation, Func<Expression<T>, Expression<T>> handler) : base(representation)
		{
			Handler = handler ?? throw new ParserException("handler cannot be null");
		}
	}

	public class BinaryOperator<T> : Identifier
	{
		public readonly Func<Expression<T>, Expression<T>, Expression<T>> Handler;
		public readonly int Priority;

		public BinaryOperator(string representation, 
			int priority, Func<Expression<T>, Expression<T>, Expression<T>> handler) : base(representation)
		{
			Priority = priority;
			Handler = handler ?? throw new ParserException("handler cannot be null");
		}
	}

	public class Function<T> : Identifier
	{
		public readonly Func<Expression<T>[], Expression<T>> Handler;

		public Function(string representation, Func<Expression<T>[], Expression<T>> handler) : base(representation)
		{
			Handler = handler ?? throw new ParserException("handler cannot be null");
		}
	}

	public class Variable : Identifier
	{
		public readonly int Index;

		public Variable(string representation, int index) : base(representation)
		{
			if(index < 0) throw new ParserException("index must be grater or equal to 0");
			Index = index;
		}
	}

	#endregion

	#region EXPRESSION_PARSER
	public class ExpressionParser<T>
	{
		private readonly Dictionary<string, UnaryOperator<T>> _unaryOperators;
		private readonly Dictionary<string, BinaryOperator<T>> _binaryOperators;
		private readonly Dictionary<string, Function<T>> _functions;
		private readonly Dictionary<string, Variable> _variables;

		private readonly HashSet<string> _registredIdentifiers;
		
		public const int PRIORITY_LOW = 0;
		public const int PRIORITY_HIGH = 1;

		private readonly int _minPriority;
		private readonly int _maxPriority;

		public ExpressionParser(UnaryOperator<T>[] unaryOperators, BinaryOperator<T>[] binaryOperators, 
			Function<T>[] functions, Variable[] variables)
		{
			_registredIdentifiers = new HashSet<string>();
			
			_unaryOperators = new Dictionary<string, UnaryOperator<T>>();
			if(unaryOperators != null)
			{
				foreach (var op in unaryOperators)
				{
					if(op == null) throw new ParserException("unaryOperators cannot contain null values");
					_registredIdentifiers.Add(op.Representation);
					
					if(!_unaryOperators.TryAdd(op.Representation, op))
						throw new ParserException("Duplicit definition of operator: " + op.Representation);
				}
			}

			_minPriority = 0;
			_maxPriority = 0;
			
			_binaryOperators = new Dictionary<string, BinaryOperator<T>>();
			if(binaryOperators != null && binaryOperators.Length > 0)
			{
				_minPriority = binaryOperators[0].Priority;
				_maxPriority = binaryOperators[0].Priority;
				
				foreach (var op in binaryOperators)
				{
					if(op == null) throw new ParserException("binaryOperators cannot contain null values");
					
					_registredIdentifiers.Add(op.Representation);
					if (op.Priority < _minPriority)
						_minPriority = op.Priority;
					if (op.Priority > _maxPriority)
						_maxPriority = op.Priority;
					
					if(!_binaryOperators.TryAdd(op.Representation, op))
						throw new ParserException("Duplicit definition of operator: " + op.Representation);
					
				}
			}
			
			_functions = new Dictionary<string, Function<T>>();
			if(functions != null)
			{
				foreach (var op in functions)
				{
					if(op == null)
						throw new ParserException("functions cannot contain null elements");
					
					_registredIdentifiers.Add(op.Representation);
					
					if(!_functions.TryAdd(op.Representation, op))
						throw new ParserException("Duplicit definition of operator: " + op.Representation);
				}
			}

			_variables = new Dictionary<string, Variable>();
			if(variables != null)
			{
				foreach (var op in variables)
				{
					if(op == null)
						throw new ParserException("variables cannot contain null elements");
					
					_registredIdentifiers.Add(op.Representation);
					
					if(!_variables.TryAdd(op.Representation, op))
						throw new ParserException("Duplicit definition of operator: " + op.Representation);
				}
			}
		}

		public Expression<T> Parse(string input)
		{
			var tokenizer = new Tokenizer(_registredIdentifiers);
			var tokens = tokenizer.Tokenize(input);

			return ParseBinaryOperator(tokens, _minPriority, 0, tokens.Length);
		}

		private Expression<T> ParseBinaryOperator(Token[] tokens, int priority, int first, int count)
		{
			if (priority > _maxPriority)
				return ParseUnaryOperator(tokens, first, count);

			int bracketDepth = 0;
			for (int i = first; i < first + count; i++)
			{
				if (tokens[i].Type == Token.TokenType.Control)
				{
					if (tokens[i].Value == "(")
						bracketDepth++;
					else if (tokens[i].Value == ")")
						bracketDepth--;
				} else if (tokens[i].Type == Token.TokenType.Identifier 
				           && bracketDepth == 0
				           && i != first
				           && i != first + count - 1
				           && _binaryOperators.TryGetValue(tokens[i].Value, out var op)
				           && op.Priority == priority)
				{
					var leftSide = ParseBinaryOperator(tokens, priority + 1, first, (i - first));
					var rightSide = ParseBinaryOperator(tokens, priority, i + 1, count - i -1 + first);
					return op.Handler(leftSide, rightSide);
				}
			}

			return ParseBinaryOperator(tokens, priority + 1, first, count);
		}

		private Expression<T> ParseUnaryOperator(Token[] tokens, int first, int count)
		{
			if (tokens[first].Type == Token.TokenType.Identifier
			    && _unaryOperators.TryGetValue(tokens[first].Value, out var op))
			{
				var argument = ParseIdentifier(tokens, first + 1, count - 1);
				return op.Handler(argument);
			}

			return ParseIdentifier(tokens, first, count);
		}

		private Expression<T> ParseIdentifier(Token[] tokens, int first, int count)
		{
			if (tokens[first].Type == Token.TokenType.Control && tokens[first].Value == "(" &&
			    tokens[first + count - 1].Type == Token.TokenType.Control && tokens[first + count - 1].Value == ")")
			{
				return ParseBinaryOperator(tokens, _minPriority, first + 1, count - 2);
			} else if (tokens[first].Type == Token.TokenType.Identifier)
			{
				if (count == 1 && _variables.TryGetValue(tokens[first].Value, out var variable))
				{
					return args => args[variable.Index];
				} 
				else if (_functions.TryGetValue(tokens[first].Value, out var function) 
				         && tokens[first+1].Value == "("
				         && tokens[first+count-1].Value == ")")
				{
					var arguments = new List<Expression<T>>();
					int bracketDepth = 0;
					first += 2;
					int argStart = first;
					for(int i = first; i - first < count-3; i++)
					{
						if (tokens[i].Type == Token.TokenType.Control)
						{
							switch (tokens[i].Value)
							{
								case "(":
									bracketDepth++;
									break;
								case ")":
									bracketDepth--;
									break;
								case ",":
									if (bracketDepth == 0)
									{
										var argument = ParseBinaryOperator(tokens, _minPriority, argStart, i - argStart);
										argStart = i + 1;
										arguments.Add(argument);
									}

									break;
								default:
									throw new ParserException("Unknown token: " + tokens[i].Value);
							}
						}
					}

					var lastArgument = ParseBinaryOperator(tokens, _minPriority, argStart, count - 3 - argStart+first);
					arguments.Add(lastArgument);

					return function.Handler(arguments.ToArray());
				}
			} else if (tokens[first].Type == Token.TokenType.Literal && count == 1)
			{
				return ParseLiteral(tokens[first]);
			}
			
			throw new ParserException("Unable to parse " + count + " tokens starting at index " + first);
		}

		private Expression<T> ParseLiteral(Token token)
		{
			var literal = ParseLiteral(token.Value);
			return args => literal;
		}

		protected virtual T ParseLiteral(string representation)
		{
			throw new ParserException("Unknown literal: " + representation);
		}
	}

	public class IntExpressionParser : ExpressionParser<int>
	{
		public IntExpressionParser(UnaryOperator<int>[] unaryOperators, BinaryOperator<int>[] binaryOperators,
			Function<int>[] functions, Variable[] variables)
			: base(unaryOperators, binaryOperators, functions, variables)
		{
			
		}

		protected override int ParseLiteral(string representation)
		{
			if (int.TryParse(representation, out int value))
				return value;
			
			throw new ParserException(representation + " is not a number");
		}
	}

	public class DoubleExpressionParser : ExpressionParser<double>
	{
		public DoubleExpressionParser(UnaryOperator<double>[] unaryOperators, BinaryOperator<double>[] binaryOperators,
			Function<double>[] functions, Variable[] variables)
			: base(unaryOperators, binaryOperators, functions, variables)
		{
			
		}

		protected override double ParseLiteral(string representation)
		{
			if (double.TryParse(representation, out double value))
				return value;
			
			throw new ParserException(representation + " is not a number");
		}
	}
	
	public class IntExpressionParserFactory
	{
		//TODO: Consider making private and adding access methods
		public UnaryOperator<int>[] UnaryOperators;
		public BinaryOperator<int>[] BinaryOperators;
		public Function<int>[] Functions;

		public IntExpressionParserFactory()
		{
			UnaryOperators = new[]
			{
				new UnaryOperator<int>("-", 
					expression => { return (args) => -1 * expression(args);}), 
			};
			BinaryOperators = new[]
			{
				new BinaryOperator<int>("+", ExpressionParser<int>.PRIORITY_LOW,
					(left, right) => { return (args) => left(args) + right(args); }),
				new BinaryOperator<int>("-", ExpressionParser<int>.PRIORITY_LOW,
					(left, right) => { return (args) => left(args) - right(args); }),
				new BinaryOperator<int>("*", ExpressionParser<int>.PRIORITY_HIGH,
					(left, right) => { return (args) => left(args) * right(args); }),
				new BinaryOperator<int>("/", ExpressionParser<int>.PRIORITY_HIGH,
					(left, right) => { return (args) => left(args) / right(args); }),
			};
			Functions = new Function<int>[0];
		}

		public IntExpressionParser Create(Variable[] variables)
		{
			return new IntExpressionParser(UnaryOperators, BinaryOperators, Functions, variables);
		}

		public IntExpressionParser Create(string[] variableNames)
		{
			var variables = CreateVariables(variableNames);
			return Create(variables);
		}

		private Variable[] CreateVariables(string[] variableNames)
		{
			var variables = new Variable[variableNames.Length];
			for(int i = 0; i < variableNames.Length; i++)
			{
				variables[i] = new Variable(variableNames[i], i);
			}

			return variables;
		}
	}

	#endregion

	#region TOKENIZER
	internal class Tokenizer
	{
		private readonly HashSet<string> _identifiers;

		public Tokenizer(string[] identifiers)
		{
			if(identifiers == null)
				identifiers = new string[0];
			
			_identifiers = new HashSet<string>(identifiers);
		}

		public Tokenizer(HashSet<string> identifiers)
		{
			_identifiers = identifiers ?? new HashSet<string>();
		}

		public Token[] Tokenize(string input)
		{
			input = RemoveDoubleWhitespaces(input);
			var tokens = new List<Token>();
		
			var builder = new StringBuilder();
			var prevCharType = CharType.Unknown;
			foreach (var c in input)
			{
				var charType = GetCharType(c);
				if (charType == CharType.Digit && prevCharType == CharType.Letter) 
					charType = CharType.Letter; //Identifiers can contain numbers but must have letters before them

				if (charType == CharType.Control)
					prevCharType = CharType.Unknown; //Force to add prev char as controls are one letter only

				if (charType != prevCharType && builder.Length != 0)
				{
					tokens.Add(CreateToken(builder.ToString()));
					builder.Clear();
				}

				if (charType == CharType.Whitespace)
				{
					if (builder.Length > 0)
					{
						tokens.Add(CreateToken(builder.ToString()));
						builder.Clear();
					}

					continue;
				}
				
				builder.Append(c);
				prevCharType = charType;
			}
			if(builder.Length != 0)
				tokens.Add(CreateToken(builder.ToString()));

			return tokens.ToArray();
		}

		private string RemoveDoubleWhitespaces(string input)
		{
			var builder = new StringBuilder();
			bool wasWhitespace = false;
			foreach (char c in input)
			{
				if (char.IsWhiteSpace(c))
				{
					if (wasWhitespace)
						continue;
					
					wasWhitespace = true;
				}
				else
				{
					wasWhitespace = false;
				}

				builder.Append(c);
			}
			return builder.ToString().Trim(); //Trim to remove one leading and one trailing whitespace if present
		}

		private Token CreateToken(string value)
		{
			if(string.IsNullOrWhiteSpace(value)) 
				return new Token(Token.TokenType.Unknown, "");
			
			if(value.Length == 1 && GetCharType(value[0]) == CharType.Control)
				return new Token(Token.TokenType.Control, value);
			
			if(_identifiers.Contains(value))
				return new Token(Token.TokenType.Identifier, value);
			
			return new Token(Token.TokenType.Literal, value);
		}

		private CharType GetCharType(char c)
		{
			if (char.IsWhiteSpace(c)) return CharType.Whitespace;
			if (char.IsLetter(c) || c == '_') return CharType.Letter;
			if (char.IsDigit(c) || c == '.') return CharType.Digit;
			if (c == '(' || c == ')' || c == ',') return CharType.Control;

			return CharType.Other;
		}
		
		private enum CharType
		{
			Unknown,
			Whitespace,
			Letter,
			Digit,
			Control,
			Other,
		}
	}
	
	
	internal struct Token : IEquatable<Token> {

		public enum TokenType
		{
			Unknown,
			Control,		//( ) ,
			Identifier,
			Literal			//E.g. explicit number values
		}

		public readonly TokenType Type;
		public readonly string Value;

		public Token(TokenType type, string value)
		{
			Type = type;
			if (string.IsNullOrWhiteSpace(value))
				throw new ParserException("token value cannot be null or empty");
			Value = value;
		}

		public bool Equals(Token other)
		{
			return (this.Type == other.Type) && this.Value == other.Value;
		}
		
		public override bool Equals(object obj)
		{
			if (obj is Token)
			{
				return Equals((Token) obj);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
	#endregion
}