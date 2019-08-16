using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("LSystemsTests")]

namespace LSystems.Parsers
{
	/// <summary>
	/// Abstract class that serves as base for other expression parsers
	/// Only missing part is method GetNumericValue which is used to convert number to T
	/// Uses recursion for parsing, shouldn't be used on unreasonably large inputs as there is threat of
	/// stack overflow
	/// </summary>
	/// <typeparam name="T">Type of parameters used in delegates evaluation</typeparam>
	public abstract class ExpressionParser<T>
	{
		public delegate T Expression(T[] parameters);

		#region PARSER_OPERATORS
		public class ParserOperator
		{
			internal readonly char Representation;

			public ParserOperator(char representation)
			{
				if(char.IsLetterOrDigit(representation) || representation == '(' || representation == ')')
					throw new ArgumentException("representation cannot be letter, digit, or bracket");

				Representation = representation;
			}
		}
		
		public sealed class UnaryOperator : ParserOperator
		{
			internal readonly Func<Expression, Expression> Handler;

			public UnaryOperator(char representation, Func<Expression, Expression> handler) : base(representation)
			{
				Handler = handler ?? throw new ArgumentException("handler cannot be null");
			}
		}
		
		public sealed class BinaryOperator : ParserOperator
		{
			public static readonly int LOW_PRIORITY = 0;
			public static readonly int HIGH_PRIORITY = 1;
			
			internal readonly int Priority;
			internal readonly Func<Expression, Expression, Expression> Handler;

			public BinaryOperator(char representation, int priority, Func<Expression, Expression, Expression> handler)
				: base(representation)
			{
				Priority = priority;
				Handler = handler ?? throw new ArgumentException("handler cannot be null");
			}

			public BinaryOperator(char representation, Func<Expression, Expression, Expression> handler)
				: this(representation, LOW_PRIORITY, handler)
			{ }
		}	
		#endregion

		private readonly string[] _variableNames;
		private readonly Dictionary<char, UnaryOperator> _unaryOperators;
		private readonly Dictionary<char, BinaryOperator> _binaryOperators;

		//Maximum priority found on some BinaryOperator in _binaryOperators
		private readonly int _maxPriority;
		private readonly int _minPriority;

		public ExpressionParser(string[] variableNames, UnaryOperator[] unaryOperators, 
			BinaryOperator[] binaryOperators)
		{
			_variableNames = variableNames ?? new string[0];
			_unaryOperators = new Dictionary<char, UnaryOperator>();
			if (unaryOperators != null)
			{
				foreach (var op in unaryOperators)
				{
					if(!_unaryOperators.TryAdd(op.Representation, op)) 
						throw new ParserException("Duplicit definition of operator: " + op.Representation);
				}
			}

			_maxPriority = 0;
			_minPriority = 0;
			_binaryOperators = new Dictionary<char, BinaryOperator>();
			if (binaryOperators != null && binaryOperators.Length > 0)
			{
				_maxPriority = binaryOperators[0].Priority;
				_minPriority = binaryOperators[0].Priority;

				foreach (var op in binaryOperators)
				{
					if (op.Priority > _maxPriority) _maxPriority = op.Priority;
					if (op.Priority < _minPriority) _minPriority = op.Priority;
					if(!_binaryOperators.TryAdd(op.Representation, op))
						throw new ParserException("Duplicit definition of operator: " + op.Representation);
				}
			}
		}
		
		/// <summary>
		/// Creates Expression from string representing given expression
		/// </summary>
		/// <param name="expression">string representation of expression</param>
		/// <returns>Expression delegate</returns>
		public Expression ParseExpression(string expression)
		{
			//Remove whitespace characters from expression
			var builder = new StringBuilder();
			foreach (var c in expression)
			{
				if (!char.IsWhiteSpace(c))
					builder.Append(c);
			}
			expression = builder.ToString();
			var atoms = Atomize(expression);
			return ParseBinaryOperator(atoms,_minPriority, 0, atoms.Count);
		}

		/// <summary>
		/// Tries finding and parsing some binary operator in given range inside atoms list
		/// If no binary operator is found calls ParseUnaryOperator
		/// If binary operator is found, calls recursively itself to parse left and right side
		/// </summary>
		/// <param name="atoms">List of atoms (preferably obtained by Atomize method)</param>
		/// <param name="priority">Priority of operation which should be parsed</param>
		/// <param name="first">Index of first item in Atoms that should be parsed</param>
		/// <param name="afterLast">Index which is directly after last item that should be parsed</param>
		/// <returns>Expression delegate</returns>
		private Expression ParseBinaryOperator(in List<string> atoms, int priority, int first, int afterLast)
		{
			if (priority > _maxPriority)
				return ParseUnaryOperators(atoms, first, afterLast);
			
			int bracketDepth = 0;
			for (int i = first; i < afterLast; i++)
			{
				if (atoms[i] == "(")
					bracketDepth++;
				else if (atoms[i] == ")")
					bracketDepth--;
				else if (bracketDepth == 0 											//Atom is not inside any bracket
				         && i != first 												//Atom has something on it's left
				         && i != (afterLast-1) 										//Atom has something on it's right
				         && atoms[i].Length == 1 									//Atom is in desired form
				         && _binaryOperators.TryGetValue(atoms[i][0], out var op)	//Atom is defined operator
				         && op.Priority == priority)								//Operator has priority which is looked for
				{
					var leftSide = ParseBinaryOperator(atoms, priority+1, first, i);
					var rightSide = ParseBinaryOperator(atoms, priority, i + 1, afterLast);
					return op.Handler(leftSide, rightSide);
				}
			}

			return ParseBinaryOperator(atoms, priority + 1, first, afterLast);
		}

		private Expression ParseUnaryOperators(in List<string> atoms, int first, int afterLast)
		{
			if (atoms[first].Length == 1 && _unaryOperators.TryGetValue(atoms[first][0], out var op))
			{
				var rightSide = ParseUnaryOperators(atoms, first + 1, afterLast);
				return op.Handler(rightSide);
			}

			return ParseNonOperators(atoms, first, afterLast);
		}
		
		private Expression ParseNonOperators(in List<string> atoms, int first, int afterLast)
		{
			if (atoms[first] == "(" && atoms[afterLast - 1] == ")")
				return ParseBinaryOperator(atoms, _minPriority, first + 1, afterLast - 1);
			else if (first == afterLast - 1)
			{
				if (double.TryParse(atoms[first], out double number))
				{
					return GetNumericValue(atoms[first]);
				}
				else
				{
					return GetVariableValue(atoms[first]);
				}
			}
			else
			{
				throw new ParserException("Unable to parse atoms between indices " + first + 
				                          " and " + afterLast);
			}
		}

		private Expression GetVariableValue(string variableName)
		{
			int index;
			for (index = 0; index < _variableNames.Length; index++)
			{
				if (_variableNames[index] == variableName)
					break;
			}
			if(index == _variableNames.Length)
				throw new ParserException("Undefined variable: " + variableName);

			return args => args[index];
		}

		protected abstract Expression GetNumericValue(string numberRepresentation);

		#region ATOMIZE
		private enum CharType
		{
			Undefined,
			Number,
			Variable,
			ParserOperator
		}

		/// <summary>
		/// Returns CharType based on character
		/// </summary>
		/// <param name="c">character whose type will be returned</param>
		/// <returns>
		/// CharType.Number if c is digit,
		/// CharType.Variable if c is digit,
		/// CharType.ParserOperator if c is some defined ParserOperator
		/// CharType.Undefined otherwise
		/// </returns>
		private CharType GetCharType(char c)
		{
			if (char.IsDigit(c) || c == '.') 
				return CharType.Number;
			if (char.IsLetter(c)) 
				return CharType.Variable;
			if (_unaryOperators.ContainsKey(c) || _binaryOperators.ContainsKey(c) 
			                                   || c == '(' || c == ')') 
				return CharType.ParserOperator;

			return CharType.Undefined;
		}
		
		/// <summary>
		/// Convert expression to atoms
		/// Internal only for UnitTesting purposes
		/// </summary>
		/// <param name="expression">string with given math expression, without spaces</param>
		/// <returns>List of strings, each element of list representing one element in expression</returns>
		/// <exception cref="ParserException">Thrown if unknown symbol is found during atomizing</exception>
		internal List<string> Atomize(string expression)
		{
			var result = new List<string>();
			var builder = new StringBuilder();

			CharType prevCharType = CharType.Undefined;
			foreach (var c in expression)
			{
				CharType charType = GetCharType(c);
				if(charType == CharType.Undefined) throw new ParserException("Unknown symbol: " + c);

				if (charType != prevCharType && builder.Length > 0)
				{
					result.Add(builder.ToString());
					builder.Clear();
				}

				builder.Append(c);
				prevCharType = charType;
				
				//Hack: All math symbols are one character only -> we want to add them in next iteration
				if (charType == CharType.ParserOperator) prevCharType = CharType.Undefined;
			}
			
			if(builder.Length > 0) result.Add(builder.ToString());
			
			return result;
		}
		#endregion
	}
	
	/// <summary>
	/// Parser based on ExpressionParser which is specialized to int
	/// </summary>
	public class IntExpressionParser : ExpressionParser<int>
	{
		public IntExpressionParser(string[] variableNames, UnaryOperator[] unaryOperators, 
			BinaryOperator[] binaryOperators) 
			: base(variableNames, unaryOperators, binaryOperators) { }

		protected override Expression GetNumericValue(string numberRepresentation)
		{
			if (int.TryParse(numberRepresentation, out int value))
			{
				return args => value;
			}
			
			throw new ParserException("Following cannot be parsed into integer: " + numberRepresentation);
		}
	}

	/// <summary>
	/// Factory which creates IntExpressionParser with common operators predefined
	/// Predefined operators are: unary: -
	/// 						  binary: +, -, *, / 
	/// </summary>
	public class BasicIntExpressionParserFactory
	{
		public IntExpressionParser Create(string[] variableNames)
		{
			var unaryOperators = new IntExpressionParser.UnaryOperator[]
			{
				new ExpressionParser<int>.UnaryOperator('-',
					expression => { return args => -1 * expression(args);}), 
			};
			var binaryOperators = new IntExpressionParser.BinaryOperator[]
			{
				new ExpressionParser<int>.BinaryOperator(
					'+', 
					ExpressionParser<int>.BinaryOperator.LOW_PRIORITY,
					(left, right) => { return args => left(args) + right(args);}),
				new ExpressionParser<int>.BinaryOperator(
					'-',
					ExpressionParser<int>.BinaryOperator.LOW_PRIORITY,
					(left, right) => { return args => left(args) - right(args);}),
				new ExpressionParser<int>.BinaryOperator(
					'*',
					ExpressionParser<int>.BinaryOperator.HIGH_PRIORITY,
					(left, right) => { return args => left(args) * right(args);}),
				new ExpressionParser<int>.BinaryOperator(
					'/',
					ExpressionParser<int>.BinaryOperator.HIGH_PRIORITY,
					(left, right) => { return args => left(args) / right(args);}) 
			};
			return new IntExpressionParser(variableNames, unaryOperators, binaryOperators);
		}
	}
	
	/// <summary>
	/// Parser based on ExpressionParser which is specialized to double
	/// </summary>
	public class DoubleExpressionParser : ExpressionParser<double>
	{
		public DoubleExpressionParser(string[] variableNames, UnaryOperator[] unaryOperators, 
			BinaryOperator[] binaryOperators) 
			: base(variableNames, unaryOperators, binaryOperators) { }

		protected override Expression GetNumericValue(string numberRepresentation)
		{
			if (double.TryParse(numberRepresentation, out double value))
			{
				return args => value;
			}

			throw new ParserException("Following cannot be parsed into real: " + numberRepresentation);
		}
	}
	
	//TODO: Consider creating BasicDoubleExpressionParserFactory or using inheritance from BasicIntExpressionFactory

}