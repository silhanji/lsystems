using System.Collections.Generic;
using System.Text;

namespace LSystems.Parsers
{
	public static class StringExtension
	{
		/// <summary>
		/// Splits original string into substrings based on separators and bracketing
		/// Ensures that anything which is in brackets is not splitted even if bracket contains separator char
		/// Separator char shouldn't contain ( or )
		/// </summary>
		/// <param name="original">string which will be divided</param>
		/// <param name="separator">string used as delimiter between parts of original string</param>
		/// <returns>List of string which correspond to original string without separator character</returns>
		public static List<string> BracketAwareSplit(this string original, string separator)
		{
			var splitRepresentations = new List<string>();
			var builder = new StringBuilder();
			int bracketWidth = 0;
			int visitedChars = 0;
			for (int i = 0; i < original.Length; i++)
			{
				char c = original[i];
				visitedChars++;
				
				if (c == '(')
					bracketWidth++;
				else if (c == ')')
					bracketWidth--;

				if (visitedChars >= separator.Length)
				{
					if (bracketWidth == 0)
					{
						bool areEqual = true;
						//Compare last N chars to see if they equal to separator
						for (int n = 0; n < separator.Length; n++)
							if (original[i - separator.Length + n + 1] != separator[n])
							{
								areEqual = false;
								break;
							}


						if (areEqual)
						{
							splitRepresentations.Add(builder.ToString());
							builder.Clear();
							visitedChars = 0;
							continue;
						}
					}

					builder.Append(original[i - separator.Length+1]);
				}
			}

			//Append last characters which were not appended by for loop
			if (visitedChars < separator.Length)
				builder.Append(original.Substring(original.Length - visitedChars));
			else
				builder.Append(original.Substring(original.Length - separator.Length+1));

			splitRepresentations.Add(builder.ToString());

			return splitRepresentations;
		}
		
		/// <summary>
		/// Splits original string into substrings based on separators and bracketing
		/// Ensures that anything which is in brackets is not splitted even if bracket contains separator char
		/// Separator char shouldn't contain ( or )
		/// </summary>
		/// <param name="original">string which will be divided</param>
		/// <param name="separator">character used as delimiter between parts of original string</param>
		/// <returns>List of string which correspond to original string without separator character</returns>
		public static List<string> BracketAwareSplit(this string original, char separator) =>
			BracketAwareSplit(original, separator.ToString());
	}
}