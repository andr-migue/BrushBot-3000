using System;

namespace BrushBot
{
	public class Variable : Expression
    {
        public Token Token;
        public Variable((int , int) location, Token token) : base (location)
        {
            Token = token;
        }
        public override Object Evaluate(Context context)
        {
            if (context.Scope.TryGet(Token.Value, out var value))
            {
                return value;
            }
            else throw new CodeError(ErrorType.OutOfContext, Location, $"Variable {Token.Value}");
        }
    }
}