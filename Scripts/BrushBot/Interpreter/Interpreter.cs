using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BrushBot
{
    public class Interpreter
    {
        List<Node> Nodes;
        public Interpreter(List<Node> nodes)
        {
            Nodes = nodes;
        }
        public async Task Interpret(Context context)
        {
            try
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (Nodes[i] is Assignment assignment)
                    {
                        (string Name, Object Value) = assignment.Assign(context);
                        if (context.Scope.Variables.ContainsKey(Name))
                        {
                            context.Scope.Variables[Name] = Value;
                        }
                        else context.Scope.Variables.Add(Name, Value);
                    }

                    else if (Nodes[i] is Instruction instruction)
                    {
                        await instruction.Execute(context);
                    }

                    else if (Nodes[i] is Jump jump)
                    {
                        if (jump.Expression.Evaluate(context) is true)
                        {
                            string label = jump.Label.Value;

                            if (context.Labels.ContainsKey(label))
                            {
                                i = context.Labels[jump.Label.Value];
                            }
                        }
                        else continue;
                    }

                    else if (Nodes[i] is Label) continue;

                    else throw new CodeError(ErrorType.Invalid, Nodes[i].Location, $"Sentence.");
                }
            }
            catch (InterpreterError error)
            {
                context.Message = error.Message;
                context.RuntimeError = true;
            }
        }
    }
}