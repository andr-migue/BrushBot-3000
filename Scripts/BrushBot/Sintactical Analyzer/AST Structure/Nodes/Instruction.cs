using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrushBot
{
	public class Instruction : Node
    {
        public Token Keyword {get; }
        public List<Expression> Parameters {get; }
        public Instruction ((int, int) location, Token keyword, List<Expression> parameters) : base(location)
        {
            Keyword = keyword;
            Parameters = parameters;
        }
        public void CheckSemantic(Context context)
        {
            switch (Keyword.Value)
            {
                case "Spawn" :
                    Handle.CheckSpawn(Parameters, context);
                    break;
                case "ReSpawn" :
                    Handle.CheckSpawn(Parameters, context);
                    break;
                case "Color":
                    Handle.CheckColor(Parameters, context);
                    break;
                case "Size" :
                    Handle.CheckSize(Parameters, context);
                    break;
                case "DrawLine" :
                    Handle.CheckDrawLine(Parameters, context);
                    break;
                case "DrawCircle" :
                    Handle.CheckDrawCircle(Parameters, context);
                    break;
                case "DrawRectangle" :
                    Handle.CheckDrawRectangle(Parameters, context);
                    break;
                case "Fill" :
                    Handle.CheckFill(Parameters, context);
                    break;
                default: throw new CodeError(ErrorType.Invalid, Location, $"{Keyword.Value}");
            }
        }
        public async Task Execute(Context context)
        {
            switch (Keyword.Value)
            {
                case "Spawn" :
                    await Handle.Spawn(Parameters, context);
                    break;
                case "ReSpawn" :
                    await Handle.Spawn(Parameters, context);
                    break;
                case "Color":
                    await Handle.Color(Parameters, context);
                    break;
                case "Size" :
                    await Handle.Size(Parameters, context);
                    break;
                case "DrawLine" :
                    await Handle.DrawLine(Parameters, context);
                    break;
                case "DrawCircle" :
                    await Handle.DrawCircle(Parameters, context);
                    break;
                case "DrawRectangle" :
                    await Handle.DrawRectangle(Parameters, context);
                    break;
                case "Fill" :
                    await Handle.Fill(context);
                    break;
                default: throw new CodeError(ErrorType.Invalid, Location, $"{Keyword.Value}");;
            }
        }
    }
}