using System;

namespace LSystems.Parsers
{
	public static class HelperMethods
	{
		/// <summary>
		/// Reads content of bracket and returns it as string
		/// </summary>
		/// <param name="input">string containing some bracket</param>
		/// <param name="startIndex">index at which bracket starts</param>
		/// <param name="endIndex">index at which bracket ends</param>
		/// <returns>Content of bracket (without bracket itself)</returns>
		/// <exception cref="ArgumentException">
		/// Thrown if there is no opening bracket at input[startIndex] or if start index is not in interval
		/// [0, input.Length]
		/// </exception>
		/// <exception cref="ParserException">Thrown if bracket is not closed</exception>
		public static string ReadBracketContent(string input, int startIndex, out int endIndex)
		{
			if(startIndex < 0 || startIndex >= input.Length) 
				throw new ArgumentException("startIndex is out of range");
			if(input[startIndex] != '(') 
				throw new ArgumentException("input doesn't contains ( at startIndex");

			int bracketDepth = 0;
			for(int i = startIndex; i < input.Length; i++)
			{
				
				switch(input[i])
				{
					case '(':
						bracketDepth++;
						break;
					case ')':
						bracketDepth--;
						if (bracketDepth == 0)
						{
							endIndex = i;
							return input.Substring(startIndex+1, (endIndex - startIndex -1)); //content
																					//of bracket without bracket itself
						}
						break;
				}
			}
			
			throw new ParserException("Bracket at index: " + startIndex + " is not closed");
		}
		
		/// <summary>
		/// Returns module name which is contained at input string and starts at startIndex
		/// </summary>
		/// <param name="input">string containing module name</param>
		/// <param name="startIndex">index at which module name starts</param>
		/// <param name="endIndex">index at which module name ends</param>
		/// <returns>string with module name</returns>
		/// <exception cref="ArgumentException">Thrown if input[startIndex] doesn't contain upper case letter or if
		/// startIndex is not in range of input string</exception>
		public static string ReadModuleName(string input, int startIndex, out int endIndex)
		{
			if(startIndex < 0 || startIndex >= input.Length) 
				throw new ArgumentException("startIndex is out of range");
			while (char.IsWhiteSpace(input[startIndex]))
				startIndex++;
			if(!char.IsUpper(input[startIndex])) 
				throw new ArgumentException("input doesn't contains upper case letter at startIndex");

			for (int i = startIndex+1; i < input.Length; i++)
			{
				char c = input[i];
				if (char.IsLower(c) || char.IsDigit(c) || c == '_')
				{
					//character is valid continuation of module name
				}
				else
				{
					//character is not valid continuation of module name, return all found characters
					endIndex = i - 1;
					return input.Substring(startIndex, (endIndex - startIndex+1));
				}
			}

			endIndex = input.Length - 1;
			return input.Substring(startIndex);
		}
	}
}