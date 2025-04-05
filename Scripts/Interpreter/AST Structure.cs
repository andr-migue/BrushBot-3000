using System.Runtime.InteropServices;

namespace Interpreter
{
    public abstract class Node{}
    public class Expression : Node{}
    public class Literal : Expression
    {
        public TokenType Type {get; }
        public string Value {get; }
        public Literal(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
    public class UnaryExpression : Expression
    {
        public Token Operator {get; }
        public Expression Expression {get; }
        public bool IsLogical {get; }
        public UnaryExpression(Token oper, Expression expression, bool islogical)
        {
            Operator = oper;
            Expression = expression;
            IsLogical = islogical;
        }
    }
    public class BinaryExpression : Expression
    {
        public Expression Left {get; }
        public Token Operator {get; }
        public Expression Right {get; }
        public bool IsLogical {get; }
        public BinaryExpression(Expression left, Token oper, Expression right, bool islogical)
        {
            Left = left;
            Operator = oper;
            Right = right;
            IsLogical = islogical;
        }
    }
    public class GroupingExpression : Expression
    {
        public Expression Expression {get; }
        public GroupingExpression(Expression expression)
        {
            Expression = expression;
        }
    }
}