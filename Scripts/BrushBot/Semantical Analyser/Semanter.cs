using System.Collections.Generic;
using System;

namespace BrushBot
{
    public class Semanter
    {
        List<Node> Nodes { get; set; }
        Context context { get; set; }
        List<InterpreterError> Errors {get; }
        public Semanter(List<Node> nodes, Context context)
        {
            Nodes = nodes;
            this.context = context;
            Errors = new();
        }
        public (List<Node>, List<InterpreterError>, Context) Semant()
        {
            try
            {
                if (Nodes[0] is Instruction firstLine)
                {
                    if (firstLine.Keyword.Value != "Spawn")
                    {
                        throw new CodeError(ErrorType.Invalid, firstLine.Location, $"Every code must be start with a Spawn(int x, int y)");
                    }
                }
                else
                {
                    throw new CodeError(ErrorType.Invalid, Nodes[0].Location, $"Every code must be start with a Spawn(int x, int y)");
                }
            }
            catch (InterpreterError error)
            {
                Errors.Add(error);
            }

            for (int i = 1; i < Nodes.Count; i++)
            {
                try
                {
                    if (Nodes[i] is Label label)
                    {
                        if (context.Labels.ContainsKey(label.Token.Value))
                        {
                            throw new CodeError(ErrorType.Invalid, label.Location, $"{label.Token.Value} Already exists a label with that name");
                        }
                        else
                        {
                            context.Labels.Add(label.Token.Value, i);
                        }
                    }
                }
                catch (InterpreterError error)
                {
                    Errors.Add(error);
                }
            }

            for (int i = 0; i < Nodes.Count; i++)
            {
                try
                {
                    if (Nodes[i] is Instruction instructionS && i != 0)
                    {
                        if (instructionS.Keyword.Value == "Spawn")
                        {
                            throw new CodeError(ErrorType.Invalid, instructionS.Location, $"Only one Spawn(int x, int y) for code");
                        }
                    }

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
                        instruction.CheckSemantic(context);
                    }

                    else if (Nodes[i] is Jump jump)
                    {
                        if (jump.Expression.Evaluate(context) is bool)
                        {
                            if (!context.Labels.ContainsKey(jump.Label.Value))
                            {
                                throw new CodeError(ErrorType.OutOfContext, jump.Location, $"Label {jump.Label.Value}.");
                            }
                        }
                        else throw new CodeError(ErrorType.Invalid, jump.Location, $"Expression of GoTo must be boolean.");
                    }
                }
                catch (InterpreterError error)
                {
                    Errors.Add(error);
                }
            }

            context.Scope.Variables = new();
            return (Nodes, Errors, context);
        }
    }
}