using System;
using System.Text;

namespace BrushBot
{
    public class AstPrinter
    {
        public string Print(Node node)
        {
            var sb = new StringBuilder();
            ConstruirArbol(node, "", true, sb);
            return sb.ToString();
        }

        private void ConstruirArbol(Node expr, string prefijo, bool esUltimo, StringBuilder sb)
        {
            string simboloRama = esUltimo ? "└── " : "├── ";
            string nuevoPrefijo = esUltimo ? "    " : "│   ";

            sb.Append(prefijo).Append(simboloRama);

            switch (expr)
            {
                // Nuevos nodos del árbol
                case Assignment a:
                    sb.AppendLine($"Asignación: {a.Variable.Value} {a.Operator.Value}");
                    ConstruirArbol(a.Expression, prefijo + nuevoPrefijo, true, sb);
                    break;
                    
                case Jump j:
                    sb.AppendLine($"GoTo {j.Label.Value} if");
                    ConstruirArbol(j.Expression, prefijo + nuevoPrefijo, true, sb);
                    break;
                    
                case Instruction i:
                    sb.AppendLine($"Instrucción: {i.Keyword.Value}");
                    ConstruirArbol(i.Expression, prefijo + nuevoPrefijo, true, sb);
                    break;
                    
                case Literal l:
                    sb.AppendLine($"Literal: {l.Expression.Value}");
                    break;
                    
                case UnaryExpression u:
                    sb.AppendLine($"Operador Unario: {u.Operator.Value}");
                    ConstruirArbol(u.Expression, prefijo + nuevoPrefijo, true, sb);
                    break;
                    
                case BinaryExpression b:
                    sb.AppendLine($"Operador Binario: {b.Operator.Value}");
                    ConstruirArbol(b.Left, prefijo + nuevoPrefijo, false, sb);
                    ConstruirArbol(b.Right, prefijo + nuevoPrefijo, true, sb);
                    break;
                    
                case GroupingExpression g:
                    sb.AppendLine($"Expresión de Agrupación: ,");
                    ConstruirArbol(g.Left, prefijo + nuevoPrefijo, false, sb);
                    ConstruirArbol(g.Right, prefijo + nuevoPrefijo, true, sb);
                    break;
                    
                case Label label:
                    sb.AppendLine($"Label: {label.Token.Value}");
                    break;
                case Function function:
                    sb.AppendLine($"Función: {function.Token.Value}");
                    if (function.Value != null)
                    {
                        ConstruirArbol(function.Value, prefijo + nuevoPrefijo, true, sb);
                    }
                    break;
                default:
                    sb.AppendLine("Nodo no reconocido");
                    break;
            }
        }
    }
}
