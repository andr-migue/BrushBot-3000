using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BrushBot
{
    public static class Handle
    {
        public static int delay = 30;
        public static void CheckSpawn(List<Expression> parameters)
        {
            if (parameters.Count != 2)
            {
                throw new CodeError (ErrorType.Count, parameters[0].Location, $"Spawn(int x, int y).");
            }

            foreach (Expression expression in parameters)
            {
                if (!(expression.Evaluate() is int))
                {
                    throw new CodeError (ErrorType.Typing, expression.Location, $"Spawn(int x, int y).");
                }
            }
        }
        public static void CheckColor(List<Expression> parameters)
        {
            if (parameters.Count != 1)
            {
                throw new CodeError (ErrorType.Count, parameters[0].Location, $"Color(string color).");
            }
            else if (!(parameters[0].Evaluate() is Color))
            {
                throw new CodeError (ErrorType.Typing, parameters[0].Location, $"Color(string color).");
            }
        }
        public static void CheckSize(List<Expression> parameters)
        {
            if (parameters.Count != 1)
            {
                throw new CodeError (ErrorType.Count, parameters[0].Location, $"Size(int size).");
            }
            if (!(parameters[0].Evaluate() is int))
            {
                throw new CodeError ($"Error de tipado: Size(int size).");
            }
        }
        public static void CheckDrawLine(List<Expression> parameters)
        {
            if (parameters.Count != 3)
            {
                throw new CodeError ($"Error: DrawLine(int dirX, int dirY, int distance).");
            }

            foreach (Expression expression in parameters)
            {
                if (!(expression.Evaluate() is int))
                {
                    throw new CodeError ($"Error de tipado: DrawLine(int dirX, int dirY, int distance).");
                }
            }
        }
        public static void CheckDrawCircle(List<Expression> parameters)
        {
            if (parameters.Count != 3)
            {
                throw new CodeError ($"Error: DrawCircle(int dirX, int dirY, int radius).");
            }

            foreach (Expression expression in parameters)
            {
                if (!(expression.Evaluate() is int))
                {
                    throw new CodeError ($"Error de tipado: DrawCircle(int dirX, int dirY, int radius).");
                }
            }
        }
        public static void CheckDrawRectangle(List<Expression> parameters)
        {
            if (parameters.Count != 5)
            {
                throw new CodeError ($"Error: DrawRectangle(int dirX, int dirY, int distance, int width, int height).");
            }

            foreach (Expression expression in parameters)
            {
                if (!(expression.Evaluate() is int))
                {
                    throw new CodeError ($"Error de tipado: DrawRectangle(int dirX, int dirY, int distance, int width, int height).");
                }
            }
        }
        public static void CheckFill(List<Expression> parameters)
        {
            if (parameters != null)
            {
                throw new CodeError ($"Error: Fill().");
            }
        }
        public static async Task Spawn(List<Expression> parameters)
        {
            int x = (int)parameters[0].Evaluate();
            int y = (int)parameters[1].Evaluate();

            if (IsValid(x, y))
            {
                Scope.Position = (x, y);
                Scope.flag = true;
                await Task.Delay(0);
            }
            else throw new CodeError ($"Error: Coordenadas de Spawn() fuera de rango");
        }
        public static async Task Color(List<Expression> parameters)
        {
            Color color = (Color)parameters[0].Evaluate();

            Scope.BrushColor = color;
            Scope.flag = true;
            await Task.Delay(0);
        }
        public static async Task Size(List<Expression> parameters)
        {
            int size = (int)parameters[0].Evaluate();

            if (size % 2 == 0)
            {
                Scope.BrushSize = size - 1;
                Scope.flag = true;
                await Task.Delay(0);
            }
            else Scope.BrushSize = size;
        }
        public static async Task DrawLine(List<Expression> parameters)
        {
            int x = Scope.Position.Item1;
            int y = Scope.Position.Item2;
            Scope.Picture[x, y] = Scope.BrushColor;
            
            int dirX = (int)parameters[0].Evaluate();
            int dirY = (int)parameters[1].Evaluate();
            int distance = (int)parameters[2].Evaluate();

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
                else throw new CodeError($"Error: Coordenadas de DrawLine fuera de rango: ({newx}, {newy})");
            }
            Scope.animation = false;
        }
        public static async Task DrawCircle(List<Expression> parameters)
        {
            int x = Scope.Position.Item1;
            int y = Scope.Position.Item2;

            int dirX = (int)parameters[0].Evaluate();
            int dirY = (int)parameters[1].Evaluate();
            int radius = (int)parameters[2].Evaluate();

            int centerX = x + dirX;
            int centerY = y + dirY;

            if (!IsValid(centerX, centerY)) throw new CodeError($"Error: Centro del círculo fuera de rango: ({centerX}, {centerY})");
                
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
        public static async Task DrawRectangle(List<Expression> parameters)
        {
            int x = Scope.Position.Item1;
            int y = Scope.Position.Item2;

            int dirX = (int)parameters[0].Evaluate();
            int dirY = (int)parameters[1].Evaluate();
            int distance = (int)parameters[2].Evaluate();
            int width = (int)parameters[3].Evaluate();
            int height = (int)parameters[4].Evaluate();

            int newx = x + dirX * distance;
            int newy = y + dirY * distance;

            if (!IsValid(newx, newy)) throw new CodeError($"Error: Centro del rectángulo fuera de rango: ({newx}, {newy})");
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
        public static int GetActualX(List<Expression> parameters)
        {
            if (parameters == null)
            {
                return Scope.Position.Item1;
            }
            else throw new CodeError("Error: GetActualX() no recibe parametros.");
        }
        public static int GetActualY(List<Expression> parameters)
        {
            if (parameters == null)
            {
                return Scope.Position.Item2;
            }
            else throw new CodeError("Error: GetActualY() no recibe parametros.");
        }
        public static int GetCanvasSize(List<Expression> parameters)
        {
            if (parameters == null)
            {
                return Scope.Size;
            }
            else throw new CodeError("Error: GetCanvasSize() no recibe parametros.");
        }
        public static bool IsBrushColor(List<Expression> parameters)
        {
            if (parameters.Count != 1)
            {
                throw new CodeError("Error: IsBrushColor() recibe solo un parametro.");
            }
            else if (parameters[0].Evaluate() is Color color)
            {
                return color == Scope.BrushColor;
            }
            else throw new CodeError ("Error de tipado: IsBrushColor(string color).");
        }
        public static bool IsBrushSize(List<Expression> parameters)
        {
            if (parameters.Count != 1)
            {
                throw new CodeError("Error: IsBrushSize() recibe solo un parametro.");
            }
            else if (parameters[0].Evaluate() is int brush)
            {
                return brush == Scope.BrushSize;
            }
            else throw new CodeError ("Error de tipado: IsBrushSize(int size).");
        }
        public static bool IsCanvasColor(List<Expression> parameters)
        {
            if (parameters.Count != 3)
            {
                throw new CodeError("Error: IsCanvasColor() recibe tres parametros.");
            }
            else if (parameters[0].Evaluate() is Color color &&
                     parameters[1].Evaluate() is int vertical &&
                     parameters[2].Evaluate() is int horizontal)
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
            else throw new CodeError ("Error de tipado: IsCanvasColor(string color, int vertical, int horizontal).");
        }
        public static int GetColorCount(List<Expression> parameters)
        {
            if (parameters.Count != 5)
            {
                throw new CodeError("Error: GetColorCount() recibe cinco parametros");
            }
            else if (parameters[0].Evaluate() is Color color &&
                     parameters[1].Evaluate() is int x1 &&
                     parameters[2].Evaluate() is int y1 &&
                     parameters[3].Evaluate() is int x2 &&
                     parameters[4].Evaluate() is int y2)
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
                throw new CodeError ("Error de tipado: GetColorCount(string color, int x1, int y1, int x2, int y2).");
            }
        }
        private static bool IsValid(int x, int y)
        {
            return x >= 0 && x < Scope.Size && y >= 0 && y < Scope.Size;
        }
    }
}