using Godot;
using System;

public partial class GridGenerator : TextureRect
{
    Color GridColor = new Color(0, 0, 0, 0.1f);
    [Export] float LineWidth = 1.0f;
    public override void _Draw()
    {
        int size = GlobalData.Size;

        float space = Size.X / size;

        for (int i = 1; i < size; i++)
        {
            float c = i * space;
            DrawLine(new Vector2(0, c), new Vector2(Size.X, c), GridColor, LineWidth);
            DrawLine(new Vector2(c, 0), new Vector2(c, Size.Y), GridColor, LineWidth);
        }
    }
}
