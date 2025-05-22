namespace BrushBot
{
	public class Label : Node
    {
        public Token Token {get; }
        public Label ((int , int) location, Token token) : base (location)
        {
            Token = token;
        }
    }
}