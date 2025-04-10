using System;
using System.Collections.Generic;
namespace BrushBot
{
    public static class Interpreter
    {
        public static List<Node> Nodes;
        public static List<RuntimeError> Errors;
        public static Dictionary<string, int> Labels;
        public static Dictionary<string, Object> Variables;
        public static int Size = 64;
        public static Color[,] Picture = new Color[Size, Size];
        public static (int, int) Position = (0, 0);
        public static int BrushSize = 1;
        public static Color BrushColor = Color.Transparent;
        public static void Interpret()
        {
            try
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (Nodes[i] is Label label) 
                    {
                        Labels.Add(label.Token.Value, label.Token.Ln);
                    }
                    else continue;
                }
                for (int i = 0; i < Nodes.Count; i++)
                {
                    
                    if (Nodes[i] is Instruction instruction)
                    {
                        instruction.Execute();
                    }
                    else if (Nodes[i] is Assignment assignment)
                    {
                        (string Name, Object Value) = assignment.Assign();
                        if (Variables.ContainsKey(Name))
                        {
                            Variables[Name] = Value;
                        }
                        else Variables.Add(Name, Value);
                    }
                    else if (Nodes[i] is Jump jump)
                    {
                        if (jump.Expression.Interpret() is true)
                        {
                            if (Labels.ContainsKey(jump.Label.Value))
                            {
                                i = Labels[jump.Label.Value];
                            }
                            else throw new RuntimeError ($"Label: {jump.Label.Value} no existe en este contexto.");
                        }
                        else continue;
                    }
                    else if (Nodes[i] is Label label) continue;
                    else throw new RuntimeError ($"Sentencia no válida.");
                }
            }
            catch (RuntimeError error)
            {
                Errors.Add(error);
            }
        }
        public static void Init(List<Node> nodes)
        {
            Nodes = nodes;
            Labels = new();
            Variables = new();
            Errors = new();

        }
    }
}