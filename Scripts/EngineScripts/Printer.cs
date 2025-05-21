using BrushBot;
using Godot;
public partial class Printer : TextureRect
{
    Godot.Color GridColor = new Godot.Color(0, 0, 0, 0.1f);
    [Export] float LineWidth = 1.0f;
    [Export] AnimatedSprite2D Brush;
    public override void _PhysicsProcess(double delta)
    {
        if (Context.animation == true) Brush.Play("move");
        
        else Brush.Play("default");
    }
    public override void _Draw()
    {
        int size = Context.Size;
        float space = Size.X / size;
        
        DrawColor(size, space);
        UpdateBrushPosition(space);
        for (int i = 1; i < size; i++)
        {
            float c = i * space;
            DrawLine(new Vector2(0, c), new Vector2(Size.X, c), GridColor, LineWidth);
            DrawLine(new Vector2(c, 0), new Vector2(c, Size.Y), GridColor, LineWidth);
        }
    }
    private void DrawColor(int size, float space)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Godot.Color color = Handle.CheckColor(Context.Picture[i, j]);
                Rect2 rect = new Rect2(i * space, j * space, space, space);
                DrawRect(rect, color);
            }
        }
    }
    private void UpdateBrushPosition(float space)
    {
        (int x, int y) = Context.Position;
        Vector2 currentPosition = new Vector2(x, y);

        float X = currentPosition.X * space + 20;
        float Y = currentPosition.Y * space - 20;

        Brush.Position = new Vector2(X, Y);
    }
}