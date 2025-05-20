using System;

namespace BrushBot
{
	public class CodeError : Exception
	{
		public ErrorType Type { get; private set; }
		public (int, int) Location { get; private set; } // (int Ln, int Col)
		public string customMessage { get; private set; }
		public CodeError(ErrorType type, (int, int) location, string customMessage)
		{
			Type = type;
			Location = location;
			this.customMessage = customMessage;
		}
		public override string Message
		{
			get
			{
				return $"Error: {Type} Ln {Location.Item1} Col {Location.Item2}: {customMessage}";
			}
		}
	}
	public enum ErrorType
	{
		Expected,
		Invalid,
		Null,
		Unknown
	}
}