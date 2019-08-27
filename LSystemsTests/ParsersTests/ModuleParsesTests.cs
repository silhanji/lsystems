using System;
using System.Collections.Generic;
using LSystems.Parsers;
using NUnit.Framework;


namespace LSystemsTests.ParsersTests
{
	[TestFixture]
	public class ModuleParsesTests
	{
		[TestCase(
			"ModuleModule(details)",
			new string[]
			{
				"Module", null, "Module", "details"
			})]
		[TestCase(
			"ModuleModuleLong_module",
			new string[]
			{
				"Module", null, "Module", null, "Long_module", null
			})]
		[TestCase(
			"A(x)B(y)C(z)",
			new string[]
			{
				"A", "x", "B", "y", "C", "z"
			})]
		[TestCase(
			"AAAAAA",
			new string[]
			{
				"A", null, "A", null, "A", null, "A", null, "A", null, "A", null
				
			})]
		public void RuleParser_SeparateModuleRepresentations_ValidInputTest(string input, 
			string[] expectedOutputContent)
		{
			//Prepare output to format used in method return value
			var expectedOutput = new List<Tuple<string, string>>();
			for (int i = 0; i < expectedOutputContent.Length; i += 2)
				expectedOutput.Add(new Tuple<string, string>(expectedOutputContent[i], expectedOutputContent[i+1]));
			if(expectedOutputContent.Length % 2 == 1)
				expectedOutput.Add(new Tuple<string, string>(expectedOutputContent[expectedOutputContent.Length-1], null));
			
			
			var parser = new ModuleParser<int>(new IntExpressionParserFactory());

			var output = parser.SeparateModulesRepresentations(input);

			
			Assert.That(output, Is.EqualTo(expectedOutput));
		}
		
		[TestCase("Module1(Details)(AnotherDetails)")]
		[TestCase("Module(Details)not_a_module")]
		public void RuleParser_SeparateModuleRepresentations_ExceptionTest(string input)
		{
			var parser = new ModuleParser<int>(new IntExpressionParserFactory());

			Assert.Throws<ParserException>(() => parser.SeparateModulesRepresentations(input));
		}
	}
}