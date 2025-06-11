using System;
using System.Collections.Generic;
namespace BrushBot
{
    public class Parser
    {
        List<Token> Tokens;
        int Current = 0;
        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
        }
        public (List<Node>, List<InterpreterError>, List<Object>) Parse()
        {
            List<Node> nodes = new();
            List<InterpreterError> Errors = new();
            List<Object> result = new();

            while (!IsEndToken())
            {
                try
                {
                    if (UnknownToken())
                    {
                        Synchronize();
                        continue;
                    }
                    Node node = Node();
                    nodes.Add(node);
                    result.Add(node);

                    if (!IsEndToken() && CurrentToken().Type != TokenType.JumpLine && !(node is Label))
                    {
                        throw new CodeError(ErrorType.Expected, (CurrentToken().Ln, CurrentToken().Col), "JumpLine after statement.");
                    }

                    SkipJumpLine();
                }
                catch (CodeError error)
                {
                    Errors.Add(error);
                    result.Add(error);
                    Synchronize();
                }
                catch (ParameterError error)
                {
                    Errors.Add(error);
                    result.Add(error);
                    Synchronize();
                }
            }
            return (nodes, Errors, result);
        }
        private Node Node()
        {
            if (Match(TokenType.Keyword, null))
            {
                return Instruction();
            }
            else if (Match(TokenType.Identifier, null))
            {
                Token token = PreviousToken();

                if (Match(TokenType.JumpLine, null) || Match(TokenType.EndOfFile, null))
                {
                    return new Label((token.Ln, token.Col), token);
                }
                else
                {
                    return Identifier(token);
                }
            }
            else if (Match(TokenType.Function, null))
            {
                throw new CodeError(ErrorType.Invalid, (CurrentToken().Ln, CurrentToken().Col), "Function can't start a sentence.");
            }
            else
            {
                throw new CodeError(ErrorType.Invalid, (CurrentToken().Ln, CurrentToken().Col), $"{CurrentToken().Value}.");
            }
        }
        private Node Instruction()
        {
            Token Keyword = PreviousToken();

            if (Keyword.Value == "GoTo")
            {
                return GoTo(Keyword);
            }
            else return GetInstruction(Keyword);

        }
        private Instruction GetInstruction(Token keyword)
        {
            if (Match(TokenType.Delimiter, ["("]))
            {
                if (Match(TokenType.Delimiter, [")"]))
                {
                    return new Instruction((keyword.Ln, keyword.Col), keyword, null);
                }
                if (ValidateExpression())
                {
                    List<Expression> parameters = Parameters();

                    if (Match(TokenType.Delimiter, [")"]))
                    {
                        return new Instruction((keyword.Ln, keyword.Col), keyword, parameters);
                    }
                    else throw new CodeError(ErrorType.Expected, (CurrentToken().Ln, CurrentToken().Col), $").");
                }
                else throw new CodeError(ErrorType.Invalid, (CurrentToken().Ln, CurrentToken().Col), "Expression.");
            }
            else throw new CodeError(ErrorType.Expected, (CurrentToken().Ln, CurrentToken().Col), $"(.");
        }
        private List<Expression> Parameters()
        {
            List<Expression> parameters = new();
            List<CodeError> errors = new();

            try
            {
                Expression expression = Expression();
                parameters.Add(expression);
            }
            catch (CodeError error)
            {
                errors.Add(error);
                SynchronizeParameter();
            }

            while (Match(TokenType.Delimiter, [","]))
            {
                try
                {
                    Expression expression = Expression();
                    parameters.Add(expression);
                }
                catch (CodeError error)
                {
                    errors.Add(error);
                    SynchronizeParameter();
                }
            }

            if (errors.Count > 0)
            {
                string message = "Sintactical Errors in Parameters:\n";

                foreach (CodeError error in errors)
                {
                    message += error.Message + '\n';
                }
                throw new ParameterError(message);
            }
            else
            {
                return parameters;
            }
        }
        private Jump GoTo(Token keyword)
        {
            if (Match(TokenType.Delimiter, ["["]))
            {
                if (Match(TokenType.Identifier, [null]))
                {
                    Token Label = PreviousToken();

                    if (Match(TokenType.Delimiter, ["]"]))
                    {
                        if (Match(TokenType.Delimiter, ["("]))
                        {
                            if (ValidateExpression())
                            {
                                Expression expression = Expression();

                                if (Match(TokenType.Delimiter, [")"]))
                                {
                                    return new Jump((keyword.Ln, keyword.Col), keyword, Label, expression);
                                }
                                else throw new CodeError(ErrorType.Expected, (CurrentToken().Ln, CurrentToken().Col), ").");
                            }
                            else throw new CodeError(ErrorType.Invalid, (CurrentToken().Ln, CurrentToken().Col), "Expression.");
                        }
                        else throw new CodeError(ErrorType.Expected, (CurrentToken().Ln, CurrentToken().Col), "(.");
                    }
                    else throw new CodeError(ErrorType.Expected, (CurrentToken().Ln, CurrentToken().Col), $"].");
                }
                else throw new CodeError(ErrorType.Expected, (CurrentToken().Ln, CurrentToken().Col), "Label.");
            }
            else throw new CodeError(ErrorType.Expected, (CurrentToken().Ln, CurrentToken().Col), $"[.");
        }
        private Node Identifier(Token Identifier)
        {
            if (Match(TokenType.Operator, ["<-"]))
            {
                if (ValidateExpression())
                {
                    Expression expression = Expression();

                    if (CurrentToken().Type != TokenType.JumpLine && CurrentToken().Type != TokenType.EndOfFile)
                    {
                        throw new CodeError(ErrorType.Invalid, (CurrentToken().Ln, CurrentToken().Col), $"{CurrentToken().Value} after assignment.");
                    }
                    return new Assignment((Identifier.Ln, Identifier.Col), Identifier, expression);
                }
                else throw new CodeError(ErrorType.Undefined, (CurrentToken().Ln, CurrentToken().Col), $"Expression in assignment.");
            }
            else throw new CodeError(ErrorType.Expected, (CurrentToken().Ln, CurrentToken().Col), $"<-.");
        }
        #region Expressions
        private Expression Expression()
        {
            return And();
        }

        private Expression And()
        {
            Expression expression = Or();

            while (Match(TokenType.Operator, ["&&"]))
            {
                Token Operator = PreviousToken();
                Expression right = Or();
                expression = new BinaryExpression((Operator.Ln, Operator.Col), expression, Operator, right);
            }
            return expression;
        }
        private Expression Or()
        {
            Expression expression = Equality();

            while (Match(TokenType.Operator, ["||"]))
            {
                Token Operator = PreviousToken();
                Expression right = Equality();
                expression = new BinaryExpression((Operator.Ln, Operator.Col), expression, Operator, right);
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
                expression = new BinaryExpression((Operator.Ln, Operator.Col), expression, Operator, right);
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
                expression = new BinaryExpression((Operator.Ln, Operator.Col), expression, Operator, right);
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
                expression = new BinaryExpression((Operator.Ln, Operator.Col), expression, Operator, right);
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
                expression = new BinaryExpression((Operator.Ln, Operator.Col), expression, Operator, right);
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
                expression = new BinaryExpression((Operator.Ln, Operator.Col), expression, Operator, right);
            }
            return expression;
        }
        private Expression Unary()
        {
            if (Match(TokenType.Operator, ["!", "-"]))
            {
                Token Operator = PreviousToken();
                Expression expression = Unary();
                return new UnaryExpression((Operator.Ln, Operator.Col), Operator, expression);
            }
            return Primary();
        }
        private Expression Primary()
        {
            if (Match(TokenType.Delimiter, ["("]))
            {
                if (ValidateExpression())
                {
                    Expression expression = Expression();

                    if (!Match(TokenType.Delimiter, [")"]))
                    {
                        throw new CodeError(ErrorType.Expected, (CurrentToken().Ln, CurrentToken().Col), ").");
                    }
                    else
                    {
                        return expression;
                    }
                }
                else throw new CodeError(ErrorType.Invalid, (CurrentToken().Ln, CurrentToken().Col), $"Expression.");
            }

            TokenType CurrentType = CurrentToken().Type;

            if (CurrentType == TokenType.Number || CurrentType == TokenType.Color || CurrentType == TokenType.Boolean || CurrentType == TokenType.String || CurrentType == TokenType.Identifier || CurrentType == TokenType.Function)
            {
                Advance();
                Token token = PreviousToken();
                if (token.Type == TokenType.Number || token.Type == TokenType.Color || CurrentType == TokenType.Boolean || CurrentType == TokenType.String)
                {
                    return new Literal((token.Ln, token.Col), token);
                }
                else if (token.Type == TokenType.Identifier)
                {
                    return new Variable((token.Ln, token.Col), token);
                }
                else if (token.Type == TokenType.Function)
                {
                    if (Match(TokenType.Delimiter, ["("]))
                    {
                        if (Match(TokenType.Delimiter, [")"]))
                        {
                            return new Function((token.Ln, token.Col), token, null);
                        }
                        if (ValidateExpression())
                        {
                            List<Expression> parameters = Parameters();

                            if (Match(TokenType.Delimiter, [")"]))
                            {
                                return new Function((token.Ln, token.Col), token, parameters);
                            }
                            else throw new CodeError(ErrorType.Expected, (CurrentToken().Ln, CurrentToken().Col), $").");
                        }
                        else throw new CodeError(ErrorType.Invalid, (CurrentToken().Ln, CurrentToken().Col), $"Expression.");
                    }
                    else throw new CodeError(ErrorType.Expected, (CurrentToken().Ln, CurrentToken().Col), "(.");
                }
                else throw new CodeError(ErrorType.Invalid, (CurrentToken().Ln, CurrentToken().Col), "Expression.");
            }
            else throw new CodeError(ErrorType.Invalid, (CurrentToken().Ln, CurrentToken().Col), $"Expression.");
        }
        #endregion
        #region Aux
        private void Synchronize()
        {
            while (!IsEndToken())
            {
                if (CurrentToken().Type == TokenType.JumpLine)
                {
                    SkipJumpLine();
                    return;
                }
                Advance();
            }
        }
        private bool UnknownToken()
        {
            int start = Current;

            while (!IsEndToken() && CurrentToken().Type != TokenType.JumpLine)
            {
                if (CurrentToken().Type == TokenType.Unknown)
                {
                    return true;
                }
                Advance();
            }

            Current = start;
            return false;
        }
        private void SkipJumpLine()
        {
            while (Match(TokenType.JumpLine, null)) { }
        }
        private bool Match(TokenType type, string[] values)
        {
            if (values == null)
            {
                if (Check(type, null))
                {
                    Advance();
                    return true;
                }
                return false;
            }
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
            return CurrentToken().Type == type && (value == null || CurrentToken().Value == value);
        }
        private Token Advance()
        {
            if (!IsEndToken()) Current++;
            return PreviousToken();
        }
        private bool IsEndToken()
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
        private bool ValidateExpression()
        {
            return CurrentToken().Type != TokenType.Keyword &&
                   CurrentToken().Type != TokenType.Delimiter &&
                   CurrentToken().Type != TokenType.JumpLine &&
                   CurrentToken().Type != TokenType.Unknown &&
                   CurrentToken().Type != TokenType.EndOfFile;
        }
        private void SynchronizeParameter()
        {
            while (!IsEndToken())
            {
                if (CurrentToken().Value == "," || CurrentToken().Value == ")")
                {
                    return;
                }
                Advance();
            }
        }
        #endregion
    }
}