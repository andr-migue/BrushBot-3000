using System;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace BrushBot
{
    public abstract class Node
    {
        public (int, int) Location;
        public Node((int, int) location)
        {
            Location = location;
        }
    }
    public abstract class Expression : Node
    {
		public Expression((int, int) location) : base(location) {}
		public abstract Object Evaluate();
    }
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
        public override Object Evaluate()
        {
            Object left = Left.Evaluate();
            Object right = Right.Evaluate();
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
            throw new CodeError (ErrorType.Invalid, Location, $"Imposible convert {value} to boolean.");
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
    public class UnaryExpression : Expression
    {
        public Token Operator {get; }
        public Expression Expression {get; }
        public UnaryExpression((int , int) location, Token oper, Expression expression) : base(location)
        {
            Operator = oper;
            Expression = expression;
        }
        public override Object Evaluate()
        {
            if (Operator.Value == "-")
            {
                Object expr = Expression.Evaluate();
                if (expr is int)
                {
                    return -(int)expr;
                }
                else throw new CodeError(ErrorType.Invalid, Location, $"No se puede aplicar el operador '-' a {Expression.Evaluate()}.");
            }
            else
            {
                Object expr = Expression.Evaluate();
                if (expr is bool)
                {
                    return !(bool)expr;
                }
                else throw new CodeError(ErrorType.Invalid, Location, $"No se puede aplicar el operador '!' a {Expression.Evaluate()}.");
            }
        }
    }
    public class Function : Expression
    {
        public Token Name {get; }
        public List<Expression> Parameters {get; }
        public Function ((int, int) location, Token name, List<Expression> parameters) : base (location)
        {
            Name = name;
            Parameters = parameters;
        }
        public override Object Evaluate()
        {
            switch (Name.Value)
            {
                case "GetActualX": return Handle.GetActualX(Parameters);
                case "GetActualY": return Handle.GetActualY(Parameters);
                case "GetCanvasSize": return Handle.GetCanvasSize(Parameters);
                case "IsBrushColor": return Handle.IsBrushColor(Parameters);
                case "IsBrushSize": return Handle.IsBrushSize(Parameters);
                case "IsCanvasColor": return Handle.IsCanvasColor(Parameters);
                case "GetColorCount": return Handle.GetColorCount(Parameters);
                default: throw new CodeError(ErrorType.Invalid, Location, $"{Name.Value}");
            }
        }
    }
    public class Literal : Expression
    {
        public Token Token;
        public Literal((int, int) location, Token token) : base(location)
        {
            Token = token;
        }
        public override Object Evaluate()
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
    public class Variable : Expression
    {
        public Token Token;
        public Variable((int , int) location, Token token) : base (location)
        {
            Token = token;
        }
        public override Object Evaluate()
        {
            if (Context.Variables.ContainsKey(Token.Value))
            {
                return Context.Variables[Token.Value];
            }
            else throw new CodeError(ErrorType.OutOfContext, Location, $"Variable {Token.Value}");
        }
    }
    public class Assignment : Node
    {
        public Token Variable {get; }
        public Expression Expression {get; }
        public Assignment ((int , int) location, Token variable, Expression expression) : base(location)
        {
            Variable = variable;
            Expression = expression;
        }
        public (string, Object) Assign()
        {
            string Name = Variable.Value;
            Object Value = Expression.Evaluate();
            return (Name, Value);
        }
    }
    public class Label : Node
    {
        public Token Token {get; }
        public Label ((int , int) location, Token token) : base (location)
        {
            Token = token;
        }
    }
    public class Instruction : Node
    {
        public Token Keyword {get; }
        public List<Expression> Parameters {get; }
        public Instruction ((int, int) location, Token keyword, List<Expression> parameters) : base(location)
        {
            Keyword = keyword;
            Parameters = parameters;
        }
        public void CheckSemantic()
        {
            switch (Keyword.Value)
            {
                case "Spawn" :
                    Handle.CheckSpawn(Parameters);
                    break;
                case "Color" :
                    Handle.CheckColor(Parameters);
                    break;
                case "Size" :
                    Handle.CheckSize(Parameters);
                    break;
                case "DrawLine" :
                    Handle.CheckDrawLine(Parameters);
                    break;
                case "DrawCircle" :
                    Handle.CheckDrawCircle(Parameters);
                    break;
                case "DrawRectangle" :
                    Handle.CheckDrawRectangle(Parameters);
                    break;
                case "Fill" :
                    Handle.CheckFill(Parameters);
                    break;
                default: throw new CodeError(ErrorType.Invalid, Location, $"{Keyword.Value}");
            }
        }
        public async Task Execute()
        {
            switch (Keyword.Value)
            {
                case "Spawn" :
                    await Handle.Spawn(Parameters);
                    break;
                case "Color" :
                    await Handle.Color(Parameters);
                    break;
                case "Size" :
                    await Handle.Size(Parameters);
                    break;
                case "DrawLine" :
                    await Handle.DrawLine(Parameters);
                    break;
                case "DrawCircle" :
                    await Handle.DrawCircle(Parameters);
                    break;
                case "DrawRectangle" :
                    await Handle.DrawRectangle(Parameters);
                    break;
                case "Fill" :
                    await Handle.Fill();
                    break;
                default: throw new CodeError(ErrorType.Invalid, Location, $"{Keyword.Value}");;
            }
        }
    }
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