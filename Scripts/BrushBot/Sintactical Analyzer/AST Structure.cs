using System;
using System.Threading.Tasks;
namespace BrushBot
{
    public abstract class Node {}
    public abstract class Expression : Node
    {
        public abstract Object Interpret();
    }
    public class BinaryExpression : Expression
    {
        public Expression Left {get; }
        public Token Operator {get; }
        public Expression Right {get; }
        public BinaryExpression(Expression left, Token oper, Expression right)
        {
            Left = left;
            Operator = oper;
            Right = right;
        }
        public override Object Interpret()
        {
            Object left = Left.Interpret();
            Object right = Right.Interpret();
            string oper = Operator.Value;
            if (oper == "&&")
            {
                if (left is bool && right is bool)
                {
                    return (bool)left && (bool)right;
                }
                else throw new SemanticalError($"Error: No se puede aplicar el operador '&&' entre {left} y {right}.");
            }
            else if (oper == "||")
            {
                if (left is bool && right is bool)
                {
                    return (bool)left || (bool)right;
                }
                else throw new SemanticalError($"Error: No se puede aplicar el operador '||' entre {left} y {right}.");
            }
            else if ((oper == "==") && (left is bool && right is bool))
            {
                return (bool)left == (bool)right;
            }
            else if ((oper == "!=") && (left is bool && right is bool))
            {
                return (bool)left != (bool)right;
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
                        else throw new SemanticalError("Error: No está definida la división por 0.");
                    case "%":
                        if (newRight != 0)
                        {
                            return newLeft % newRight;
                        }
                        else throw new SemanticalError("Error: No está definida la división por 0.");
                    case "**": return Math.Pow(newLeft, newRight);

                    default: throw new SemanticalError($"Error: Operador no válido {oper}.");
                }
            }
            else throw new SemanticalError($"Error: Operación Binaria no válida.");
        }
    }
    public class UnaryExpression : Expression
    {
        public Token Operator {get; }
        public Expression Expression {get; }
        public UnaryExpression(Token oper, Expression expression)
        {
            Operator = oper;
            Expression = expression;
        }
        public override Object Interpret()
        {
            if (Operator.Value == "-")
            {
                Object expr = Expression.Interpret();
                if (expr is int)
                {
                    return -(int)expr;
                }
                else throw new SemanticalError($"Error: No se puede aplicar el operador '-' a {Expression}.");
            }
            else
            {
                Object expr = Expression.Interpret();
                if (expr is bool)
                {
                    return !(bool)expr;
                }
                else throw new SemanticalError($"Error: No se puede aplicar el operador '!' a {Expression}.");
            }
        }
    }
    public class GroupingExpression : Expression
    {
        public Expression Left {get; }
        public Expression Right {get; }
        public GroupingExpression(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }
        public override Object Interpret()
        {
            return (Left.Interpret(), Right.Interpret());
        }
    }
    public class Function : Expression
    {
        public Token Name {get; }
        public Expression Arguments {get; }
        public Function (Token name, Expression arguments)
        {
            Name = name;
            Arguments = arguments;
        }
        public override Object Interpret()
        {
            switch (Name.Value)
            {
                case "GetActualX": return Handle.GetActualX();
                case "GetActualY": return Handle.GetActualY();
                case "GetCanvasSize": return Handle.GetCanvasSize();
                case "IsBrushColor": return Handle.IsBrushColor(Arguments);
                case "IsBrushSize": return Handle.IsBrushSize(Arguments);
                case "IsCanvasColor": return Handle.IsCanvasColor(Arguments);
                case "GetColorCount": return Handle.GetColorCount(Arguments);
                default: throw new SemanticalError("Error: Function no reconocida");
            }
        }
    }
    public class Literal : Expression
    {
        public Token Token;
        public Literal(Token token)
        {
            Token = token;
        }
        public override Object Interpret()
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

                    default : throw new SemanticalError ($"Error: Color no válido.");
                }
            }
            else throw new SemanticalError ($"Error: Literal no válido");
        }
    }
    public class Variable : Expression
    {
        public Token Token;
        public Variable(Token token)
        {
            Token = token;
        }
        public override Object Interpret()
        {
            if (Scope.Variables.ContainsKey(Token.Value))
            {
                return Scope.Variables[Token.Value];
            }
            else throw new SemanticalError($"Error: {Token.Value} no existe en este contexto");
        }
    }
    public class Assignment : Node
    {
        public Token Variable {get; }
        public Expression Expression {get; }
        public Assignment (Token variable, Expression expression)
        {
            Variable = variable;
            Expression = expression;
        }
        public (string, Object) Assign()
        {
            string Name = Variable.Value;
            Object Value = Expression.Interpret();
            return (Name, Value);
        }
    }
    public class Label : Node
    {
        public Token Token {get; }
        public Label (Token token)
        {
            Token = token;
        }
    }
    public class Instruction : Node
    {
        public Token Keyword {get; }
        public Expression Arguments {get; }
        public Instruction (Token keyword, Expression arguments)
        {
            Keyword = keyword;
            Arguments = arguments;
        }
        public void CheckSemantic()
        {
            switch (Keyword.Value)
            {
                case "Spawn" :
                    Handle.CheckSpawn(Arguments);
                    break;
                case "Color" :
                    Handle.CheckColor(Arguments);
                    break;
                case "Size" :
                    Handle.CheckSize(Arguments);
                    break;
                case "DrawLine" :
                    Handle.CheckDrawLine(Arguments);
                    break;
                case "DrawCircle" :
                    Handle.CheckDrawCircle(Arguments);
                    break;
                case "DrawRectangle" :
                    Handle.CheckDrawRectangle(Arguments);
                    break;
                case "Fill" :
                    Handle.CheckFill(Arguments);
                    break;
                default: throw new SemanticalError("Error: Instruction no reconocida");
            }
        }
        public async Task Execute()
        {
            switch (Keyword.Value)
            {
                case "Spawn" :
                    await Handle.Spawn(Arguments);
                    break;
                case "Color" :
                    await Handle.Color(Arguments);
                    break;
                case "Size" :
                    await Handle.Size(Arguments);
                    break;
                case "DrawLine" :
                    await Handle.DrawLine(Arguments);
                    break;
                case "DrawCircle" :
                    await Handle.DrawCircle(Arguments);
                    break;
                case "DrawRectangle" :
                    await Handle.DrawRectangle(Arguments);
                    break;
                case "Fill" :
                    await Handle.Fill();
                    break;
                default: throw new RuntimeError("Error: Instruccion no reconocida");;
            }
        }
    }
    public class Jump : Node
    {
        public Token GoTo {get; }
        public Token Label {get; }
        public Expression Expression {get; }
        public Jump(Token go, Token label, Expression expression)
        {
            GoTo = go;
            Label = label;
            Expression = expression;
        }

    }
}