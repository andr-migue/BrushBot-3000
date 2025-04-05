using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
namespace Interpreter
{
    public class ParserError : Exception
    {
        public ParserError(string message) : base(message){}
    }
    public class Parser
    {
        List<Token> Tokens;
        int Current = 0;
        private List<ParserError> Errors = new();
        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
        }
        public Expression Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParserError error)
            {
                Errors.Add(error);
                Synchronize();
                return null;
            }
        }
        public ParserError Error(Token token, string message)
        {
            if (token.Type == TokenType.EndOfFile)
            {
                return new ParserError($"Ln: {token.Ln} al final {message}");
            }
            else return new ParserError($"Ln: {token.Ln} Col: {token.Col} {message}");
        }
        private bool Match(TokenType type, string[] values)
        {
            foreach (var value in values)
            {
                if (Check(type, value))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }
        private bool Check(TokenType type, string value)
        {
            if (IsEnd()) return false;
            return CurrentToken().Type == type && CurrentToken().Value == value;
        }
        private Token Advance()
        {
            if (!IsEnd()) Current++;
            return PreviousToken();
        }
        private bool IsEnd()
        {
            return CurrentToken().Type == TokenType.EndOfFile;
        }
        private Token CurrentToken()
        {
            return Tokens[Current];
        }
        private Token PreviousToken()
        {
            return Tokens[Current - 1];
        }
        private void Synchronize()
        {
            Advance();
            while (!IsEnd())
            {
                if (PreviousToken().Type == TokenType.JumpLine && CurrentToken().Type == TokenType.Keyword)
                {
                    return;
                }
                if (CurrentToken().Type == TokenType.Color || CurrentToken().Type == TokenType.Number || CurrentToken().Value == "(" || CurrentToken().Type == TokenType.Variable)
                {
                    return;
                }
                Advance();
            }
        }
        private Expression Expression()
        {
            return Or();
        }
        private Expression Or()
        {
            Expression expression = And();

            while (Match(TokenType.Operator, ["||"]))
            {
                Token Operator = PreviousToken();
                Expression right = And();
                expression = new BinaryExpression(expression, Operator, right, true);
            }
            return expression;
        }
        private Expression And()
        {
            Expression expression = Equality();

            while (Match(TokenType.Operator, ["&&"]))
            {
                Token Operator = PreviousToken();
                Expression right = Equality();
                expression = new BinaryExpression(expression, Operator, right, true);
            }
            return expression;
        }
        private Expression Equality()
        {
            Expression expression = Comparison();

            while (Match(TokenType.Operator, ["!=", "=="]))
            {
                Token Operator = PreviousToken();
                Expression right = Comparison();
                expression = new BinaryExpression(expression, Operator, right, true);
            }
            return expression;
        }
        private Expression Comparison()
        {
            Expression expression = Term();

            while (Match(TokenType.Operator, ["<", ">", "<=", ">="]))
            {
                Token Operator = PreviousToken();
                Expression right = Term();
                expression = new BinaryExpression(expression, Operator, right, false);
            }
            return expression;
        }
        private Expression Term()
        {
            Expression expression = Factor();

            while (Match(TokenType.Operator, ["+", "-"]))
            {
                Token Operator = PreviousToken();
                Expression right = Factor();
                expression = new BinaryExpression(expression, Operator, right, false);
            }
            return expression;
        }
        private Expression Factor()
        {
            Expression expression = Pow();

            while (Match(TokenType.Operator, ["*", "/", "%"]))
            {
                Token Operator = PreviousToken();
                Expression right = Pow();
                expression = new BinaryExpression(expression, Operator, right, false);
            }
            return expression;
        }
        private Expression Pow()
        {
            Expression expression = Unary();

            while (Match(TokenType.Operator, ["**"]))
            {
                Token Operator = PreviousToken();
                Expression right = Unary();
                expression = new BinaryExpression(expression, Operator, right, false);
            }
            return expression;
        }
        private Expression Unary()
        {
            Expression expression = Primary();

            if (Match(TokenType.Operator, ["!", "-"]))
            {
                Token Operator = PreviousToken();
                expression = Unary();
                return new UnaryExpression(Operator, expression, Operator.Value == "!");
            }
            return expression;
        }
        private Expression Primary()
        {
            if (Match(TokenType.Delimiter, ["("]))
            {
                Expression expression = Expression();
                if (!Match(TokenType.Delimiter, [")"]))
                {
                    throw Error(CurrentToken(), "Se espera ) después de la expresión");
                }
                else
                {
                    return new GroupingExpression(expression);
                }
            }
            else if (CurrentToken().Type == TokenType.Number)
            {
                Advance();
                return new Literal(TokenType.Number, PreviousToken().Value);
            }
            else if (CurrentToken().Type == TokenType.Color)
            {
                Advance();
                return new Literal(TokenType.Color, PreviousToken().Value);
            }
            else throw Error(CurrentToken(), "Token inesperado");
        }
    }
}