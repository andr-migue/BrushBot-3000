using System;

namespace BrushBot
{
	public class BinaryExpression : Expression
    {
        public Expression Left {get; }
        public Token Operator {get; }
        public Expression Right {get; }
        public BinaryExpression((int, int) location, Expression left, Token oper, Expression right) : base(location)
        {
            Left = left;
            Operator = oper;
            Right = right;
        }
        public override Object Evaluate(Context context)
        {
            Object left = Left.Evaluate(context);
            Object right = Right.Evaluate(context);
            string oper = Operator.Value;

            if (oper == "&&" || oper == "||")
            {
                return HandleBool(left, right, oper);
            }
            else if (oper == "==" || oper == "!=")
            {
                if (left is int && right is int)
                {
                    int newLeft = (int)left;
                    int newRight = (int)right;

                    return oper == "==" ? newLeft == newRight : newLeft != newRight;
                }
                else if (left is bool || right is bool)
                {
                    return HandleBool(left, right, oper);
                }
                else throw new CodeError(ErrorType.Invalid, Location,$"{left} and {right} are not comparables.");
            }
            else if (left is int && right is int)
            {
                int newLeft = (int)left;
                int newRight = (int)right;
                
                switch (oper)
                {
                    case "==": return newLeft == newRight;
                    case "!=": return newLeft != newRight;
                    case "<": return newLeft < newRight;
                    case ">": return newLeft > newRight;
                    case "<=": return newLeft <= newRight;
                    case ">=": return newLeft >= newRight;
                    case "+": return newLeft + newRight;
                    case "-": return newLeft - newRight;
                    case "*": return newLeft * newRight;
                    case "/":
                        if (newRight != 0)
                        {
                            return newLeft / newRight;
                        }
                        else throw new CodeError(ErrorType.Undefined, Location, "Division by 0.");
                    case "%":
                        if (newRight != 0)
                        {
                            return newLeft % newRight;
                        }
                        else throw new CodeError(ErrorType.Undefined, Location, "Division by 0.");
                    case "**": return Math.Pow(newLeft, newRight);

                    default: throw new CodeError(ErrorType.Invalid, Location, $"{oper}.");
                }
            }
            else throw new CodeError(ErrorType.Invalid, Location, $"{Operator.Value}.");
        }
        private bool ToBool(Object value)
        {
            if (value is bool b) return b;
            if (value is int n) return n != 0;
            throw new CodeError (ErrorType.Invalid, Location, $"Can't convert {value} to boolean.");
        }
        private Object HandleBool(Object left, Object right, string oper)
        {
            bool leftBool = ToBool(left);
            bool rightBool = ToBool(right);

            switch (oper)
            {
                case "&&": return leftBool && rightBool;
                case "||": return leftBool || rightBool;
                case "==": return leftBool == rightBool;
                case "!=": return leftBool != rightBool;
                default: throw new CodeError(ErrorType.Invalid, Location,$"{oper}.");
            }
        }
    }
}