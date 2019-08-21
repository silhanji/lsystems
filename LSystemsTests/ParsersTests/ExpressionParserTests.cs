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
		
		public void ExpressionParser_Atomize_ValidInputTest(
			string input, string[] variableNames, string[] expectedOutput)
		{
			//Can't test method from ExpressionParser directly as ExpressionParser is abstract class
			//Testing BasicIntExpressionParser as it uses method from ExpressionParser
			ExpressionParser<int> expressionParser = new BasicIntExpressionParserFactory().Create(variableNames);

			var output = expressionParser.Atomize(input);
			
			Assert.That(output, Is.EqualTo(expectedOutput));
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
		public void BasicIntExpressionParser_Parse_ValidInputTest(
			string input, string[] variableNames, int[] parameters, int expectedValue)
		{
			var expressionParser = new BasicIntExpressionParserFactory().Create(variableNames);
			var handler = expressionParser.ParseExpression(input);

			int value = handler(parameters);
			
			Assert.That(value, Is.EqualTo(expectedValue));
		}

		[TestCase(
			"x+y",
			new []{"x"})]
		[TestCase(
			"x+1.0",
			new []{"x"})]
		public void BasicIntExpressionParser_Parse_ExceptionTest(string input, string[] variableNames)
		{
			var expressionParser = new BasicIntExpressionParserFactory().Create(variableNames);

			Assert.Throws<ParserException>(() => expressionParser.ParseExpression(input));
		}
	}
}