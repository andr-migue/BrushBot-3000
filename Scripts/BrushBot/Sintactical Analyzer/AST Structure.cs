using System;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace BrushBot
{
    public abstract class Node {}
    public abstract class Expression : Node
    {
        public abstract Object Evaluate();
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
                else throw new SemanticalError($"Error: Operandos {left} y {right} no son comparables.");
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
        private bool ToBool(Object value)
        {
            if (value is bool b) return b;
            if (value is int n) return n != 0;
            throw new SemanticalError ($"Error: No se puede convertir {value} a booleano.");
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
                default: throw new SemanticalError($"Error: Operador booleano no valido {oper}.");
            }
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
        public override Object Evaluate()
        {
            if (Operator.Value == "-")
            {
                Object expr = Expression.Evaluate();
                if (expr is int)
                {
                    return -(int)expr;
                }
                else throw new SemanticalError($"Error: No se puede aplicar el operador '-' a {Expression}.");
            }
            else
            {
                Object expr = Expression.Evaluate();
                if (expr is bool)
                {
                    return !(bool)expr;
                }
                else throw new SemanticalError($"Error: No se puede aplicar el operador '!' a {Expression}.");
            }
        }
    }
    public class Function : Expression
    {
        public Token Name {get; }
        public List<Expression> Parameters {get; }
        public Function (Token name, List<Expression> parameters)
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
        public override Object Evaluate()
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
            Object Value = Expression.Evaluate();
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
        public List<Expression> Parameters {get; }
        public Instruction (Token keyword, List<Expression> parameters)
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
                default: throw new SemanticalError("Error: Instruction no reconocida");
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