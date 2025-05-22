using System;
using System.Collections.Generic;

namespace BrushBot
{
	public class Function : Expression
    {
        public Token Name {get; }
        public List<Expression> Parameters {get; }
        public Function ((int, int) location, Token name, List<Expression> parameters) : base (location)
        {
            Name = name;
            Parameters = parameters;
        }
        public override Object Evaluate(Context context)
        {
            switch (Name.Value)
            {
                case "GetActualX": return Handle.GetActualX(Parameters, context);
                case "GetActualY": return Handle.GetActualY(Parameters, context);
                case "GetCanvasSize": return Handle.GetCanvasSize(Parameters, context);
                case "IsBrushColor": return Handle.IsBrushColor(Parameters, context);
                case "IsBrushSize": return Handle.IsBrushSize(Parameters, context);
                case "IsCanvasColor": return Handle.IsCanvasColor(Parameters, context);
                case "GetColorCount": return Handle.GetColorCount(Parameters, context);
                default: throw new CodeError(ErrorType.Invalid, Location, $"{Name.Value}");
            }
        }
    }
}