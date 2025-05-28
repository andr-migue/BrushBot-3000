namespace BrushBot
{
    public enum TokenType
    {
        Keyword,
        Function,
        Identifier,
        Number,
        Color,
        String,
        Boolean,
        Operator,
        Delimiter,
        JumpLine,
        Unknown,
        EndOfFile
    }
    public class Token
    {
        public TokenType Type {get; private set; }
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
            return $"[{Type}] [{Value}] [Ln {Ln}] [Col {Col}]";
        }
    }
}