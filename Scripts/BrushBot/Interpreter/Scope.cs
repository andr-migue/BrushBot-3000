using System;
using System.Collections.Generic;

namespace BrushBot
{
    public enum Color
    {
        Transparent,
        Red,
        Blue,
        Green,
        Yellow,
        Orange,
        Purple,
        Black,
        White,
        Pink
    }
    public static class Scope
    {
        public static Dictionary<string, int> Labels;
        public static Dictionary<string, Object> Variables;
        public static int Size;
        public static Color[,] Picture;
        public static (int, int) Position;
        public static int BrushSize;
        public static Color BrushColor;
        public static bool flag;
        public static bool animation;
        public static void Init()
        {
            Replay();
            Size = 64;
            Picture = new Color[Size, Size];
            Position = (0, 0);
            BrushSize = 1;
            BrushColor = Color.Transparent;
            flag = false;
            animation = false;
        }
        public static void Replay()
        {
            Labels = new();
            Variables = new();
        }
    }
}