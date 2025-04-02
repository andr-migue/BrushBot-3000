using System.Collections.Generic;
namespace Interpreter
{
    public enum TokenType
    {
        Instruction,
        Identifier,
        Number,
        Color,
        Operator,
        BiOperator,
        Delimiter,
        JumpLine,
        Unknown,
        EndOfFile
    }
    public class Token
    {
        public TokenType Type {get; set; }
        public string Value {get; set; }
        public int Ln {get; set; }
        public int Col {get; set; }
        public Token(TokenType type, string value, int ln, int col)
        {
            Type = type;
            Value = value;
            Ln = ln;
            Col = col;
        }
        public override string ToString()
        {
            return $"Token ({Type}, Value: {Value}, Ln: {Ln}, Col: {Col})";
        }
    }
    public class Lexer
    {
        private string Text;
        private int Position;
        private char CurrentChar;
        public int CurrentLn { get; private set; } = 1;
        public int CurrentCol { get; private set; } = 0;
        public Lexer(string text)
        {
            Text = text;
            Position = 0;
            CurrentChar = Text.Length > 0 ? Text[0] : '\0';
        }
        private HashSet<string> Instructions = new HashSet<string>
        {
            "Spawn", "Color", "Size", "Fill", "GoTo", "DrawLine", "DrawCircle", "DrawRectangle",
            "GetActualX", "GetActualY", "GetCanvasSize", "GetColorCount", "IsBrushColor", "IsBrushSize",
            "IsCanvasColor", "IsColor"
        };
        private HashSet<string> Operators = new HashSet<string>
        {
            "+", "-", "*", "/", "=", "<", ">", "%"
        };
        private HashSet<string> BiOperators = new HashSet<string>
        {
            "**", "<=", ">=", "<-", "=="
        };
        private HashSet<string> Delimiters = new HashSet<string>
        {
            "(", ")", "[", "]", ",",
        };
        private HashSet<string> Colors = new HashSet<string>
        {
            "Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Black", "White", "Transparent"
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
                return new Token(TokenType.JumpLine, "JumpLine", ActualLn, ActualCol);
            }
            else // Mac o Unix
            {
                Advance();
                CurrentCol = 0;
                return new Token(TokenType.JumpLine, "JumpLine", ActualLn, ActualCol);
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
                return new Token(TokenType.Unknown, result, CurrentLn, CurrentCol);
            }
            Advance(); // Para saltar la comilla de cierre.
            
            if (Colors.Contains(result)) return new Token(TokenType.Color, result, CurrentLn, CurrentCol);
            else return new Token(TokenType.Unknown, result, CurrentLn, CurrentCol);
        }
        private Token GetInstruction()
        {
            string result = "";
            while (CurrentChar != '\0' && (char.IsLetterOrDigit(CurrentChar) || CurrentChar == '_' || CurrentChar == '-'))
            {
                result += CurrentChar;
                Advance();
            }

            if (Instructions.Contains(result)) return new Token(TokenType.Instruction, result, CurrentLn, CurrentCol);
            else return new Token(TokenType.Identifier, result, CurrentLn, CurrentCol);
        }
        private Token GetOperatorOrDelimiter()
        {
            string result = "";
            result += CurrentChar;
            if (Operators.Contains(result))
            {
                string next = result + Peek();

                if (BiOperators.Contains(next)){
                    Advance();
                    Advance();
                    return new Token(TokenType.BiOperator, next, CurrentLn, CurrentCol);
                }
                else
                {
                    Advance();
                    return new Token(TokenType.Operator, result, CurrentLn, CurrentCol);
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
                return new Token(TokenType.Unknown, result, CurrentLn, CurrentCol);
            }
        }
        public Token GetNextToken()
        {
            while (CurrentChar != '\0')
            {
                if (CurrentChar == '\r' || CurrentChar == '\n') return GetJumpLine();
                
                else if (char.IsWhiteSpace(CurrentChar)) Advance();
                
                else if (char.IsDigit(CurrentChar)) return GetNumber();
                
                else if (CurrentChar == '"') return GetColor();
                
                else if (char.IsLetter(CurrentChar)) return GetInstruction();
                
                else return GetOperatorOrDelimiter();
            }
            return new Token(TokenType.EndOfFile, "EndOfFile", CurrentLn, CurrentCol);
        }
    }
}