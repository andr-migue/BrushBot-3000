using System.Collections.Generic;

namespace BrushBot
{
	public class Context
	{
		public int Size { get; set; }
		public Color[,] Picture { get; set; }
		public (int, int) Position { get; set; }
		public int BrushSize { get; set; }
		public Color BrushColor { get; set; }
		public bool Flag { get; set; }
		public bool Animation { get; set; }
		public bool RuntimeError { get; set; }
		public bool Print { get; set; }
		public string Message { get; set; }
		public Dictionary<string, int> Labels { get; set; }
		public Scope Scope { get; set; }
		public Context(int size)
		{
			Size = size;
			Picture = new Color[size, size];
			Position = (0, 0);
			BrushSize = 1;
			BrushColor = Color.Transparent;
			Flag = false;
			Animation = false;
			RuntimeError = false;
			Print = false;
			Message = null;
			Labels = new();
			Scope = new();
		}
		public void Reset()
        {
            RuntimeError = false;
            Message = null;
            Labels.Clear();
            Scope = new Scope();
        }
	}
}