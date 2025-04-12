using System;
namespace BrushBot
{
    public class LexerError : Exception
    {
        public LexerError (string message) : base(message) {}
    }
    public class ParserError : Exception
    {
        public ParserError (string message) : base (message) {}
    }
    public class SemanticalError : Exception
    {
        public SemanticalError (string message) : base (message) {}
    }
    public class RuntimeError : Exception
    {
        public RuntimeError (string message) : base (message) {}
    }
}