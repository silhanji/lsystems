using System;
using LSystems.Parsers;
using NUnit.Framework;

namespace LSystemsTests.ParsersTests
{
	[TestFixture]
	public class HelperMethodsTests
	{
		[TestCase(
			"(content)",
			0,
			"content",
			8)]
		[TestCase(
			"long(bracket)test",
			4,
			"bracket",
			12)]
		[TestCase(
			"multiple((bracket))test",
			8,
			"(bracket)",
			18)]
		public void RuleParser_ReadBracketContent_ValidInputTest(string input, int startIndex, string expectedOutput, 
			int expectedEndIndex)
		{
			var output = HelperMethods.ReadBracketContent(input, startIndex, out int endIndex);
			
			Assert.That(output, Is.EqualTo(expectedOutput));
			Assert.That(endIndex, Is.EqualTo(expectedEndIndex));
		}
		
		[TestCase("hello(world)", 3)]
		[TestCase("hello(world)", 100)]
		public void RuleParser_ReadBracketContent_ArgumentExceptionTest(string input, int startIndex)
		{
			Assert.Throws<ArgumentException>(
				() => HelperMethods.ReadBracketContent(input, startIndex, out var endIndex)
			);
		}

		[TestCase("hello(world", 5)]
		public void RuleParser_ReadBracketsContent_ParserExceptionTest(string input, int startIndex)
		{
			Assert.Throws<ParserException>(
				() => HelperMethods.ReadBracketContent(input, startIndex, out var endIndex)
			);
		}

		[TestCase(
			"Module_name", 
			0, 
			"Module_name", 
			10)]
		[TestCase(
			"longModuleName", 
			4, 
			"Module", 
			9)]
		[TestCase(
			"Module(with)details", 
			0, 
			"Module", 
			5)]
		public void RuleParser_ReadModuleName_ValidInputTest(string input, int startIndex, string expectedOutput,
			int expectedEndIndex)
		{
			var output = HelperMethods.ReadModuleName(input, startIndex, out var endIndex);
			
			Assert.That(output, Is.EqualTo(expectedOutput));
			Assert.That(endIndex, Is.EqualTo(expectedEndIndex));
		}

		[TestCase("notUpperLetter", 2)]
		[TestCase("OutOfRange", 43)]
		public void RuleParser_ReadModuleName_ArgumentExceptionTest(string input, int startIndex)
		{
			Assert.Throws<ArgumentException>(
				() => HelperMethods.ReadModuleName(input, startIndex, out var endIndex)
			);
		}
	}
}