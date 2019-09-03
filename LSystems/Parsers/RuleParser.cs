using System;
using System.Collections.Generic;
using System.Text;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("LSystemsTests")]

namespace LSystems.Parsers
{
	/// <summary>
	/// Parser designed to create Rule objects from text representation of LSystems
	/// Supposed representation is: LeftContext < SourceModule > RightContext : ParametricConditions -> NextGenModules
	/// Where only SourceModule and NextGenModules parts are obligatory 
	/// </summary>
	public class RuleParser<T> where T : IComparable<T>
	{
		private readonly ModuleParser<T> _moduleParser;
		private readonly ExpressionParserFactory<T> _expressionParserFactory;
		
		public RuleParser(ModuleParser<T> moduleParser, ExpressionParserFactory<T> expressionParserFactory)
		{
			_moduleParser = moduleParser ?? throw new ParserException("moduleParser cannot be null");
			_expressionParserFactory = expressionParserFactory ?? 
			                    throw new ArgumentException("expressionParserFactory cannot be null");
		}

		public Rule<T> Parse(string representation)
		{
			//Remove white spaces
			var builder = new StringBuilder();
			foreach (var c in representation)
			{
				if (!char.IsWhiteSpace(c))
					builder.Append(c);
			}
			representation = builder.ToString();

			var sections = Subdivide(representation);
			
			//Parse source name and params
			ParseSourceModule(sections[1], out int id, out var paramNames);
			
			//Parse context conditions
			Rule<T>.ContextCondition[] contextConditions;
			if (sections[0] != null || sections[2] != null)
				contextConditions = new[]
				{
					ParseContextCondition(sections[1], sections[0],
						sections[2])
				};
			else
				contextConditions = new Rule<T>.ContextCondition[0];
			
			//Parse param conditions
			Rule<T>.ParamCondition[] paramConditions;
			if (sections[3] != null)
				paramConditions = ParseParamConditions(sections[3], paramNames);
			else
				paramConditions = new Rule<T>.ParamCondition[0];
			
			//Parse next generation modules
			var nextGenFactories = _moduleParser.ParseModuleFactories(sections[4], paramNames);

			
			return new Rule<T>(id, nextGenFactories, paramConditions, contextConditions);
		}

		/// <summary>
		/// Parser source module ID and param names based on its representation
		/// </summary>
		/// <param name="representation">string containing one module name and up to one bracket with param names</param>
		/// <param name="sourceId">id of module in representation</param>
		/// <param name="paramNames">names of param in bracket if present</param>
		/// <exception cref="ParserException">Thrown if unknown symbols are found</exception>
		internal void ParseSourceModule(string representation, out int sourceId, out string[] paramNames)
		{
			string moduleName = HelperMethods.ReadModuleName(representation, 0, out int endIndex);
			sourceId = _moduleParser.GetModuleId(moduleName);
			endIndex++;

			if (endIndex < representation.Length && representation[endIndex] == '(')
			{
				string param = HelperMethods.ReadBracketContent(representation, endIndex, out endIndex);
				if (endIndex != representation.Length - 1)
					throw new ParserException("Unknown content at source module at index: " + (endIndex + 1));

				paramNames = param.Split(',');
			}
			else
				paramNames = new string[0];
		}
		
		/// <summary>
		/// Divides string with representation of rule into substrings that correspond to parts of rule.
		/// Returns array with exactly five elements:
		/// {leftContext, sourceModule, rightContext, paramConditions, nextGeneration}
		/// If some parts are not present in representation, null pointer is stored on it's place
		/// </summary>
		/// <param name="representation">String representing Rule</param>
		/// <returns>string array with 5 elements</returns>
		internal static string[] Subdivide(string representation)
		{
			var result = new string[5];
			var builder = new StringBuilder();

			bool paramConditionsPresent = false; //Used to distinguish between rightContext and param conditions
			
			for(int i = 0; i < representation.Length; i++)
			{
				char c = representation[i];
				switch (c)
				{
					case '<':
						if (paramConditionsPresent) break; //Does not determine context, but is used in condition
						result[0] = builder.ToString();
						builder.Clear();
						continue;
					case '>':
						if (paramConditionsPresent) break; //Does not determine context, but is used in condition
						result[1] = builder.ToString();
						builder.Clear();
						continue;
					case ':':
						if (result[1] == null) result[1] = builder.ToString();
						else result[2] = builder.ToString();
						builder.Clear();
						paramConditionsPresent = true;
						continue;
					case '-':
						if (representation[i + 1] != '>')
							break; //Not the arrow symbol we are looking for

						if (result[1] == null) result[1] = builder.ToString();
						else if (!paramConditionsPresent) result[2] = builder.ToString();
						else result[3] = builder.ToString();
						builder.Clear();
						i++; //Skip '>' part of arrow which is already processed
						continue;
				}

				builder.Append(c);
			}

			result[4] = builder.ToString();
			
			//Replace empty strings with nulls
			for(int i = 0; i < result.Length; i++)
				if (result[i] != null && result[i].Length == 0)
					result[i] = null;
			
			if(result[1] == null) throw new ParserException("representation doesn't contain source module");
			if(result[4] == null) throw new ParserException("representation doesn't contain nextgen modules");

			return result;
		}

		/// <summary>
		/// Parses parametric conditions from string representation
		/// </summary>
		/// <param name="representation">string with parametric conditions, each condition is separated by comma</param>
		/// <param name="variableNames">array with all variable names defined with source module</param>
		/// <returns>List off all param conditions contained in representation string</returns>
		/// <exception cref="ParserException">Thrown if some condition is empty or cannot be parsed</exception>
		internal Rule<T>.ParamCondition[] ParseParamConditions(string representation, string[] variableNames)
		{
			var individualRepresentations = representation.BracketAwareSplit(',');
			var result = new Rule<T>.ParamCondition[individualRepresentations.Count];
			var parser = _expressionParserFactory.Create(variableNames);
			
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = ParseParamCondition(individualRepresentations[i],parser);
			}

			return result;
		}

		/// <summary>
		/// Creates one ParamCondition out of string representation of this condition
		/// </summary>
		/// <param name="representation">string containing one comparison operator (<, <=, >, >=, =, !=)</param>
		/// <param name="parser">parser used for left and right side of condition</param>
		/// <returns>Delegate which returns true if condition was met, false otherwise</returns>
		/// <exception cref="ParserException">Thrown if unknown comparison operator is found</exception>
		internal Rule<T>.ParamCondition ParseParamCondition(string representation, ExpressionParser<T> parser)
		{
			var supportedOperators = new[] {"<=", ">=", "!=", "<", ">", "="}; //All of these operators should be later
																				//used in delegate!

			string foundOp = null;
			List<string> splited = null;
			foreach (var op in supportedOperators)
			{
				splited = representation.BracketAwareSplit(op);
				if (splited.Count == 2)
				{
					//If Count == 1 than representation doesn't contained operator,
                    //if Count > 2 than representation contained operator more than once which is not supported
                    foundOp = op;
                    break;
					
				} 
			}
			if(splited == null || splited.Count != 2)
				throw new ParserException("Couldn't find comparison operator in " + representation);

			var left = parser.Parse(splited[0]);
			var right = parser.Parse(splited[1]);

			return (args) =>
			{
				var leftV = left(args);
				var rightV = right(args);

				int comparisonValue = leftV.CompareTo(rightV);

				switch (foundOp)
				{
					case "<":
						if (comparisonValue < 0) return true;
						return false;
					case "<=":
						if (comparisonValue <= 0) return true;
						return false;
					case ">":
						if (comparisonValue > 0) return true;
						return false;
					case ">=":
						if (comparisonValue >= 0) return true;
						return false;
					case "=":
						if (comparisonValue == 0) return true;
						return false;
					case "!=":
						if (comparisonValue != 0) return true;
						return false;
					default:
						throw new ParserException("Unknown operator");
				}
			};
		}

		/// <summary>
		/// Creates context condition from text representation
		/// </summary>
		/// <param name="sourceModuleSection">string representing module between left and right context</param>
		/// <param name="leftContextSection">string with names of modules which should be on left side of source</param>
		/// <param name="rightContextSection">string with names of modules which should be on right side of source</param>
		/// <returns>Delegate which returns true if given GenerationIndex meets condition described by provided
		/// representation</returns>
		/// <exception cref="ParserException">Thrown if some module in left or right context contains bracket</exception>
		internal Rule<T>.ContextCondition ParseContextCondition(string sourceModuleSection, string leftContextSection, 
			string rightContextSection)
		{
			int sourceId = _moduleParser.GetModuleId(
				HelperMethods.ReadModuleName(sourceModuleSection, 0, out var tmp));

			var leftContext = _moduleParser.ParseModules(leftContextSection);
			var leftContextIds = new List<int>();
			foreach (var module in leftContext)
			{
				var details = module.Parameters;
				if(details != null && details.Length > 0) 
					throw new ParserException("Parameters are not allowed in context conditions");
				
				leftContextIds.Add(module.Id);
			}
			
			var rightContext = _moduleParser.ParseModules(rightContextSection);
			var rightContextIds = new List<int>();
			foreach (var module in rightContext)
			{
				var details = module.Parameters;
				if(details != null && details.Length > 0) 
					throw new ParserException("Parameters are not allowed in context conditions");

				rightContextIds.Add(module.Id);
			}

			return generationIndex =>
			{
				if (generationIndex.Module.Id != sourceId) return false;
				int start = generationIndex.Index;
				//Not enough modules to the left of source module
				if (leftContextIds.Count > start) return false;
				//Not enough modules to the right of source module
				if (rightContextIds.Count > generationIndex.Count - start - 1) return false;

				for (int i = 0; i < leftContextIds.Count; i++)
				{
					if (generationIndex[start - i - 1].Id != leftContextIds[leftContext.Length - i - 1]) return false;
				}

				for (int i = 0; i < rightContextIds.Count; i++)
				{
					if (generationIndex[start + i + 1].Id != rightContextIds[i]) return false;
				}

				return true;
			};
		}

		
		
		
	}
}