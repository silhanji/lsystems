using System;
using System.Collections.Generic;
using System.Linq;
using LSystems;
using LSystems.Parsers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;

namespace LSystemsTests.ParsersTests
{
	[TestFixture]
	public class RuleParserTests
	{
		[TestCase(
			"X->X",
			new []{null, "X", null, null, "X"})]
		[TestCase("X(y):y>0->Y", new[]{null, "X(y)", null, "y>0", "Y"})]
		[TestCase("A<X(y)>B:y<1->C", new []{"A", "X(y)", "B", "y<1", "C"})]
		public void RuleParser_Subdivide_ValidInputTest(string input, string[] expectedOutput)
		{
			var output = RuleParser<int>.Subdivide(input);
			
			Assert.That(output, Is.EqualTo(expectedOutput));
		}

		//TODO: Think of same way to test this
		public void RuleParser_ParseContextCondition_ValidTest(string sourceModule, string leftContext,
			string rightContext, string[] generation, int index, bool expectedOutput)
		{
			throw new NotImplementedException();
		}

		[TestCase(
			"Source",
			"Left(param)",
			"Right")]
		[TestCase(
			"Source",
			"Left",
			"Right(param)")]
		public void RuleParser_ParseContextCondition_ExceptionTest(string sourceModule, string leftContext,
			string rightContext)
		{
			var expressionParserFactory = new IntExpressionParserFactory();
			var parser = new RuleParser<int>(new ModuleParser<int>(expressionParserFactory), expressionParserFactory);

			Assert.Throws<ParserException>(
				() => parser.ParseContextCondition(sourceModule, leftContext, rightContext));
		}

		[TestCase("A", new string[0])]
		[TestCase("A(x,y,z)", new []{"x", "y", "z"})]
		[TestCase("Module(param1,param2)", new [] {"param1", "param2"})]
		public void RuleParser_ParseSourceModule_ValidInputTest(string representation, string[] expectedParamNames)
		{
			var expressionParserFactory = new IntExpressionParserFactory();
			var parser = new RuleParser<int>(new ModuleParser<int>(expressionParserFactory), expressionParserFactory);
			
			parser.ParseSourceModule(representation, out int id, out var paramNames);
			
			Assert.That(paramNames, Is.EqualTo(expectedParamNames));
		}

		[TestCase("A", "B")]
		[TestCase("A", "Aa")]
		public void RuleParser_ParseSourceModule_UniqueIDTest(string representation1, string representation2)
		{
			var expressionParserFactory = new IntExpressionParserFactory();
			var parser = new RuleParser<int>(new ModuleParser<int>(expressionParserFactory), expressionParserFactory);

			parser.ParseSourceModule(representation1, out int id1, out var param1);
			parser.ParseSourceModule(representation2, out int id2, out var param2);
			
			Assert.That(id1, Is.Not.EqualTo(id2));
		}

		[TestCase("Module(param)Module")]
		[TestCase("Module(param)(param)")]
		public void RuleParser_ParseSourceModule_ExceptionTest(string representation)
		{
			var expressionParserFactory = new IntExpressionParserFactory();
			var parser = new RuleParser<int>(new ModuleParser<int>(expressionParserFactory), expressionParserFactory);

			Assert.Throws<ParserException>(
				() => parser.ParseSourceModule(representation, out var tmp1, out var tmp2));
		}

		[TestCase(
			"1<2",
			new string[0],
			new int[0],
			true)]
		[TestCase(
			"1>2",
			new string[0],
			new int[0],
			false)]
		[TestCase(
			"1=1",
			new string[0],
			new int[0],
			true)]
		[TestCase(
			"1<=1",
			new string[0],
			new int[0],
			true)]
		[TestCase(
			"1>=2",
			new string[0],
			new int[0],
			false)]
		[TestCase(
			"1!=2",
			new string[0],
			new int[0],
			true)]
		[TestCase(
			"1!=1",
			new string[0],
			new int[0],
			false)]
		[TestCase(
			"1<=2",
			new string[0],
			new int[0],
			true)]
		[TestCase(
			"1<=-1",
			new string[0],
			new int[0],
			false)]
		[TestCase(
			"x<2",
			new []{"x"},
			new []{1},
			true)]
		[TestCase(
			"x<2",
			new []{"x"},
			new []{10},
			false)]
		[TestCase(
			"x+y<25-y",
			new []{"x", "y"},
			new []{10, 15},
			false)]
		[TestCase(
			"(x+5)*y<2*5",
			new []{"x", "y"},
			new []{2, 3},
			false)]
		[TestCase(
			"x+x>=x*x",
			new []{"x"},
			new []{1},
			true)]
		public void RuleParser_ParseParamCondition_ValidInputTest(string input, string[] variableNames, 
			int[] variableValues, bool expectedOutput)
		{
			var expressionParserFactory = new IntExpressionParserFactory();
			var parser = new RuleParser<int>(new ModuleParser<int>(expressionParserFactory), expressionParserFactory);
			
			var intParser = expressionParserFactory.Create(variableNames);

			var handler = parser.ParseParamCondition(input, intParser);

			var output = handler(variableValues);
			
			Assert.That(output, Is.EqualTo(expectedOutput));
		}
	}
}