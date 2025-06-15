using System;

namespace BrushBot
{
	public abstract class InterpreterError : Exception
	{
		public (int, int) Location { get; private set; } // (Ln, Col)
		public InterpreterError(string message, (int, int) location) : base(message)
		{
			Location = location;
		}
	}
}