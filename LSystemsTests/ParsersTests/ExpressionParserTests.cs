using System;
using LSystems.Parsers;
using NUnit.Framework;

namespace LSystemsTests.ParsersTests
{
	[TestFixture]
	public class ExpressionParserTests
	{
		[TestCase(
			"22", 
			new string[0], 
			new []{"22"})]
		[TestCase(
			"2+3", 
			new string[0], 
			new []{"2", "+", "3"})]
		[TestCase(
			"2*41-8",
			new string[0],
			new []{"2", "*", "41", "-", "8"})]
		[TestCase(
			"-15+(x/8)-y",
			new []{"x", "y"},
			new []{"-", "15","+","(","x","/","8",")", "-", "y"})]
		[TestCase(
			"(a((433)34)2b)",
			new []{"a", "b", "c"},
			new []{"(","a","(","(","433",")","34",")","2","b",")"})]
		
		public void Tokenizer_Tokenize_ValidInputTest(
			string input, string[] variableNames, string[] expectedOutput)
		{
			//Can't test method from ExpressionParser directly as ExpressionParser is abstract class
			//Testing BasicIntExpressionParser as it uses method from ExpressionParser
			Tokenizer tokenizer = new Tokenizer(variableNames);

			var output = tokenizer.Tokenize(input);
			
			//Comparing only string values
			var sOutput = new string[output.Length];
			for(int i = 0; i < output.Length; i++)
			{
				sOutput[i] = output[i].Value;
			}
			
			Assert.That(sOutput, Is.EqualTo(expectedOutput));
		}

		[TestCase(
			"1",
			new string[]{},
			new int[]{},
			1)]
		[TestCase(
			"1+2",
			new string[]{},
			new int[]{},
			3)]
		[TestCase(
			"3+9*2",
			new string[]{},
			new int[]{},
			21)]
		[TestCase(
			"3*(2+7)",
			new string[]{},
			new int[]{},
			27)]
		[TestCase(
			"-(2+8)-1",
			new string[]{},
			new int[]{},
			-11)]
		[TestCase(
			"x",
			new string[]{"x"},
			new int[]{20},
			20)]
		[TestCase(
			"x-y",
			new string[]{"x", "y"},
			new int[]{4, 3},
			1)]
		[TestCase(
			"(-y+y*y-4*x*z)/(2*x)",
			new string[]{"x", "y", "z"},
			new int[]{3, 4, 5},
			-8)]
		[TestCase(
			"4*((1+2)+3)",
			new string[]{},
			new int[]{},
			24)]
		public void IntExpressionParser_Parse_ValidInputTest(
			string input, string[] variableNames, int[] parameters, int expectedValue)
		{
			var expressionParser = new IntExpressionParserFactory().Create(variableNames);
			var handler = expressionParser.Parse(input);

			int value = handler(parameters);
			
			Assert.That(value, Is.EqualTo(expectedValue));
		}

		[TestCase(
			"mult(3,sum(2,7))",
			new string[]{},
			new int[]{},
			27)]
		[TestCase(
			"diff(x,  y)",
			new string[]{"x", "y"},
			new int[]{4, 3},
			1)]
		[TestCase(
			"div(6, 2+1)",
			new string[]{"x", "y"},
			new int[]{4, 3},
			2)]
		public void IntExpressionParser_Parse_ValidInputFunctionsTest(
			string input, string[] variableNames, int[] parameters, int expectedValue)
		{
			var factory = new IntExpressionParserFactory();
			//Add some non standard functions which will be tested
			var functions = new[]
			{
				new Function<int>("sum", 
					(param) => { return (args) => param[0](args) + param[1](args); }), 
				new Function<int>("diff", 
					(param) => { return (args) => param[0](args) - param[1](args); }), 
				new Function<int>("mult", 
					(param) => { return (args) => param[0](args) * param[1](args); }), 
				new Function<int>("div", 
					(param) => { return (args) => param[0](args) / param[1](args); }), 
			};
			factory.Functions = functions;
			
			var expressionParser = factory.Create(variableNames);
			var handler = expressionParser.Parse(input);

			int value = handler(parameters);
			
			Assert.That(value, Is.EqualTo(expectedValue));
		}
		
		[TestCase(
			"x+y",
			new []{"x"})]
		[TestCase(
			"x+1.0",
			new []{"x"})]
		public void IntExpressionParser_Parse_ExceptionTest(string input, string[] variableNames)
		{
			var expressionParser = new IntExpressionParserFactory().Create(variableNames);

			Assert.Throws<ParserException>(() => expressionParser.Parse(input));
		}
	}
}