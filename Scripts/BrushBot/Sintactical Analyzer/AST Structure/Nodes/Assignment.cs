using System;

namespace BrushBot
{
	public class Assignment : Node
    {
        public Token Variable {get; }
        public Expression Expression {get; }
        public Assignment ((int , int) location, Token variable, Expression expression) : base(location)
        {
            Variable = variable;
            Expression = expression;
        }
        public (string, Object) Assign(Context context)
        {
            string Name = Variable.Value;
            Object Value = Expression.Evaluate(context);
            return (Name, Value);
        }
    }
}