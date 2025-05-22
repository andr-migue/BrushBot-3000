using System;

namespace BrushBot
{
	public class UnaryExpression : Expression
    {
        public Token Operator {get; }
        public Expression Expression {get; }
        public UnaryExpression((int , int) location, Token oper, Expression expression) : base(location)
        {
            Operator = oper;
            Expression = expression;
        }
        public override Object Evaluate(Context context)
        {
            if (Operator.Value == "-")
            {
                Object expr = Expression.Evaluate(context);
                if (expr is int)
                {
                    return -(int)expr;
                }
                else throw new CodeError(ErrorType.Invalid, Location, $"No se puede aplicar el operador '-' a {Expression.Evaluate(context)}.");
            }
            else
            {
                Object expr = Expression.Evaluate(context);
                if (expr is bool)
                {
                    return !(bool)expr;
                }
                else throw new CodeError(ErrorType.Invalid, Location, $"No se puede aplicar el operador '!' a {Expression.Evaluate(context)}.");
            }
        }
    }
}