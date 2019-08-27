using System;
using System.Collections.Generic;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("LSystemsTests")]

namespace LSystems.Parsers
{
	public class ModuleParser<T>
	{
		private ExpressionParserFactory<T> _expressionParserFactory;
		private Dictionary<string, int> _knownModules;
		private int _nextModuleId;

		public ModuleParser(ExpressionParserFactory<T> expressionParserFactory)
		{
			_expressionParserFactory = expressionParserFactory ?? 
			                           throw new ParserException("expressionParser cannot be null");
			_knownModules = new Dictionary<string, int>();
			_nextModuleId = 0;
		}

		//TODO: Test
		/// <summary>
		/// Creates module factories for modules represented in given string
		/// </summary>
		/// <param name="representation">String with multiple modules names</param>
		/// <param name="variableNames">array containing names of variables</param>
		/// <returns>Array of module factories</returns>
		public ModuleFactory<T>[] ParseModuleFactories(string input, string[] variableNames)
		{
			if(input == null) throw new ArgumentException("input cannot be null");
			if (variableNames == null) variableNames = new string[0];
			
			var expressionParser = _expressionParserFactory.Create(variableNames);
			var modulesData = SeparateModulesRepresentations(input);
			
			var result = new ModuleFactory<T>[modulesData.Count];

			for (int moduleIndex = 0; moduleIndex < modulesData.Count; moduleIndex++) 
			{
				int moduleId = GetModuleId(modulesData[moduleIndex].Item1);
				
				string[] paramArray;
				if (modulesData[moduleIndex].Item2 == null)
					paramArray = new string[0];					
				else 
					paramArray = modulesData[moduleIndex].Item2.Split(',');
				
				var handlers = new Func<T[], T>[paramArray.Length];
				for (int paramIndex = 0; paramIndex < paramArray.Length; paramIndex++)
				{
					handlers[paramIndex] = 
						new Func<T[], T>(expressionParser.Parse(paramArray[paramIndex]));
				}
				
				result[moduleIndex] = new ModuleFactory<T>(moduleId, handlers);
			}

			return result;
		}

		//TODO: Test
		/// <summary>
		/// Creates modules from string representation. Modules can have only literal paramaters
		/// (no variables, or expressions)
		/// </summary>
		/// <param name="input">String with modules</param>
		/// <returns>Array of modules</returns>
		/// <exception cref="ArgumentException">Thrown if input string is null</exception>
		public Module<T>[] ParseModules(string input)
		{
			if(input == null) throw new ArgumentException("input cannot be null");
			
			var expressionParser = _expressionParserFactory.Create(new string[0]); //TODO: Hack, improve
			var modulesData = SeparateModulesRepresentations(input);
			
			var result = new Module<T>[modulesData.Count];

			for (int moduleIndex = 0; moduleIndex < modulesData.Count; moduleIndex++) 
			{
				int moduleId = GetModuleId(modulesData[moduleIndex].Item1);

				string[] paramArray;
				if (modulesData[moduleIndex].Item2 == null)
					paramArray = new string[0];
				else
					paramArray = modulesData[moduleIndex].Item2.Split(',');
				
				var arguments = new T[paramArray.Length];
				for (int paramIndex = 0; paramIndex < paramArray.Length; paramIndex++)
				{
					arguments[paramIndex] = expressionParser.ParseLiteral(paramArray[paramIndex]);
				}
				
				result[moduleIndex] = new Module<T>(moduleId, arguments);
			}

			return result;
		}

		/// <summary>
		/// Returns module ID if module has already one associated. Otherwise creates new id for module.
		/// </summary>
		/// <param name="moduleName">string with module name</param>
		/// <returns>integer uniquely identifying moduleName</returns>
		public int GetModuleId(string moduleName)
		{
			if (_knownModules.TryGetValue(moduleName, out int id))
			{
				return id;
			}
			else
			{
				_knownModules.Add(moduleName, _nextModuleId);
				var tmp = _nextModuleId;
				_nextModuleId++;
				return tmp;
			}
		}
		
		/// <summary>
		/// Reads representation string and divides it into modules and content in brackets that follows each name
		/// (or null if there is no bracket)
		/// </summary>
		/// <param name="representation">
		/// string containing only module names and possibly one bracket behind each name
		/// </param>
		/// <returns>List of doubles, first item contains module name, second content of it bracket</returns>
		/// <exception cref="ParserException">Thrown if representation doesn't contain valid modules represenation</exception>
		internal List<Tuple<string, string>> SeparateModulesRepresentations(string representation)
		{
			if(representation == null) return new List<Tuple<string, string>>();
			
			var modules = new List<Tuple<string, string>>();

			try
			{
				int index = 0;
				while (index < representation.Length)
				{
					string name = HelperMethods.ReadModuleName(representation, index, out index);
					index++;
					string details = null;
					if (index < representation.Length && representation[index] == '(')
					{
						details = HelperMethods.ReadBracketContent(representation, index, out index);
						index++;
					}
					
					modules.Add(new Tuple<string, string>(name, details));
				}

				return modules;
			}
			catch (ArgumentException)
			{
				throw new ParserException("representation cannot be parsed into modules");
			}
		}
		
		
	}
}