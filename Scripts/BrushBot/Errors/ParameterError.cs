using System;

namespace BrushBot
{
	public class ParameterError : InterpreterError
	{
		public string customMessage { get; private set; }
		public ParameterError(string message)
		{
			customMessage = message;
		}
		public override string Message
		{
			get
			{
				return customMessage;
			}
		}
	}
}