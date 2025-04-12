using BrushBot;
using Godot;
using System;
using System.Data;

public partial class GridGenerator : TextureRect
{
    Godot.Color GridColor = new Godot.Color(0, 0, 0, 0.1f);
    [Export] float LineWidth = 1.0f;
    [Export] AnimatedSprite2D Brush;
    
    public override void _Ready(){}

    public override void _Draw()
    {
        int size = Scope.Size;
        float space = Size.X / size;
        for (int i = 1; i < size; i++)
        {
            float c = i * space;
            DrawLine(new Vector2(0, c), new Vector2(Size.X, c), GridColor, LineWidth);
            DrawLine(new Vector2(c, 0), new Vector2(c, Size.Y), GridColor, LineWidth);
        }
        DrawColor(size, space);
    }

    private void DrawColor(int size, float space)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Godot.Color color = CheckColor(Scope.Picture[i, j]);
                Rect2 rect = new Rect2(i * space, j * space, space, space);
                DrawRect(rect, color);
            }
        }
    }
    public Godot.Color CheckColor(BrushBot.Color color)
    {
        switch (color)
        {
            case  BrushBot.Color.Transparent: return new Godot.Color(255, 255, 255, 0);
            case  BrushBot.Color.Red: return new Godot.Color(255, 0, 0);
            case  BrushBot.Color.Blue: return new Godot.Color(0, 0, 255);
            case  BrushBot.Color.Green: return new Godot.Color(0, 255, 0);
            case  BrushBot.Color.Yellow: return new Godot.Color(255, 255, 0);
            case  BrushBot.Color.Orange: return new Godot.Color(255, 165, 0);
            case  BrushBot.Color.Purple: return new Godot.Color(160, 32, 240);
            case  BrushBot.Color.Black: return new Godot.Color(0, 0, 0);
            case  BrushBot.Color.White: return new Godot.Color(255, 255, 255);
            case  BrushBot.Color.Pink : return new Godot.Color(255, 80, 220);

            default: return new Godot.Color(255, 255, 255, 0);
        }
    }
}
