namespace BrushBot
{
	public class CodeError : InterpreterError
	{
		public ErrorType Type { get; private set; }
		public string customMessage { get; private set; }
		public CodeError(ErrorType type, (int, int) location, string customMessage) : base(null, location)
		{
			Type = type;
			this.customMessage = customMessage;
		}
		public override string Message
		{
			get
			{
				return $"Ln {Location.Item1}, Col {Location.Item2}, {Type} {customMessage}";
			}
		}
	}
	public enum ErrorType
	{
		Expected,
		Invalid,
		Unknown,
		Typing,
		Count,
		IndexOutOfRange,
		OutOfContext,
		Undefined,
	}
}