using System;
using System.Collections.Generic;
using System.Text;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("LSystemsTests")]

namespace LSystems.Utils.Parsers
{
	#region IDENTIFIERS

	/// <summary>
	/// Delegate used in parsing process
	/// </summary>
	/// <param name="parameters">Values of parameters</param>
	/// <typeparam name="T">Value of expression after evaluation</typeparam>
	public delegate T Expression<T>(T[] parameters);

	/// <summary>
	/// Base class for all types of identifiers, (variables, functions, ...)
	/// </summary>
	public abstract class Identifier
	{
		/// <summary>
		/// Characters used in parsed expression to represent this identifier.
		/// Cannot contain point, comma or brackets
		/// </summary>
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

	/// <summary>
	/// Represents function which has only one argument, written directly after identifier
	/// </summary>
	/// <typeparam name="T">Type used in Expressions</typeparam>
	public class UnaryOperator<T> : Identifier
	{
		/// <summary>
		/// Delegate returning Expression after it is provided with Expression which corresponds to single argument
		/// of UnaryOperator
		/// </summary>
		public readonly Func<Expression<T>, Expression<T>> Handler;
		
		public UnaryOperator(string representation, Func<Expression<T>, Expression<T>> handler) : base(representation)
		{
			Handler = handler ?? throw new ParserException("handler cannot be null");
		}
	}

	/// <summary>
	/// Represents function with exactly two arguments, where one is written directly in front of BinaryOperators
	/// representation and the other is written directly behind BinaryOperator representation
	/// </summary>
	/// <typeparam name="T">Type used in Expressions</typeparam>
	public class BinaryOperator<T> : Identifier
	{
		/// <summary>
		/// Delegate returning Expression after is is provided with Expressions which corresponds to arguments of
		/// BinaryOperator. Left argument corresponds to first Expression, right argument corresponds to second
		/// Expression
		/// </summary>
		public readonly Func<Expression<T>, Expression<T>, Expression<T>> Handler;
		/// <summary>
		/// Number indicating priority of operator, operators with higher priority are evaluated first.
		/// Note: BinaryOperator has always lower priority than any other Identifier
		/// </summary>
		public readonly int Priority;

		public BinaryOperator(string representation, 
			int priority, Func<Expression<T>, Expression<T>, Expression<T>> handler) : base(representation)
		{
			Priority = priority;
			Handler = handler ?? throw new ParserException("handler cannot be null");
		}
	}

	/// <summary>
	/// Represents function with arbitrary number of arguments. Arguments must be closed in bracket which starts
	/// directly after function representation and separated by commas.
	/// </summary>
	/// <typeparam name="T">Type used in Expressions</typeparam>
	public class Function<T> : Identifier
	{
		/// <summary>
		/// Delegate returning Expression after it is provided with array of Expressions representing function
		/// arguments. Element at nth index of array corresponds to n+1 argument of function (e.g. element at index 0
		/// corresponds to first argument).
		/// </summary>
		public readonly Func<Expression<T>[], Expression<T>> Handler;

		public Function(string representation, Func<Expression<T>[], Expression<T>> handler) : base(representation)
		{
			Handler = handler ?? throw new ParserException("handler cannot be null");
		}
	}

	/// <summary>
	/// Represents variable which can have assigned value during evaluation of Expression
	/// </summary>
	public class Variable : Identifier
	{
		/// <summary>
		/// Represents Index of this variable value when Expression is Evaluated
		/// </summary>
		public readonly int Index;

		public Variable(string representation, int index) : base(representation)
		{
			if(index < 0) throw new ParserException("index must be grater or equal to 0");
			Index = index;
		}
	}

	#endregion

	#region EXPRESSION_PARSER
	/// <summary>
	/// Base class for all ExpressionParsers
	/// Uses recursion to parse string into one expression which can be evaluated later.
	/// </summary>
	/// <typeparam name="T">Type used in Expressions, also type which corresponds to values of variables</typeparam>
	public abstract class ExpressionParser<T>
	{
		private readonly Dictionary<string, UnaryOperator<T>> _unaryOperators;
		private readonly Dictionary<string, BinaryOperator<T>> _binaryOperators;
		private readonly Dictionary<string, Function<T>> _functions;
		private readonly Dictionary<string, Variable> _variables;

		/// <summary>
		/// Collection of all representations used at least in one of _unaryOperators, _binaryOperators, _functions or
		/// _variables
		/// </summary>
		private readonly HashSet<string> _registredIdentifiers;
		
		/// <summary>
		/// Recommended value for priority variable in Low priority BinaryOperators
		/// </summary>
		public const int PRIORITY_LOW = 0;
		/// <summary>
		/// Recommended value for priority variable in High priority BinaryOperators
		/// </summary>
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

		/// <summary>
		/// Creates Expression from string input
		/// </summary>
		/// <param name="input">string containing representation of Expression</param>
		/// <returns>Expression which returns value of string when it is provided with values for variables</returns>
		public Expression<T> Parse(string input)
		{
			var tokenizer = new Tokenizer(_registredIdentifiers);
			var tokens = tokenizer.Tokenize(input);

			return ParseBinaryOperator(tokens,0, tokens.Length);
		}

		/// <summary>
		/// Method calling ParseBinaryOperator with _minPriority. Should be called instead the other it the call is not
		/// recursive..
		/// </summary>
		/// <param name="tokens">Array of tokens which forms entire expression</param>
		/// <param name="first">Index of first token searched for binary operator</param>
		/// <param name="afterLast">Index of token directly after last token meant to be searched</param>
		/// <returns>Expression which returns value of parsed BinaryOperator</returns>
		private Expression<T> ParseBinaryOperator(Token[] tokens, int first, int afterLast) =>
			ParseBinaryOperator(tokens, _minPriority, first, afterLast);
		
		/// <summary>
		/// Method parsing binary operator if some is present, or calling ParseUnaryOperator if none binary operator
		/// is found.
		/// </summary>
		/// <param name="tokens">Array of tokens which forms entire expression</param>
		/// <param name="priority">Priority of currently parsed binary operator, method won't parse binary operators
		/// with different priority</param>
		/// <param name="first">Index of first token to search for BinaryOperator</param>
		/// <param name="afterLast">Index of token directly after last token meant to be searched</param>
		/// <returns>Expression which returns value of parsed BinaryOperator</returns>
		/// <seealso cref="Token"/>
		/// <seealso cref="Tokenizer"/>
		private Expression<T> ParseBinaryOperator(Token[] tokens, int priority, int first, int afterLast)
		{
			if (priority > _maxPriority)
				return ParseUnaryOperator(tokens, first, afterLast); //No oher binary operator can exists

			int bracketDepth = 0;
			for (int i = first; i < afterLast; i++)
			{
				if (tokens[i].Type == Token.TokenType.Control)
				{
					if (tokens[i].Value == "(")
						bracketDepth++;
					else if (tokens[i].Value == ")")
						bracketDepth--;
				} 
				else if (tokens[i].Type == Token.TokenType.Identifier 
				           && bracketDepth == 0	//Is not nested
				           && i != first	//Has left argument
				           && i != afterLast - 1	//Has right argument
				           && _binaryOperators.TryGetValue(tokens[i].Value, out var op) //Corresponds to some BinaryOperator
				           && op.Priority == priority)	//Has requested priority
				{
					//Parse left argument, it is sure that if some BinaryOperator is on left side, it must have priority
					//higher than current operator
					var leftSide = ParseBinaryOperator(tokens, priority + 1, first, i);
					var rightSide = ParseBinaryOperator(tokens, priority, i + 1, afterLast);
					return op.Handler(leftSide, rightSide);
				}
			}

			//Parse binary operators with higher priority
			return ParseBinaryOperator(tokens, priority + 1, first, afterLast);
		}

		/// <summary>
		/// Method parsing unary operator if some is found or calling ParseIdentifier if no unary operator is found
		/// </summary>
		/// <param name="tokens">Array of tokens which corresponds to entire expression</param>
		/// <param name="first">Index of tokens which should contain UnaryOperator</param>
		/// <param name="afterLast">Index of token directly after last token meant to be searched</param>
		/// <returns>Expression which returns value of parsed UnaryOperator</returns>
		/// <seealso cref="Token"/>
		/// <seealso cref="Tokenizer"/>
		private Expression<T> ParseUnaryOperator(Token[] tokens, int first, int afterLast)
		{
			if (tokens[first].Type == Token.TokenType.Identifier
			    && _unaryOperators.TryGetValue(tokens[first].Value, out var op))
			{
				var rightSide = ParseBracket(tokens, first + 1, afterLast);
				return op.Handler(rightSide);
			}

			return ParseBracket(tokens, first, afterLast);
		}

		/// <summary>
		/// Method for parsing bracket and its content
		/// </summary>
		/// <param name="tokens">Array of Tokens which corresponds to entire expression</param>
		/// <param name="first">Index of first Token to be evaluated</param>
		/// <param name="afterLast">Index of token directly after last token meant to be searched</param>
		/// <returns>Expression which returns value of parsed bracket</returns>
		/// <exception cref="ParserException">Thrown if Tokens can be parsed into expression (e.g. unknown identifiers,
		/// missing arguments, ...)</exception>
		/// <seealso cref="Token"/>
		/// <seealso cref="Tokenizer"/>
		private Expression<T> ParseBracket(Token[] tokens, int first, int afterLast)
		{
			if (tokens[first].Type == Token.TokenType.Control && tokens[first].Value == "(" &&
			    tokens[afterLast - 1].Type == Token.TokenType.Control && tokens[afterLast - 1].Value == ")")
			{
				//Bracket can contain anything, start parsing from top again
				return ParseBinaryOperator(tokens,first + 1, afterLast - 1); 
			} 
			
			if (tokens[first].Type == Token.TokenType.Identifier)
				return ParseIdentifier(tokens, first, afterLast);
			
			if (tokens[first].Type == Token.TokenType.Literal && (afterLast - first) == 1)
				return ParseLiteral(tokens[first]);
			
			//Token is not identifier or literal
			throw new ParserException("Unable to parse token at " + first + " unknown token type");
		}

		/// <summary>
		/// Method for parsing identifiers
		/// </summary>
		/// <param name="tokens">Array of Tokens which corresponds to entire expression</param>
		/// <param name="first">Index of first Token to be evaluated</param>
		/// <param name="afterLast">Index of token directly after last token meant to be searched</param>
		/// <returns>Expression which returns value of parsed Identifier</returns>
		/// <exception cref="ParserException">Thrown if tokens[first] is not an identifier</exception>
		private Expression<T> ParseIdentifier(Token[] tokens, int first, int afterLast)
		{
			if(tokens[first].Type != Token.TokenType.Identifier)
				throw new ParserException("Unable to parse token at " + first + ". Token is not identifier");

			if ((afterLast - first) == 1
			    && _variables.TryGetValue(tokens[first].Value, out var variable))
				return args => args[variable.Index];

			//If not variable than identifier must identify function
			return ParseFunction(tokens, first, afterLast);
		}

		/// <summary>
		/// Method for parsing functions
		/// </summary>
		/// <param name="tokens">Array of Tokens which corresponds to entire expression</param>
		/// <param name="first">Index of first Token to be evaluated</param>
		/// <param name="afterLast">Index of token directly after last token meant to be searched</param>
		/// <returns>Expression which returns value of parsed Function</returns>
		/// <exception cref="ParserException">Thrown if tokens[first] is not a function, or if brackets delimiting
		/// arguments are missing</exception>
		private Expression<T> ParseFunction(Token[] tokens, int first, int afterLast)
		{
			if(!_functions.TryGetValue(tokens[first].Value, out var function) ||
			   tokens[first+1].Value != "(" ||
			   tokens[afterLast-1].Value != ")")
				throw new DrawerException("Cannot parse token at " + first + 
				                          ". Token doesn't correspond to any function");
			
			var arguments = new List<Expression<T>>();
			int bracketDepth = 0;
			int argStart = first+2; //Determines start index of current argument
			for(int i = first+2; i < afterLast-1; i++)
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
								var argument = ParseBinaryOperator(tokens,argStart, i);
								argStart = i + 1;
								arguments.Add(argument);
							}
							break;
						default:
							throw new ParserException("Unknown Control token: " + tokens[i].Value);
					}
				}
			}

			var lastArgument = ParseBinaryOperator(tokens,argStart, afterLast-1);
			arguments.Add(lastArgument);

			return function.Handler(arguments.ToArray());
		}
		
		/// <summary>
		/// Parses literal from token
		/// </summary>
		/// <param name="token">Token whose value will be returned in Expression</param>
		/// <returns>Expression which returns value of given token</returns>
		private Expression<T> ParseLiteral(Token token)
		{
			T literal = ParseLiteral(token.Value);
			return args => literal;
		}

		/// <summary>
		/// Abstract class which determines how to handle strings which does not correspond to any identifiers
		/// For example if T is some numeric value, it's parsing from string should be done in this method
		/// </summary>
		/// <param name="representation">string with unknown value</param>
		/// <returns>Value of string</returns>
		public abstract T ParseLiteral(string representation);
	}

	/// <summary>
	/// ExpressionParser which is specialized on int.
	/// Serves for parsing any expression containing Integers
	/// </summary>
	public class IntExpressionParser : ExpressionParser<int>
	{
		public IntExpressionParser(UnaryOperator<int>[] unaryOperators, BinaryOperator<int>[] binaryOperators,
			Function<int>[] functions, Variable[] variables)
			: base(unaryOperators, binaryOperators, functions, variables)
		{
			
		}

		/// <summary>
		/// Parses string into int
		/// </summary>
		/// <param name="representation">string representation of integer</param>
		/// <returns>Int value of representation</returns>
		/// <exception cref="ParserException">Thrown if representation cannot be parsed into integer</exception>
		public override int ParseLiteral(string representation)
		{
			if (int.TryParse(representation, out int value))
				return value;
			
			throw new ParserException(representation + " is not a number");
		}
	}

	/// <summary>
	/// ExpressionParser which is specialized on double
	/// Serves for parsing any expression containing real values (represented as Double)
	/// </summary>
	public class DoubleExpressionParser : ExpressionParser<double>
	{
		public DoubleExpressionParser(UnaryOperator<double>[] unaryOperators, BinaryOperator<double>[] binaryOperators,
			Function<double>[] functions, Variable[] variables)
			: base(unaryOperators, binaryOperators, functions, variables)
		{
			
		}

		/// <summary>
		/// Parses string into double
		/// </summary>
		/// <param name="representation">string representation of real value</param>
		/// <returns>double value of representation</returns>
		/// <exception cref="ParserException">Thrown if representation cannot be parsed into double</exception>
		public override double ParseLiteral(string representation)
		{
			if (double.TryParse(representation, out double value))
				return value;
			
			throw new ParserException(representation + " is not a number");
		}
	}

	/// <summary>
	/// ExpressionParser specialized on bools
	/// Serves for parsing any expression containing bool values
	/// </summary>
	public class BoolExpressionParser : ExpressionParser<bool>
	{
		public BoolExpressionParser(UnaryOperator<bool>[] unaryOperators, BinaryOperator<bool>[] binaryOperators,
			Function<bool>[] functions, Variable[] variables)
			: base(unaryOperators, binaryOperators, functions, variables)
		{
			
		}

		/// <summary>
		/// Parses string into bool
		/// </summary>
		/// <param name="representation">string containing "true" or "false"</param>
		/// <returns>true if representation contained "true", false if representation contained "false"</returns>
		/// <exception cref="ParserException">Thrown if representation doesn't contain true or false</exception>
		public override bool ParseLiteral(string representation)
		{
			var normalized = representation.ToLower();
			if (normalized == "true")
				return true;
			if (normalized == "false")
				return false;

			throw new ParserException(representation + " is not a boolean value");
		}
	}
	
	#endregion

	#region EXPRESSION_PARSER_FACTORIES

	/// <summary>
	/// Base class for Factories creating ExpressionParsers.
	/// Use of these factories is mainly to simplify creation of ExpressionParsers by predefining common operators and
	/// functions 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ExpressionParserFactory<T>
	{
		public abstract UnaryOperator<T>[] UnaryOperators { get; set; }
		public abstract BinaryOperator<T>[] BinaryOperators { get; set; }
		public abstract Function<T>[] Functions { get; set; }

		/// <summary>
		/// Implementations should create ExpressionParsers which contain exactly UnaryOperators, BinaryOperators and
		/// Functions defined as properties in factory, plus Variables provided as argument
		/// </summary>
		/// <param name="variables">Variables used in ExpressionParser creation</param>
		/// <returns>ExpressionParser with given properties</returns>
		public abstract ExpressionParser<T> Create(Variable[] variables);

		/// <summary>
		/// Creates ExpressionParser by creating Variables from string array and than calling Create(Variable[])
		/// </summary>
		/// <param name="variableNames">string array where each element represents one variable</param>
		/// <returns>ExpressionParser with given properties</returns>
		public ExpressionParser<T> Create(string[] variableNames)
		{
			var variables = CreateVariables(variableNames);
			return Create(variables);
		}

		/// <summary>
		/// Creates Variable array out of string array. Assumes that Variable whose name is stored on nth position
		/// corresponds to nth value in parsed Expression
		/// </summary>
		/// <param name="variableNames">array containing variable names which will be turned into variables</param>
		/// <returns>Array of variables whose names correspond to array provided in argument</returns>
		private static Variable[] CreateVariables(string[] variableNames)
		{
			var variables = new Variable[variableNames.Length];
			for(int i = 0; i < variableNames.Length; i++)
			{
				variables[i] = new Variable(variableNames[i], i);
			}

			return variables;
		}
	}
	
	/// <summary>
	/// ExpressionParserFactory specialized on int
	/// Predefines unary operator - and binary operators +, -, *, /
	/// </summary>
	public class IntExpressionParserFactory : ExpressionParserFactory<int>
	{
		public override UnaryOperator<int>[] UnaryOperators { get; set; }
		public override BinaryOperator<int>[] BinaryOperators { get; set; }
		public override Function<int>[] Functions { get; set; }
		
		public IntExpressionParserFactory() => Init();

		private void Init()
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
		
		public override ExpressionParser<int> Create(Variable[] variables)
		{
			return new IntExpressionParser(UnaryOperators, BinaryOperators, Functions, variables);
		}
	}

	/// <summary>
	/// ExpressionParserFactory specialized on double
	/// Predefines unary operator - and binary operators +, -, *, /
	/// </summary>
	public class DoubleExpressionParserFactory : ExpressionParserFactory<double>
	{
		public override UnaryOperator<double>[] UnaryOperators { get; set; }
		public override BinaryOperator<double>[] BinaryOperators { get; set; }
		public override Function<double>[] Functions { get; set; }
		
		public DoubleExpressionParserFactory() => Init();
		
		private void Init()
		{
			UnaryOperators = new[]
			{
				new UnaryOperator<double>("-", 
					expression => { return (args) => -1 * expression(args);}), 
			};
			BinaryOperators = new[]
			{
				new BinaryOperator<double>("+", ExpressionParser<int>.PRIORITY_LOW,
					(left, right) => { return (args) => left(args) + right(args); }),
				new BinaryOperator<double>("-", ExpressionParser<int>.PRIORITY_LOW,
					(left, right) => { return (args) => left(args) - right(args); }),
				new BinaryOperator<double>("*", ExpressionParser<int>.PRIORITY_HIGH,
					(left, right) => { return (args) => left(args) * right(args); }),
				new BinaryOperator<double>("/", ExpressionParser<int>.PRIORITY_HIGH,
					(left, right) => { return (args) => left(args) / right(args); }),
			};
			Functions = new Function<double>[0];
		}
		
		public override ExpressionParser<double> Create(Variable[] variables)
		{
			return new DoubleExpressionParser(UnaryOperators, BinaryOperators, Functions, variables);
		}
	}
	
	/// <summary>
	/// ExpressionParserFactory specialized on bool
	/// Predefines unary operator ! (invert bool value) and binary operators || (or) and && (and)
	/// </summary>
	public class BoolExpressionParserFactory : ExpressionParserFactory<bool>
	{
		public override UnaryOperator<bool>[] UnaryOperators { get; set; }
		public override BinaryOperator<bool>[] BinaryOperators { get; set; }
		public override Function<bool>[] Functions { get; set; }
		
		public BoolExpressionParserFactory() => Init();

		private void Init()
		{
			UnaryOperators = new[]
			{
				new UnaryOperator<bool>("!", 
					expression => { return (args) => !expression(args);}), 
			};
			BinaryOperators = new[]
			{
				new BinaryOperator<bool>("||", ExpressionParser<bool>.PRIORITY_LOW,
					(left, right) => { return (args) => left(args) || right(args); }),
				new BinaryOperator<bool>("&&", ExpressionParser<bool>.PRIORITY_LOW,
					(left, right) => { return (args) => left(args) && right(args); }),
			};
			Functions = new Function<bool>[0];
		}
		
		public override ExpressionParser<bool> Create(Variable[] variables)
		{
			return new BoolExpressionParser(UnaryOperators, BinaryOperators, Functions, variables);
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