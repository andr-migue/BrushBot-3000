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
        public (List<Node>, List<ParserError>, List<Object>) Parse()
        {
            List<Node> nodes = new();
            List<ParserError> Errors = new();
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
                    SkipJumpLine();
                }
                catch (ParserError error)
                {
                    Errors.Add(error);
                    result.Add(error);
                    Synchronize();
                }
            }
            return (nodes, Errors, result);
        }
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
        private ParserError Error(Token token, string message)
        {
            return new ParserError($"Error: Ln {token.Ln} Col {token.Col} {message}");
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
            while (Match(TokenType.JumpLine, null)) {}
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
                    return new Label(token);
                }
                else
                {
                    return Identifier(token);
                }
            }
            else if (Match(TokenType.Function, null))
            {
                throw Error (PreviousToken(), "Funciones no pueden comenzar una sentencia.");
            }
            else
            {
                throw Error(CurrentToken(), $"Sentencia no válida. Token inesperado: '{CurrentToken().Value}' de tipo {CurrentToken().Type}.");
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
                    return new Instruction(keyword, null);
                }
                if (ValidateExpression())
                {
                    Expression expression = Expression();

                    if (Match(TokenType.Delimiter, [")"]))
                    {
                        return new Instruction(keyword, expression);
                    }
                    else throw Error(CurrentToken(), "Se espera ).");
                }
                else throw Error(CurrentToken(), "Expresión no válida.");
            }
            else throw Error(CurrentToken(), $"Se espera '(' después de la instrucción '{keyword.Value}'.");
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
                                    return new Jump(keyword, Label, expression);
                                }
                                else throw Error(CurrentToken(), "Se espera ).");
                            }
                            else throw Error(CurrentToken(), "Expresión no válida.");
                        }
                        else throw Error(CurrentToken(), "Se espera (.");
                    }
                    else throw Error(CurrentToken(), $"Se espera ].");
                }
                else throw Error(CurrentToken(), "Se espera un Label.");
            }
            else throw Error(CurrentToken(), $"Se esperaba '[', pero se encontró '{CurrentToken().Value}'.");
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
                        throw Error(CurrentToken(), "Tokens inesperados después de la expresión de asignación.");
                    }
                    return new Assignment(Identifier, expression);
                }
                else throw Error(CurrentToken(), $"Expresión no válida en la asignación a '{Identifier.Value}'.");
            }
            else throw Error(CurrentToken(), $"Se espera '<-' después del identificador '{Identifier.Value}'.");
        }
        private Expression Expression()
        {
            return Grouping();
        }
        private Expression Grouping()
        {
            List<Expression> parameters = Parameters();

            if (parameters.Count == 1)
            {
                return parameters[0];
            }

            else return Build(parameters);
        }
        private List<Expression> Parameters()
        {
            List<Expression> parameters = new List<Expression>();
            List<ParserError> errors = new List<ParserError>();

            try
            {
                parameters.Add(And());
            }
            catch (ParserError error)
            {
                errors.Add(error);
                SynchronizeParameter();
            }

            while (CurrentToken().Type == TokenType.Delimiter && CurrentToken().Value == ",")
            {
                Advance();
                
                try
                {
                    parameters.Add(And());
                }
                catch (ParserError error)
                {
                    errors.Add(error);
                    SynchronizeParameter();
                }
            }

            if (parameters.Count == 1)
            {
                if (errors.Count > 0)
                {
                    throw errors[0];
                }
                else return parameters;
            }
            else
            {
                if (errors.Count > 0)
                {
                    string errorMessage = "Errores en los parámetros:\n";
                    foreach (ParserError error in errors)
                    {
                        errorMessage += $"- {error.Message}\n";
                    }
                    throw new ParserError(errorMessage);
                }
                return parameters;
            }
        }
        private void SynchronizeParameter()
        {
            while (!IsEndToken())
            {
                if (CurrentToken().Type == TokenType.Delimiter && (CurrentToken().Value == "," || CurrentToken().Value == ")"))
                {
                    return;
                }
                Advance();
            }
        }
        private Expression Build(List<Expression> elements)
        {
            Expression expression = elements[elements.Count - 1];

            for (int i = elements.Count - 2; i >= 0; i--)
            {
                expression = new GroupingExpression(elements[i], expression);
            }
            return expression;
        }
        private Expression And()
        {
            Expression expression = Or();

            while (Match(TokenType.Operator, ["&&"]))
            {
                Token Operator = PreviousToken();
                Expression right = Or();
                expression = new BinaryExpression(expression, Operator, right);
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
                expression = new BinaryExpression(expression, Operator, right);
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
                expression = new BinaryExpression(expression, Operator, right);
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
                expression = new BinaryExpression(expression, Operator, right);
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
                expression = new BinaryExpression(expression, Operator, right);
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
                expression = new BinaryExpression(expression, Operator, right);
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
                expression = new BinaryExpression(expression, Operator, right);
            }
            return expression;
        }
        private Expression Unary()
        {
            if (Match(TokenType.Operator, ["!", "-"]))
            {
                Token Operator = PreviousToken();
                Expression expression = Unary();
                return new UnaryExpression(Operator, expression);
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
                        throw Error(CurrentToken(), "Se espera ')'.");
                    }
                    else
                    {
                        return expression;
                    }
                }
                else throw Error(CurrentToken(), $"Expresión no válida. Token inesperado: '{CurrentToken().Value}' de tipo {CurrentToken().Type}.");
            }

            TokenType CurrentType = CurrentToken().Type;

            if (CurrentType == TokenType.Number || CurrentType == TokenType.Color || CurrentType == TokenType.Identifier || CurrentType == TokenType.Function)
            {
                Advance();
                Token token = PreviousToken();
                if (token.Type == TokenType.Number || token.Type == TokenType.Color)
                {
                    return new Literal(token);
                }
                else if (token.Type == TokenType.Identifier)
                {
                    return new Variable(token);
                }
                else if (token.Type == TokenType.Function)
                {
                    if (Match(TokenType.Delimiter, ["("]))
                    {
                        if (Match(TokenType.Delimiter, [")"]))
                        {
                            return new Function(token, null);
                        }
                        if (ValidateExpression())
                        {
                            Expression expression = Expression();

                            if (Match(TokenType.Delimiter, [")"]))
                            {
                                return new Function(token, expression);
                            }
                            else throw Error (CurrentToken(), "Se espera ')'.");
                        }
                        else throw Error (CurrentToken(), $"Expresión no válida de '{token.Value} de tipo {token.Type}'.");
                    }
                    else throw Error (CurrentToken(), "Se espera '('.");
                }
                else throw Error(CurrentToken(), "Expresión no válida.");
            }
            else throw Error(CurrentToken(), $"Expresión no válida. Token inesperado: '{CurrentToken().Value}' de tipo {CurrentToken().Type}.");
        }
    }
}