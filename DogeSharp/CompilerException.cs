using System;
using Antlr4.Runtime;

namespace DogeSharp
{
	public class CompilerException : Exception
	{
		public CompilerException(string message, params object[] args)
			: base(string.Format(message, args))
		{
		}

		public CompilerException(IToken token, string message, params object[] args)
			: this(message + LineInfo(token), args)
		{
		}

		private static string LineInfo(IToken token)
		{
			return string.Format(" (token '{0}', line {1}, column {2})", token.Text, token.Line, token.Column);
		}
	}
}
