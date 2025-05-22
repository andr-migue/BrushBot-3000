using System;

namespace BrushBot
{
	public class Literal : Expression
    {
        public Token Token;
        public Literal((int, int) location, Token token) : base(location)
        {
            Token = token;
        }
        public override Object Evaluate(Context context)
        {
            if (Token.Type == TokenType.Number)
            {
                return int.Parse(Token.Value);
            }
            else if (Token.Type == TokenType.Color)
            {
                switch (Token.Value)
                {
                    case "Transparent" : return Color.Transparent;
                    case "Red" : return Color.Red;
                    case "Blue" : return Color.Blue;
                    case "Green" : return Color.Green;
                    case "Yellow" : return Color.Yellow;
                    case "Orange" : return Color.Orange;
                    case "Purple" : return Color.Purple;
                    case "Black" : return Color.Black;
                    case "White" : return Color.White;
                    case "Pink" : return Color.Pink;

                    default : throw new CodeError (ErrorType.Invalid, Location, $"{Token.Value}.");
                }
            }
            else throw new CodeError (ErrorType.Invalid, Location, $"{Token.Value}");
        }
    }
}