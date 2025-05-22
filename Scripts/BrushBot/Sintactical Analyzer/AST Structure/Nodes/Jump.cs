namespace BrushBot
{
	public class Jump : Node
    {
        public Token GoTo {get; }
        public Token Label {get; }
        public Expression Expression {get; }
        public Jump((int, int) location, Token go, Token label, Expression expression) : base(location)
        {
            GoTo = go;
            Label = label;
            Expression = expression;
        }
    }
}