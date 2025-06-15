namespace BrushBot
{
	public class ParameterError : InterpreterError
	{
		public ParameterError(string message, (int, int) location) : base(message, location) {}
	}
}