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
        private int CurrentCol = 0;
        private List<CodeError> Errors;
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
            "Spawn", "Color", "Size", "Fill", "GoTo", "DrawLine", "DrawCircle", "DrawRectangle"
        };
        private HashSet<string> Functions = new HashSet<string>
        {
            "GetActualX", "GetActualY", "GetCanvasSize", "GetColorCount", "IsBrushColor", "IsBrushSize",
            "IsCanvasColor"
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
                CurrentCol = 0;
                return new Token(TokenType.JumpLine, " ", ActualLn, ActualCol);
            }
            else // Mac o Unix
            {
                Advance();
                CurrentCol = 0;
                return new Token(TokenType.JumpLine, " ", ActualLn, ActualCol);
            }
        }
        private Token GetNumber()
        {
            string result = "";
            while (CurrentChar != '\0' && char.IsDigit(CurrentChar))
            {
                result += CurrentChar;
                Advance();
            }
            return new Token(TokenType.Number, result, CurrentLn, CurrentCol);
        }
        private Token GetColor()
        {
            Advance(); // Para saltar la comilla de apertura.
            string result = "";
            while (CurrentChar != '\0' && CurrentChar != '"' && char.IsLetter(CurrentChar))
            {
                result += CurrentChar;
                Advance();
            }
            if (CurrentChar != '"')
            {
                Errors.Add(new CodeError(ErrorType.Expected, (CurrentLn, CurrentCol), $"Se espera '{"\""}'."));

                return new Token(TokenType.Unknown, result, CurrentLn, CurrentCol);
            }
            Advance(); // Para saltar la comilla de cierre.
            
            if (Colors.Contains(result)) return new Token(TokenType.Color, result, CurrentLn, CurrentCol);
            else
            {
                Errors.Add(new CodeError(ErrorType.Unknown, (CurrentLn, CurrentCol), $"Color desconocido '{result}'."));

                return new Token(TokenType.Unknown, result, CurrentLn, CurrentCol);
            }
        }
        private Token GetWord()
        {
            string result = "";

            while (CurrentChar != '\0' && (char.IsLetterOrDigit(CurrentChar) || CurrentChar == '_' || CurrentChar == '-'))
            {
                result += CurrentChar;
                Advance();
            }

            if (Keywords.Contains(result))
            {
                return new Token(TokenType.Keyword, result, CurrentLn, CurrentCol);
            }

            else if (Functions.Contains(result))
            {
                return new Token(TokenType.Function, result, CurrentLn, CurrentCol);
            }

            else return new Token(TokenType.Identifier, result, CurrentLn, CurrentCol);
        }
        private Token GetOperatorOrDelimiter()
        {
            string result = "";
            result += CurrentChar;
            if (Operators.Contains(result) || result == "=")
            {
                string next = result + Peek();

                if (Operators.Contains(next)){
                    Advance();
                    Advance();
                    return new Token(TokenType.Operator, next, CurrentLn, CurrentCol);
                }
                else if (Operators.Contains(result))
                {
                    Advance();
                    return new Token(TokenType.Operator, result, CurrentLn, CurrentCol);
                }
                else
                {
                    Advance();
                    Errors.Add(new CodeError(ErrorType.Unknown, (CurrentLn, CurrentCol), $"Identificador desconocido '{result}'."));

                    return new Token(TokenType.Unknown, result, CurrentLn, CurrentCol);
                }
            }
            else if (Delimiters.Contains(result))
            {
                Advance();
                return new Token(TokenType.Delimiter, result, CurrentLn, CurrentCol);
            }
            else
            {
                Advance();
                Errors.Add(new CodeError(ErrorType.Unknown, (CurrentLn, CurrentCol), $"Identificador desconocido '{result}'."));

                return new Token(TokenType.Unknown, result, CurrentLn, CurrentCol);
            }
        }
        public (List<Token>, List<CodeError>) GetTokens()
        {
            while (CurrentChar != '\0')
            {
                if (CurrentChar == '\r' || CurrentChar == '\n') tokens.Add(GetJumpLine());
                
                else if (char.IsWhiteSpace(CurrentChar)) Advance();
                
                else if (char.IsDigit(CurrentChar)) tokens.Add(GetNumber());
                
                else if (CurrentChar == '"') tokens.Add(GetColor());
                
                else if (char.IsLetter(CurrentChar)) tokens.Add(GetWord());
                
                else tokens.Add(GetOperatorOrDelimiter());
            }
            tokens.Add(new Token(TokenType.EndOfFile, "END", CurrentLn, CurrentCol));

            return (tokens, Errors);
        }
    }
}