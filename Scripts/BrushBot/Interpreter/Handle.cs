using System;
using System.Drawing;
using System.Linq.Expressions;

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
        White
    }
    public static class Handle
    {
        public static int size = 64;
        public static void Spawn(Expression expression)
        {
            if (expression.Interpret() is (int x, int y))
            {
                if (IsValid(x, y))
                {
                    Interpreter.Position = (x, y);
                }
            }
            else throw new RuntimeError ($"Coordenadas de Spawn fuera de rango.");
        }
        public static void Color(Expression expression)
        {
            if (expression.Interpret() is Color color) Interpreter.BrushColor = color;
            else throw new RuntimeError ($"Argumento de Color no válido.");
        }
        public static void Size(Expression expression)
        {
            if (expression.Interpret() is int size)
            {
                if (size % 2 == 0)
                {
                    Interpreter.BrushSize = size - 1;
                }
                else Interpreter.BrushSize = size;
            }
            else throw new RuntimeError ($"Argumento de Size no valido.");
        }
        public static void DrawLine(Expression expression)
        {
            int x = Interpreter.Position.Item1;
            int y = Interpreter.Position.Item2;

            if (expression.Interpret() is (int dirX, (int dirY, int distance)))
            {
                for (int d = 0; d < distance; d++)
                {
                    int newx = x + dirX;
                    int newy = y + dirY;

                    if (IsValid(newx, newy))
                    {
                        Interpreter.Picture[newx, newy] = Interpreter.BrushColor;
                        x = newx; 
                        y = newy;
                    }
                    else throw new RuntimeError($"Coordenadas de DrawLine fuera de rango: ({newx}, {newy})");
                }
            }
            else
                throw new RuntimeError("Argumento de DrawLine no válido.");
        }
        public static void DrawCircle(Expression expression)
        {
            int x = Interpreter.Position.Item1;
            int y = Interpreter.Position.Item2;

            if (expression.Interpret() is (int dirX, (int dirY, int radius)))
            {
                int centerX = x + dirX;
                int centerY = y + dirY;

                if (!IsValid(centerX, centerY)) throw new RuntimeError($"Centro del círculo fuera de rango: ({centerX}, {centerY})");
                
                Interpreter.Position = (centerX, centerY);

                double step = Math.Max(1, 360.0 / (2 * Math.PI * radius));

                for (double a = 0; a < 360; a += step)
                {
                    double radians = a * Math.PI / 180;
                    int pixelX = (int)Math.Round(centerX + radius * Math.Cos(radians));
                    int pixelY = (int)Math.Round(centerY + radius * Math.Sin(radians));

                    if (IsValid(pixelX, pixelY))
                    {
                        Interpreter.Picture[pixelX, pixelY] = Interpreter.BrushColor;
                    }
                }
            }
            else throw new RuntimeError("Argumento de DrawCircle no válido.");
        }
        public static void DrawRectangle(Expression expression)
        {
            int x = Interpreter.Position.Item1;
            int y = Interpreter.Position.Item2;

            if (expression.Interpret() is (int dirX, (int dirY, (int distance, (int width, int height)))))
            {
                int newx = x + dirX * distance;
                int newy = y + dirY * distance;

                if (!IsValid(newx, newy)) throw new RuntimeError($"Centro del rectángulo fuera de rango: ({newx}, {newy})");
                else
                {
                    int topLeftX = newx - width / 2;
                    int topLeftY = newy - height / 2;
                    int bottomRightX = topLeftX + width;
                    int bottomRightY = topLeftY + height;
                    for (int i = topLeftX; i < bottomRightX; i++)
                    {
                        for (int j = topLeftY; j < bottomRightY; j++)
                        {
                            bool isBorder = (i == topLeftX) || (i == bottomRightX - 1) || (j == topLeftY) || (j == bottomRightY - 1);
                            if (isBorder && IsValid(i, j))
                            {
                                Interpreter.Picture[i, j] = Interpreter.BrushColor;
                            }
                        }
                    }
                    Interpreter.Position = (newx, newy);
                }
            }
            else
            {
                throw new RuntimeError("Argumentos de DrawRectangle no válidos.");
            }
        }
        public static void Fill()
        {
            int x = Interpreter.Position.Item1;
            int y = Interpreter.Position.Item2;
            throw new NotImplementedException();
        }
        public static int GetActualX()
        {
            return Interpreter.Position.Item1;
        }
        public static int GetActualY()
        {
            return Interpreter.Position.Item2;
        }
        public static int GetCanvasSize()
        {
            return size;
        }
        public static bool IsBrushColor(Expression expression)
        {
            if (expression.Interpret() is Color color)
            {
                return color == Interpreter.BrushColor;
            }
            else throw new RuntimeError ($"Argumento de IsBrushColor no valido.");
        }
        public static bool IsBrushSize(Expression expression)
        {
            if (expression.Interpret() is int brush)
            {
                return brush == Interpreter.BrushSize;
            }
            else throw new RuntimeError ($"Argumento de IsBrushSize no valido.");
        }
        private static bool IsValid(int x, int y)
        {
            return x >= 0 && x < size && y >= 0 && y < size;
        }
    }
}