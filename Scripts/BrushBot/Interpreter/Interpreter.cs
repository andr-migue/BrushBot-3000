using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BrushBot
{
    public class Interpreter
    {
        List<Node> Nodes;
        List<RuntimeError> Errors;
        public Interpreter (List<Node> nodes)
        {
            Nodes = nodes;
            Errors = new();
        }
        public async Task Interpret()
        {
            try
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (Nodes[i] is Assignment assignment)
                    {
                        (string Name, Object Value) = assignment.Assign();
                        if (Scope.Variables.ContainsKey(Name))
                        {
                            Scope.Variables[Name] = Value;
                        }
                        else Scope.Variables.Add(Name, Value);
                    }
                    else if (Nodes[i] is Instruction instruction)
                    {
                        await instruction.Execute();
                    }
                    else if (Nodes[i] is Jump jump)
                    {
                        if (jump.Expression.Interpret() is true)
                        {
                            if (Scope.Labels.ContainsKey(jump.Label.Value))
                            {
                                i = Scope.Labels[jump.Label.Value];
                            }
                        }
                        else continue;
                    }
                    else if (Nodes[i] is Label) continue;
                    else throw new RuntimeError ($"Sentencia no válida.");
                }
            }
            catch (RuntimeError error)
            {
                Errors.Add(error);
            }
        }
    }
}