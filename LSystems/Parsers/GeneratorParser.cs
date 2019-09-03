using System;
using System.Collections.Generic;
using System.IO;

namespace LSystems.Parsers
{
	/// <summary>
	/// Parser which expects axiom on first line and each rule on other lines
	/// </summary>
	public class GeneratorParser<T> where T : IComparable<T>
	{
		public ModuleParser<T> ModuleParser { get; }
		private RuleParser<T> _ruleParser;

		public GeneratorParser(ExpressionParserFactory<T> expressionParserFactory)
		{
			if(expressionParserFactory == null) throw new ArgumentException("expressionParser cannot be null");
			
			ModuleParser = new ModuleParser<T>(expressionParserFactory);
			_ruleParser = new RuleParser<T>(ModuleParser, expressionParserFactory);
		}

		public Generator<T> Parse(StringReader reader)
		{
			if(reader == null) throw new ArgumentException("reader cannot be null");
			
			var firstLine = reader.ReadLine();
			//Axiom
			var axiomModules = ModuleParser.ParseModules(firstLine);
			var axiom = new Generation<T>(new List<Module<T>>(axiomModules), 0);
			//Rules
			var rules = new List<Rule<T>>();
			string line = reader.ReadLine();
			while (line != null)
			{
				rules.Add(_ruleParser.Parse(line));
				line = reader.ReadLine();
			}
			
			return new Generator<T>(axiom, rules.ToArray());
		}
	}
}