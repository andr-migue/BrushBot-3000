using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BrushBot
{
    public class Interpreter
    {
        List<Node> Nodes;
        Dictionary<string, int> visitLabels;
        const int max = 3000;
        public Interpreter(List<Node> nodes)
        {
            Nodes = nodes;
            visitLabels = new();
        }
        public async Task Evaluate()
        {
            try
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (Nodes[i] is Assignment assignment)
                    {
                        (string Name, Object Value) = assignment.Assign();
                        if (Context.Variables.ContainsKey(Name))
                        {
                            Context.Variables[Name] = Value;
                        }
                        else Context.Variables.Add(Name, Value);
                    }
                    else if (Nodes[i] is Instruction instruction)
                    {
                        await instruction.Execute();
                    }
                    else if (Nodes[i] is Jump jump)
                    {
                        if (jump.Expression.Evaluate() is true)
                        {
                            string label = jump.Label.Value;

                            if (Context.Labels.ContainsKey(label))
                            {
                                if (!visitLabels.ContainsKey(label))
                                {
                                    visitLabels[label] = 0;
                                }
                                visitLabels[label]++;

                                if (visitLabels[label] > max)
                                {
                                    throw new CodeError(ErrorType.StackOverflow, jump.Location, $"Label {label} has been visited more than {max} times");
                                }

                                i = Context.Labels[jump.Label.Value];
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
                Context.possibleRuntimeError = error;
                Context.runtimeError = true;
            }
        }
    }
}