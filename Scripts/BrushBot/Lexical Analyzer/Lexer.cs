using System.Collections.Generic;
namespace BrushBot
{
    public class Lexer
    {
        private string Text;
        private int Position;
        private List<Token> tokens;
        private char CurrentChar;
        private int CurrentLn = 1;
        private int CurrentCol = 1;
        private List<InterpreterError> Errors;
        public Lexer(string text)
        {
            Text = text;
            Position = 0;
            tokens = new();
            CurrentChar = Text.Length > 0 ? Text[0] : '\0';
            Errors = new();
        }
        private HashSet<string> Keywords = new HashSet<string>
        {
            "Spawn", "Respawn", "Color", "Size", "Fill", "GoTo", "DrawLine", "DrawCircle", "DrawRectangle", "Print"
        };
        private HashSet<string> Functions = new HashSet<string>
        {
            "GetActualX", "GetActualY", "GetCanvasSize", "GetColorCount", "IsBrushColor", "IsBrushSize",
            "IsCanvasColor", "RGBA"
        };
        private HashSet<string> Operators = new HashSet<string>
        {
            "<-","**", "<=", ">=", "==", "!=", "&&", "||", "+", "-", "*", "/", "<", ">", "%", "!"
        };
        private HashSet<string> Delimiters = new HashSet<string>
        {
            "(", ")", "[", "]", ",",
        };
        private HashSet<string> Colors = new HashSet<string>
        {
            "Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Black", "White", "Transparent", "Pink"
        };
        private HashSet<string> Booleans = new HashSet<string>
        {
            "true", "false"
        };
        private void Advance()
        {
            Position++;
            CurrentCol++;
            CurrentChar = Position < Text.Length ? Text[Position] : '\0';
        }
        private char Peek()
        {
            int PeekPosition = Position + 1;
            return PeekPosition < Text.Length ? Text[PeekPosition] : '\0';
        }
        private Token GetJumpLine()
        {
            int ActualLn = CurrentLn;
            int ActualCol = CurrentCol;
            CurrentLn++;
            if (CurrentChar == '\r' && Peek() == '\n') // Windows
            {
                Advance();
                Advance();
                CurrentCol = 1;
                return new Token(TokenType.JumpLine, "JumpLine", ActualLn, ActualCol);
            }
            else // Mac o Unix
            {
                Advance();
                CurrentCol = 1;
                return new Token(TokenType.JumpLine, "JumpLine", ActualLn, ActualCol);
            }
        }
        private Token GetNumber()
        {
            string result = "";
            int ActualCol = CurrentCol;
            while (CurrentChar != '\0' && char.IsDigit(CurrentChar))
            {
                result += CurrentChar;
                Advance();
            }
            return new Token(TokenType.Number, result, CurrentLn, ActualCol);
        }
        private Token GetString()
        {
            Advance(); // Para saltar la comilla de apertura.

            string result = "";
            int ActualCol = CurrentCol;

            while (CurrentChar != '\0' && CurrentChar != '"')
            {
                result += CurrentChar;
                Advance();
            }

            if (CurrentChar != '"')
            {
                Errors.Add(new CodeError(ErrorType.Expected, (CurrentLn, CurrentCol), $"{"\""}."));

                return new Token(TokenType.Unknown, result, CurrentLn, CurrentCol);
            }
            
            Advance(); // Para saltar la comilla de cierre.

            if (Colors.Contains(result)) return new Token(TokenType.Color, result, CurrentLn, ActualCol);
            else
            {
                return new Token(TokenType.String, result, CurrentLn, ActualCol);
            }
        }
        private Token GetWord()
        {
            string result = "";
            int ActualCol = CurrentCol;

            while (CurrentChar != '\0' && (char.IsLetterOrDigit(CurrentChar) || CurrentChar == '_'))
            {
                result += CurrentChar;
                Advance();
            }

            if (Keywords.Contains(result))
            {
                return new Token(TokenType.Keyword, result, CurrentLn, ActualCol);
            }

            else if (Functions.Contains(result))
            {
                return new Token(TokenType.Function, result, CurrentLn, ActualCol);
            }

            else if (Booleans.Contains(result))
            {
                return new Token(TokenType.Boolean, result, CurrentLn, ActualCol);
            }

            else return new Token(TokenType.Identifier, result, CurrentLn, ActualCol);
        }
        private Token GetOperatorOrDelimiter()
        {
            string result = "";
            result += CurrentChar;
            int ActualCol = CurrentCol;

            if (Operators.Contains(result) || result == "=")
            {
                string next = result + Peek();

                if (Operators.Contains(next))
                {
                    Advance();
                    Advance();
                    return new Token(TokenType.Operator, next, CurrentLn, ActualCol);
                }
                else if (Operators.Contains(result))
                {
                    Advance();
                    return new Token(TokenType.Operator, result, CurrentLn, ActualCol);
                }
                else
                {
                    Advance();
                    Errors.Add(new CodeError(ErrorType.Unknown, (CurrentLn, ActualCol), $"{result}."));

                    return new Token(TokenType.Unknown, result, CurrentLn, ActualCol);
                }
            }
            else if (Delimiters.Contains(result))
            {
                Advance();
                return new Token(TokenType.Delimiter, result, CurrentLn, ActualCol);
            }
            else
            {
                Advance();
                Errors.Add(new CodeError(ErrorType.Unknown, (CurrentLn, ActualCol), $"{result}."));

                return new Token(TokenType.Unknown, result, CurrentLn, ActualCol);
            }
        }
        public (List<Token>, List<InterpreterError>) GetTokens()
        {
            while (CurrentChar != '\0')
            {
                if (CurrentChar == '\r' || CurrentChar == '\n') tokens.Add(GetJumpLine());

                else if (char.IsWhiteSpace(CurrentChar)) Advance();

                else if (char.IsDigit(CurrentChar)) tokens.Add(GetNumber());

                else if (CurrentChar == '"') tokens.Add(GetString());

                else if (char.IsLetter(CurrentChar)) tokens.Add(GetWord());

                else tokens.Add(GetOperatorOrDelimiter());
            }
            tokens.Add(new Token(TokenType.EndOfFile, "END", CurrentLn, CurrentCol));

            return (tokens, Errors);
        }
    }
}