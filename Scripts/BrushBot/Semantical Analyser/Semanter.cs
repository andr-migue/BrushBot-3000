using System.Collections.Generic;
using System;
namespace BrushBot
{
    public class Semanter
    {
        List<Node> Nodes {get; }
        List<InterpreterError> Errors {get; }
        public Semanter (List<Node> nodes)
        {
            Nodes = nodes;
            Errors = new();
        }
        public (List<Node>, List<InterpreterError>) Semant()
        {
            try
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (Nodes[i] is Label label) 
                    {
                        if (Scope.Labels.ContainsKey(label.Token.Value))
                        {
                            throw new CodeError(ErrorType.Invalid, label.Location,$"Ya exite un label con este nombre {label.Token.Value}");
                        }
                        else Scope.Labels.Add(label.Token.Value, i);
                    }
                    else continue;
                }
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
                        instruction.CheckSemantic();
                    }
                    else if (Nodes[i] is Jump jump)
                    {
                        if (jump.Expression.Evaluate() is bool)
                        {
                            if (!Scope.Labels.ContainsKey(jump.Label.Value)) throw new CodeError (ErrorType.OutOfContext, jump.Location, $"Label [{jump.Label.Value}] no existe en este contexto.");
                            continue;
                        }
                        else throw new CodeError (ErrorType.Invalid, jump.Location,$"Expresión de GoTo debe ser booleana.");
                    }
                }
            }
            catch (InterpreterError error)
            {
                Errors.Add(error);
            }
            return (Nodes, Errors);
        }
    }
}