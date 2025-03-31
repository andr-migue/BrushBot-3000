using Godot;
using System;

public partial class GridGenerator : TextureRect
{
    Color GridColor = new Color(0, 0, 0, 0.2f);
    [Export] float LineWidth = 1.0f;
    public override void _Draw()
    {
        int Rows = GlobalData.Rows;
        int Collumns = GlobalData.Collumns;

        float rowSpace = Size.Y / Rows;
        float collumnsSpace = Size.X / Collumns;

        for (int i = 1; i < Rows; i++)
        {
            float y = i * rowSpace;
            DrawLine(new Vector2(0, y), new Vector2(Size.X, y), GridColor, LineWidth);
        }

        for (int j = 1; j < Collumns; j++)
        {
            float x = j * collumnsSpace;
            DrawLine(new Vector2(x, 0), new Vector2(x, Size.Y), GridColor, LineWidth);
        }
    }
}
