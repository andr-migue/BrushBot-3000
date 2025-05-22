using System;

namespace BrushBot
{
	public abstract class Expression : Node
    {
		public Expression((int, int) location) : base(location) {}
		public abstract Object Evaluate(Context context);
    }
}