using Godot;
using System;

public partial class GlobalData : Node
{
    public static int Rows;
    public static int Collumns;
    public override void _Ready()
    {
        Rows = 64;
        Collumns = 64;
    }

}
