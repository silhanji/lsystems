using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using LSystems;
using LSystems.Core;
using LSystems.Utils;
using LSystems.Utils.Parsers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LSystemsWebApp.Pages.Editor
{
	public class Result : PageModel
	{
		public string ErrorMsg = null;
		public string Svg = null;
		public int GenerationsCount = 5;
		public int RandomizationPercent = 0;

		private Dictionary<string, ModuleInfo> _modules;
		private List<AxiomInfo> _axiom;
		private List<RuleInfo> _rules;
		
		public void OnGet()
		{
			GetSessionInfo();
			GenerateSVG();
		}

		public void OnPost()
		{
			GetSessionInfo();
			
			var sCount = Request.Form["generation-count"];
			if (string.IsNullOrWhiteSpace(sCount) || !int.TryParse(sCount, out var count))
			{
				ErrorMsg = "Please type in number of generations";
				return;
			}
			else
			{
				if (count < 0)
				{
					ErrorMsg = "Generations number must be positive";
					return;
				}
				GenerationsCount = count;
			}

			var sRandom = Request.Form["randomization-percent"];
			if (string.IsNullOrWhiteSpace(sRandom) || !int.TryParse(sRandom, out var random))
			{
				ErrorMsg = "Please type in percent of randomization";
				return;
			}
			else
			{
				if (random < 0 || random > 100)
				{
					ErrorMsg = "Randomization percent must be between 0 and 100";
					return;
				}
				RandomizationPercent = random;
			}
			GenerateSVG();
		}

		private void GenerateSVG()
		{
			if (_modules.Count == 0)
			{
				ErrorMsg = "You have not defined any modules, please define them first!";
				return;
			}
			
			if (_axiom.Count == 0)
			{
				ErrorMsg = "Your axiom is empty, please fill it first!";
				return;
			}
			
			
			var expressionParserFactory = new DoubleExpressionParserFactory();
			var sqrt = new Function<double>("sqrt",
				expressions => { return (parameters => Math.Sqrt(expressions[0](parameters)));});
			expressionParserFactory.Functions = new[] {sqrt};
			var generatorParser = new GeneratorParser<double>(expressionParserFactory);

			var builder = new StringBuilder();
			builder.AppendLine(CreateAxiomInput());
			foreach (var rule in _rules)
				builder.AppendLine(CreateRuleInput(rule));

			var input = builder.ToString();

			Generator<double> generator;
			var reader = new StringReader(input);
			try
			{
				generator = generatorParser.Parse(reader);
			}
			catch (ParserException)
			{
				ErrorMsg = "Error while processing your input. Please check if all rules are correct";
				return;
			}

			generator.AdvanceNGenerations(GenerationsCount);
			var output = generator.CurrentGeneration;

			if (output.Count > 1000000)
			{
				ErrorMsg = "Resulting image is too complex, please try reducing number of generations";
				return;
			}
			
			DrawingAction[] elements;
			try
			{
				elements = CreateDrawingActions(generatorParser.ModuleParser);
			}
			catch (ArgumentException)
			{
				ErrorMsg = "You have mistake in modules definition! Please revise them"; 
				return;
			}
			
			var creator = new VectorDrawer(elements);

			try
			{
				if (RandomizationPercent != 0)
				{
					double randomFactor = ((double) RandomizationPercent) / 100;
					Svg = creator.Draw(output, randomFactor);
				}
				else
				{
					Svg = creator.Draw(output);
				}
			}
			catch (Exception)
			{
				ErrorMsg = "Internal error while drawing your image";
				return;
			}
		}

		/// <summary>
		/// Translates moduleName into string which is possible to read by RuleParser
		/// </summary>
		/// <param name="originalName">string containing any character</param>
		/// <returns>string containing only letters, digits and underscores</returns>
		private static string TranslateModuleName(string originalName)
		{
			var builder = new StringBuilder();
			builder.Append('M');
			foreach (var c in originalName)
			{
				if (char.IsLetterOrDigit(c))
				{

					if (char.IsUpper(c))
						builder.Append(char.ToLower(c));
					else
						builder.Append(c);
				}
				else
				{
					int charVal = (int) c;
					builder.Append('_');
					builder.Append(charVal);
					builder.Append('_');
				}
			}

			return builder.ToString();
		}
		
		private string CreateAxiomInput()
		{
			var builder = new StringBuilder();
			foreach (var module in _axiom)
			{
				string name = TranslateModuleName(module.Name);
				builder.Append(name);

				if (module.Params.Count > 0)
				{
					builder.Append('(');
					for (int i = 0; i < module.Params.Count - 1; i++)
					{
						builder.Append(module.Params[i].Value);
						builder.Append(", ");
					}

					builder.Append(module.Params[module.Params.Count - 1].Value);
					builder.Append(')');
				}
			}

			return builder.ToString();
		}

		private string CreateRuleInput(RuleInfo rule)
		{
			var builder = new StringBuilder();
			if (rule.LeftContext != null && rule.LeftContext.Count > 0)
			{
				builder.Append(CreateContextInput(rule.LeftContext));
				builder.Append("<");
			}

			builder.Append(TranslateModuleName(rule.SourceModule));
			if (!_modules.TryGetValue(rule.SourceModule, out var moduleInfo))
			{
				throw new InvalidOperationException("ModuleInfo is not present in dictionary");
			}

			if (moduleInfo.ParamNames.Count > 0)
			{
				builder.Append('(');
				for (int i = 0; i < moduleInfo.ParamNames.Count - 1; i++)
				{
					builder.Append(moduleInfo.ParamNames[i]);
					builder.Append(", ");
				}

				builder.Append(moduleInfo.ParamNames[moduleInfo.ParamNames.Count - 1]);
				builder.Append(')');
			}

			if (rule.RightContext != null && rule.RightContext.Count > 0)
			{
				builder.Append(">");
				builder.Append(CreateContextInput(rule.RightContext));
			}

			if (rule.ParametricConditions != null && rule.ParametricConditions.Count > 0)
			{
				builder.Append(": ");
				for (int i = 0; i < rule.ParametricConditions.Count - 1; i++)
				{
					builder.Append(rule.ParametricConditions[i]);
					builder.Append(", ");
				}

				builder.Append(rule.ParametricConditions[rule.ParametricConditions.Count - 1]);
			}

			builder.Append("->");

			foreach (var module in rule.NextGeneration)
			{
				builder.Append(TranslateModuleName(module.Name));
				if (module.Params != null && module.Params.Count > 0)
				{
					builder.Append("(");
					for (int i = 0; i < module.Params.Count - 1; i++)
					{
						builder.Append(module.Params[i].Value);
						builder.Append(", ");
					}

					builder.Append(module.Params[module.Params.Count - 1].Value);
					builder.Append(") ");
				}
			}

			return builder.ToString();
		}

		private string CreateContextInput(List<string> context)
		{
			if (context == null || context.Count == 0)
				return string.Empty;
			
			var builder = new StringBuilder();
			foreach (var module in context)
			{
				builder.Append(TranslateModuleName(module));
				builder.Append(" ");
			}

			return builder.ToString();
		}

		private DrawingAction[] CreateDrawingActions(ModuleParser<double> parser)
		{
			var result = new List<DrawingAction>();
			foreach (var module in _modules)
			{
				var info = module.Value;
				var name = TranslateModuleName(info.ModuleName);

				var action = info.GetAction();
				
				result.Add(new DrawingAction(parser.GetModuleId(name), action));
			}

			return result.ToArray();
		}

		private void GetSessionInfo()
		{
			var sessionContents = new SessionContents(HttpContext.Session);

			var modules = sessionContents.GetSessionContents<ModuleInfo>(SessionConstants.MODULE_INFOS_SESSION_KEY);
			_modules = new Dictionary<string, ModuleInfo>();
			foreach(var module in modules)
				_modules.Add(module.ModuleName, module);
			
			_axiom = sessionContents.GetSessionContents<AxiomInfo>(SessionConstants.AXIOM_INFOS_SESSION_KEY);
			_rules = sessionContents.GetSessionContents<RuleInfo>(SessionConstants.RULE_INFOS_SESSION_KEY);
		}
	}
}