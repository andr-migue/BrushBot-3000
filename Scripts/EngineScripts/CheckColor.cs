using BrushBot;
public static class CheckColor
{
	public static Godot.Color GetColor(BrushBot.Color color)
	{
		if (color.Predef != null)
		{
			switch (color.Predef)
			{
				case PredefColor.Red: return new Godot.Color(1, 0, 0);
				case PredefColor.Blue: return new Godot.Color(0, 0, 1);
				case PredefColor.Green: return new Godot.Color(0, 1, 0);
				case PredefColor.Yellow: return new Godot.Color(1, 1, 0);
				case PredefColor.Orange: return new Godot.Color(1, 0.647f, 0);
				case PredefColor.Purple: return new Godot.Color(0.627f, 0.125f, 0.941f);
				case PredefColor.Black: return new Godot.Color(0, 0, 0);
				case PredefColor.White: return new Godot.Color(1, 1, 1);
				case PredefColor.Pink: return new Godot.Color(1, 0.314f, 0.863f);

				default: return new Godot.Color(1, 1, 1, 0);
			}
		}
		else
		{
			return new Godot.Color((float)color.R, (float)color.G, (float)color.B, (float)color.A);
		}
	}
}