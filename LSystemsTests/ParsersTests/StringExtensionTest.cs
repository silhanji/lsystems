using LSystems.Parsers;
using NUnit.Framework;
using System.Collections.Generic;

namespace LSystemsTests.ParsersTests
{
	[TestFixture]
	public class StringExtensionTest
	{
		[TestCase("A,B,C", ",", new []{"A", "B", "C"})]
		[TestCase("A,,B,,C", ",,", new []{"A", "B", "C"})]
		[TestCase("A,(B,C)", ",", new []{"A", "(B,C)"})]
		[TestCase("(A,B,C)", ",", new []{"(A,B,C)"})]
		[TestCase("(A,B,C)", ",,,", new []{"(A,B,C)"})]
		[TestCase("A,B,C", "?", new []{"A,B,C"})]
		public void RuleParser_BracketAwareSplit_ValidInput(string input, string separator, string[] expectedOutputSrc)
		{
			var output = input.BracketAwareSplit(separator);
			var expectedOutput = new List<string>(expectedOutputSrc);
			
			Assert.That(output, Is.EqualTo(expectedOutput));
		}
	}
}