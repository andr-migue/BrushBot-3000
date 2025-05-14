using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BrushBot
{
    public static class Handle
    {
        public static int delay = 30;
        public static void CheckSpawn(Expression expression)
        {
            if (!(expression.Interpret() is (int, int)))
            {
                throw new SemanticalError ($"Error: Argumento de Spawn no valido.");
            }
        }
        public static void CheckColor(Expression expression)
        {
            if (!(expression.Interpret() is Color))
            {
                throw new SemanticalError ($"Error: Argumento de Color no valido.");
            }
        }
        public static void CheckSize(Expression expression)
        {
            if (!(expression.Interpret() is int))
            {
                throw new SemanticalError ($"Error: Argumento de Size no valido.");
            }
        }
        public static void CheckDrawLine(Expression expression)
        {
            if (!(expression.Interpret() is (int, (int, int))))
            {
                throw new SemanticalError ($"Error: Argumento de DrawLine no valido.");
            }
        }
        public static void CheckDrawCircle(Expression expression)
        {
            if (!(expression.Interpret() is (int, (int, int))))
            {
                throw new SemanticalError ($"Error: Argumento de DrawCircle no valido.");
            }
        }
        public static void CheckDrawRectangle(Expression expression)
        {
            if (!(expression.Interpret() is (int, (int, (int, (int, int))))))
            {
                throw new SemanticalError ($"Error: Argumento de DrawRectangle no valido.");
            }
        }
        public static void CheckFill(Expression expression)
        {
            if (expression != null)
            {
                throw new SemanticalError ($"Error: Argumento de Fill no valido.");
            }
        }
        public static async Task Spawn(Expression expression)
        {
            if (expression.Interpret() is (int x, int y))
            {
                if (IsValid(x, y))
                {
                    Scope.Position = (x, y);
                    Scope.flag = true;
                    await Task.Delay(0);
                }
                else throw new RuntimeError ($"Coordenadas fuera de rango");
            }
        }
        public static async Task Color(Expression expression)
        {
            if (expression.Interpret() is Color color)
            {
               Scope.BrushColor = color;
               Scope.flag = true;
                await Task.Delay(0);
            }
        }
        public static async Task Size(Expression expression)
        {
            if (expression.Interpret() is int size)
            {
                if (size % 2 == 0)
                {
                    Scope.BrushSize = size - 1;
                    Scope.flag = true;
                    await Task.Delay(0);
                }
                else Scope.BrushSize = size;
            }
        }
        public static async Task DrawLine(Expression expression)
        {
            int x = Scope.Position.Item1;
            int y = Scope.Position.Item2;
            Scope.Picture[x, y] = Scope.BrushColor;
            
            if (expression.Interpret() is (int dirX, (int dirY, int distance)))
            {
                for (int d = 0; d < distance; d++)
                {
                    int newx = x + dirX;
                    int newy = y + dirY;

                    if (IsValid(newx, newy))
                    {
                        Scope.Picture[newx, newy] = Scope.BrushColor;
                        Scope.Position = (newx, newy);
                        x = newx;
                        y = newy;
                        Scope.flag = true;
                        Scope.animation = true;
                        await Task.Delay(delay);
                    }
                    else throw new RuntimeError($"Coordenadas de DrawLine fuera de rango: ({newx}, {newy})");
                }
                Scope.animation = false;
            }
        }
        public static async Task DrawCircle(Expression expression)
        {
            int x = Scope.Position.Item1;
            int y = Scope.Position.Item2;

            if (expression.Interpret() is (int dirX, (int dirY, int radius)))
            {
                int centerX = x + dirX;
                int centerY = y + dirY;

                if (!IsValid(centerX, centerY)) throw new RuntimeError($"Centro del círculo fuera de rango: ({centerX}, {centerY})");
                
                Scope.Position = (centerX, centerY);

                double step = Math.Max(1, 360.0 / (2 * Math.PI * radius));

                for (double a = 0; a < 360; a += step)
                {
                    double radians = a * Math.PI / 180;
                    int pixelX = (int)Math.Round(centerX + radius * Math.Cos(radians));
                    int pixelY = (int)Math.Round(centerY + radius * Math.Sin(radians));

                    if (IsValid(pixelX, pixelY))
                    {
                        Scope.Position = (pixelX, pixelY);
                        Scope.Picture[pixelX, pixelY] =Scope.BrushColor;
                        Scope.flag = true;
                        Scope.animation = true;
                        await Task.Delay(delay);
                    }
                }
                Scope.Position = (centerX, centerY);
                Scope.flag = true;
                Scope.animation = false;
            }
        }
        public static async Task DrawRectangle(Expression expression)
        {
            int x = Scope.Position.Item1;
            int y = Scope.Position.Item2;

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
                                Scope.Position = (i, j);
                                Scope.Picture[i, j] = Scope.BrushColor;
                                Scope.flag = true;
                                Scope.animation = true;
                                await Task.Delay(delay);
                            }
                        }
                    }
                   Scope.Position = (newx, newy);
                   Scope.flag = true;
                   Scope.animation = false;
                }
            }
        }
        public static async Task Fill()
        {
            int i = Scope.Position.Item1;
            int j = Scope.Position.Item2;
            Color Current = Scope.Picture[i, j];
            if (Current == Scope.BrushColor) return;

            Queue<(int, int)> queue = new();
            
            queue.Enqueue((i, j));
            Scope.Picture[i, j] = Scope.BrushColor;

            int[] dirX = {1, -1, 0,  0};
            int[] dirY = {0,  0, 1, -1};

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();

                for (int k = 0; k < dirX.Length; k++)
                {
                    int newX = x + dirX[k];
                    int newY = y + dirY[k];

                    if (IsValid(newX, newY) && Scope.Picture[newX, newY] == Current)
                    {
                        Scope.Picture[newX, newY] = Scope.BrushColor;
                        queue.Enqueue((newX, newY));
                        Scope.flag = true;
                        Scope.animation = true;
                        Scope.Position = (newX, newY);
                        await Task.Delay(delay);
                    }
                }
            }
            Scope.Position = (i, j);
            Scope.flag = true;
            Scope.animation = false;
        }
        public static int GetActualX()
        {
            return Scope.Position.Item1;
        }
        public static int GetActualY()
        {
            return Scope.Position.Item2;
        }
        public static int GetCanvasSize()
        {
            return Scope.Size;
        }
        public static bool IsBrushColor(Expression expression)
        {
            if (expression.Interpret() is Color color)
            {
                return color == Scope.BrushColor;
            }
            else throw new SemanticalError ($"Argumento de IsBrushColor no valido.");
        }
        public static bool IsBrushSize(Expression expression)
        {
            if (expression.Interpret() is int brush)
            {
                return brush == Scope.BrushSize;
            }
            else throw new SemanticalError ($"Argumento de IsBrushSize no valido.");
        }
        public static bool IsCanvasColor(Expression expression)
        {
            if (expression.Interpret() is (Color color, (int vertical, int horizontal)))
            {
                int x = Scope.Position.Item1;
                int y = Scope.Position.Item2;
                bool flag = true;

                for (int i = x; i < x + horizontal; i++)
                {
                    for (int j = y; y < y + vertical; j++)
                    {
                        if (Scope.Picture[i, j] != color)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag == false)
                    {
                        break;
                    }
                }
                return flag;
            }
            else throw new SemanticalError ($"Argumento de IsBrushColor no valido.");
        }
        public static int GetColorCount(Expression expression)
        {
            if (expression.Interpret() is (Color color, (int x1, (int y1, (int x2, int y2)))))
            {
                int count = 0;
                for (int i = y1; i < y2; i++)
                {
                    for (int j = x1; j < x2; j++)
                    {
                        if (Scope.Picture [i, j] == color) count++;
                    }
                }
                return count;
            }
            else
            {
                throw new SemanticalError ($"Argumento de GetColorCount no valido.");
            }
        }
        private static bool IsValid(int x, int y)
        {
            return x >= 0 && x < Scope.Size && y >= 0 && y < Scope.Size;
        }
    }
}