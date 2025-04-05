using System.Text;
namespace Interpreter
{
    public class AstPrinter
    {
        public string Print(Expression expression)
        {
            return PrintNode(expression, "", true);
        }

        private string PrintNode(Expression node, string prefix, bool isLast)
        {
            var sb = new StringBuilder();
            string currentConnector = isLast ? "└── " : "├── ";
            string nextPrefix = prefix + (isLast ? "    " : "│   ");

            switch (node)
            {
                case BinaryExpression binary:
                    sb.AppendLine($"{prefix}{currentConnector}{binary.Operator.Value} (Binary: Logical = {binary.IsLogical})");
                    sb.Append(PrintNode(binary.Left, nextPrefix, false));
                    sb.Append(PrintNode(binary.Right, nextPrefix, true));
                    break;
                    
                case UnaryExpression unary:
                    string unaryType = unary.IsLogical ? "Logical" : "Arithmetic";
                    sb.AppendLine($"{prefix}{currentConnector}{unary.Operator.Value} (Unary: {unaryType})");
                    sb.Append(PrintNode(unary.Expression, nextPrefix, true));
                    break;
                    
                case GroupingExpression grouping:
                    sb.AppendLine($"{prefix}{currentConnector}() (Group)");
                    sb.Append(PrintNode(grouping.Expression, nextPrefix, true));
                    break;
                    
                case Literal literal:
                    sb.AppendLine($"{prefix}{currentConnector}{literal.Type}: {literal.Value}");
                    break;
                    
                default:
                    sb.AppendLine($"{prefix}{currentConnector}???");
                    break;
            }
            
            return sb.ToString();
        }
    }
}