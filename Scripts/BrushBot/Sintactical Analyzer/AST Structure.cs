using System.Collections.Generic;
using System.Linq.Expressions;
namespace BrushBot
{
    public abstract class Node{}
    public class Expression : Node{}
    public class Literal : Expression
    {
        public Token Expression;
        public Literal(Token expression)
        {
            Expression = expression;
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
    }
    public class GroupingExpression : Expression
    {
        public Expression Left {get; }
        public string Comma {get; }
        public Expression Right {get; }
        public GroupingExpression(Expression left, string comma, Expression right)
        {
            Left = left;
            Comma = comma;
            Right = right;
        }
    }
    public class Assignment : Node
    {
        public Token Variable {get; }
        public Token Operator {get; }
        public Expression Expression {get; }
        public Assignment (Token variable, Token oper, Expression expression)
        {
            Variable = variable;
            Operator = oper;
            Expression = expression;
        }
    }
    public class Instruction : Node
    {
        public Token Keyword {get; }
        public Expression Expression {get; }
        public Instruction (Token keyword, Expression expression)
        {
            Keyword = keyword;
            Expression = expression;
        }
    }
    public class Jump : Instruction
    {
        public Token Label {get; }
        public Jump(Token GoTo, Token label, Expression expression) : base(GoTo, expression)
        {
            Label = label;
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
    public class Variable : Literal
    {
        public Expression Value {get; }
        public Variable (Token token, Expression value) : base(token)
        {
            Value = value;
        }
    }
    public class Function : Expression
    {
        public Token Token {get; }
        public Expression Value {get; }
        public Function (Token token, Expression value)
        {
            Token = token;
            Value = value;
        }
    }
}