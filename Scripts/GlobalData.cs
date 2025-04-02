using Godot;
using System;

public partial class GlobalData : Node
{
    public static int Size;
    public override void _Ready()
    {
        Size = 64;
    }

}
