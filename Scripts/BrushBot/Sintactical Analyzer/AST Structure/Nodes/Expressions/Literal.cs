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
                    case "Transparent": return new Color(PredefColor.Transparent);
                    case "Red": return new Color(PredefColor.Red);
                    case "Blue": return new Color(PredefColor.Blue);
                    case "Green": return new Color(PredefColor.Green);
                    case "Yellow": return new Color(PredefColor.Yellow);
                    case "Orange": return new Color(PredefColor.Orange);
                    case "Purple": return new Color(PredefColor.Purple);
                    case "Black": return new Color(PredefColor.Black);
                    case "White": return new Color(PredefColor.White);
                    case "Pink": return new Color(PredefColor.Pink);

                    default: throw new CodeError(ErrorType.Invalid, Location, $"{Token.Value}.");
                }
            }
            else if (Token.Type == TokenType.Boolean)
            {
                if (Token.Value == "true") return true;

                else return false;
            }
            else if (Token.Type == TokenType.String)
            {
                return Token.Value;
            }
            else throw new CodeError(ErrorType.Invalid, Location, $"{Token.Value}");
        }
    }
}