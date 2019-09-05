using System;

namespace LSystems.Utils.Parsers
{
	[Serializable()]
	public class ParserException : Exception
	{
		public ParserException() : base() { }
		public ParserException(string msg) : base(msg) { }
		public ParserException(string msg, Exception ex) : base(msg, ex) { }

		public ParserException(System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}