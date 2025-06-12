using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrushBot
{
    public static class Handle
    {
        public static int delay = 30;
        #region Checks
        public static void CheckSpawn(List<Expression> parameters, Context context)
        {
            if (parameters.Count != 2)
            {
                throw new CodeError(ErrorType.Count, parameters[0].Location, $"Spawn(int x, int y).");
            }

            foreach (Expression expression in parameters)
            {
                if (!(expression.Evaluate(context) is int))
                {
                    throw new CodeError(ErrorType.Typing, expression.Location, $"Spawn(int x, int y).");
                }
            }
        }
        public static void CheckPrint(List<Expression> parameters, Context context)
        {
            if (parameters.Count != 1)
            {
                throw new CodeError(ErrorType.Count, parameters[0].Location, $"Print(string message).");
            }

            if (!(parameters[0].Evaluate(context) is string))
            {
                throw new CodeError(ErrorType.Typing, parameters[0].Location, $"Print(string message).");
            }
        }
        public static void CheckColor(List<Expression> parameters, Context context)
        {
            if (parameters.Count != 1)
            {
                throw new CodeError(ErrorType.Count, parameters[0].Location, $"Color(string color).");
            }
            else if (!(parameters[0].Evaluate(context) is Color))
            {
                throw new CodeError(ErrorType.Typing, parameters[0].Location, $"Color(string color).");
            }
        }
        public static void CheckSize(List<Expression> parameters, Context context)
        {
            if (parameters.Count != 1)
            {
                throw new CodeError(ErrorType.Count, parameters[0].Location, $"Size(int size).");
            }
            if (!(parameters[0].Evaluate(context) is int))
            {
                throw new CodeError(ErrorType.Typing, parameters[0].Location, $"Size(int size).");
            }
        }
        public static void CheckDrawLine(List<Expression> parameters, Context context)
        {
            if (parameters.Count != 3)
            {
                throw new CodeError(ErrorType.Count, parameters[0].Location, $"DrawLine(int dirX, int dirY, int distance).");
            }

            foreach (Expression expression in parameters)
            {
                if (!(expression.Evaluate(context) is int))
                {
                    throw new CodeError(ErrorType.Typing, expression.Location, $"DrawLine(int dirX, int dirY, int distance).");
                }
            }
        }
        public static void CheckDrawCircle(List<Expression> parameters, Context context)
        {
            if (parameters.Count != 3)
            {
                throw new CodeError(ErrorType.Count, parameters[0].Location, $"DrawCircle(int dirX, int dirY, int radius).");
            }

            foreach (Expression expression in parameters)
            {
                if (!(expression.Evaluate(context) is int))
                {
                    throw new CodeError(ErrorType.Typing, expression.Location, $"DrawCircle(int dirX, int dirY, int radius).");
                }
            }
        }
        public static void CheckDrawRectangle(List<Expression> parameters, Context context)
        {
            if (parameters.Count != 5)
            {
                throw new CodeError(ErrorType.Count, parameters[0].Location, $"DrawRectangle(int dirX, int dirY, int distance, int width, int height).");
            }

            foreach (Expression expression in parameters)
            {
                if (!(expression.Evaluate(context) is int))
                {
                    throw new CodeError(ErrorType.Typing, expression.Location, $"DrawRectangle(int dirX, int dirY, int distance, int width, int height).");
                }
            }
        }
        public static void CheckFill(List<Expression> parameters, Context context)
        {
            if (parameters != null)
            {
                throw new CodeError(ErrorType.Typing, parameters[0].Location, $"Fill().");
            }
        }
        #endregion
        #region Instructions
        public static async Task Spawn(List<Expression> parameters, Context context)
        {
            int x = (int)parameters[0].Evaluate(context);
            int y = (int)parameters[1].Evaluate(context);

            if (IsValid(x, y, context))
            {
                context.Position = (x, y);
                context.Flag = true;
                await Task.Delay(0);
            }
            else throw new CodeError(ErrorType.IndexOutOfRange, parameters[0].Location, $"Spawn({x}, {y})");
        }
        public static async Task Print(List<Expression> parameters, Context context)
        {
            context.Message = (string)parameters[0].Evaluate(context);
            context.Print = true;
            await Task.Delay(delay);
        }
        public static async Task Color(List<Expression> parameters, Context context)
        {
            Color color = (Color)parameters[0].Evaluate(context);

            context.BrushColor = color;
            context.Flag = true;
            await Task.Delay(0);
        }
        public static async Task Size(List<Expression> parameters, Context context)
        {
            int size = (int)parameters[0].Evaluate(context);

            if (size % 2 == 0)
            {
                context.BrushSize = size - 1;
                context.Flag = true;
                await Task.Delay(0);
            }
            else context.BrushSize = size;
        }
        public static async Task DrawLine(List<Expression> parameters, Context context)
        {
            int x = context.Position.Item1;
            int y = context.Position.Item2;
            Paint(x, y, context);
            await Task.Delay(delay);

            int dirX = (int)parameters[0].Evaluate(context);
            int dirY = (int)parameters[1].Evaluate(context);
            int distance = (int)parameters[2].Evaluate(context);

            for (int d = 0; d < distance; d++)
            {
                int newx = x + dirX;
                int newy = y + dirY;

                if (IsValid(newx, newy, context))
                {
                    Paint(newx, newy, context);
                    context.Position = (newx, newy);
                    x = newx;
                    y = newy;
                    context.Animation = true;
                    await Task.Delay(delay);
                }
                else throw new CodeError(ErrorType.IndexOutOfRange, parameters[0].Location, $"DrawLine({newx}, {newy})");
            }
            context.Animation = false;
        }
        public static async Task DrawCircle(List<Expression> parameters, Context context)
        {
            int x = context.Position.Item1;
            int y = context.Position.Item2;

            int dirX = (int)parameters[0].Evaluate(context);
            int dirY = (int)parameters[1].Evaluate(context);
            int radius = (int)parameters[2].Evaluate(context);

            int centerX = x + dirX * radius;
            int centerY = y + dirY * radius;

            if (!IsValid(centerX, centerY, context)) throw new CodeError(ErrorType.IndexOutOfRange, parameters[0].Location, $"Center of circle ({centerX}, {centerY})");

            context.Position = (centerX, centerY);

            double step = Math.Max(1, 360.0 / (2 * Math.PI * radius));

            for (double a = 0; a < 360; a += step)
            {
                double radians = a * Math.PI / 180;
                int pixelX = (int)Math.Round(centerX + radius * Math.Cos(radians));
                int pixelY = (int)Math.Round(centerY + radius * Math.Sin(radians));

                if (IsValid(pixelX, pixelY, context))
                {
                    Paint(pixelX, pixelY, context);
                    context.Position = (pixelX, pixelY);
                    context.Animation = true;
                    await Task.Delay(delay);
                }
            }
            context.Position = (centerX, centerY);
            context.Flag = true;
            context.Animation = false;
        }
        public static async Task DrawRectangle(List<Expression> parameters, Context context)
        {
            int x = context.Position.Item1;
            int y = context.Position.Item2;

            int dirX = (int)parameters[0].Evaluate(context);
            int dirY = (int)parameters[1].Evaluate(context);
            int distance = (int)parameters[2].Evaluate(context);
            int width = (int)parameters[3].Evaluate(context);
            int height = (int)parameters[4].Evaluate(context);

            int newx = x + dirX * distance;
            int newy = y + dirY * distance;

            if (!IsValid(newx, newy, context)) throw new CodeError(ErrorType.IndexOutOfRange, parameters[0].Location, $"Center of rectangle ({newx}, {newy})");
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
                        if (isBorder && IsValid(i, j, context))
                        {
                            context.Position = (i, j);
                            Paint(i, j, context);
                            context.Animation = true;
                            await Task.Delay(delay);
                        }
                    }
                }
                context.Position = (newx, newy);
                context.Flag = true;
                context.Animation = false;
            }
        }
        public static async Task Fill(Context context)
        {
            if (context.BrushColor.Predef == PredefColor.Transparent)
            {
                return;
            }

            int i = context.Position.Item1;
            int j = context.Position.Item2;
            Color Current = context.Picture[i, j];
            if (Current == context.BrushColor) return;

            Queue<(int, int)> queue = new();

            queue.Enqueue((i, j));
            context.Picture[i, j] = context.BrushColor;

            int[] dirX = { 1, -1, 0, 0 };
            int[] dirY = { 0, 0, 1, -1 };

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();

                for (int k = 0; k < dirX.Length; k++)
                {
                    int newX = x + dirX[k];
                    int newY = y + dirY[k];

                    if (IsValid(newX, newY, context) && context.Picture[newX, newY] == Current)
                    {
                        context.Picture[newX, newY] = context.BrushColor;
                        queue.Enqueue((newX, newY));
                        context.Flag = true;
                        context.Animation = true;
                        context.Position = (newX, newY);
                        await Task.Delay(delay);
                    }
                }
            }
            context.Position = (i, j);
            context.Flag = true;
            context.Animation = false;
        }
        #endregion
        #region Functions
        public static Color RGBA(List<Expression> parameters, Context context)
        {
            if (parameters.Count != 4)
            {
                throw new CodeError(ErrorType.Count, parameters[0].Location, "ARGB(int alpha, int red, int green, int blue).");
            }

            foreach (Expression expression in parameters)
            {
                if (!(expression.Evaluate(context) is int))
                {
                    throw new CodeError(ErrorType.Typing, expression.Location, "ARGB(int alpha, int red, int green, int blue).");
                }

                int current = (int)expression.Evaluate(context);

                if (!(current >= 0 && current <= 255))
                {
                    throw new CodeError(ErrorType.Invalid, expression.Location, $"{current} must be in range 0 to 254.");
                }
            }

            float R = (int)parameters[0].Evaluate(context) / 255f;
            float G = (int)parameters[1].Evaluate(context) / 255f;
            float B = (int)parameters[2].Evaluate(context) / 255f;
            float A = (int)parameters[3].Evaluate(context) / 255f;

            return new Color(null, R, G, B, A);
        }
        public static int GetActualX(List<Expression> parameters, Context context)
        {
            if (parameters == null)
            {
                return context.Position.Item1;
            }
            else throw new CodeError(ErrorType.Typing, parameters[0].Location, "GetActualX().");
        }
        public static int GetActualY(List<Expression> parameters, Context context)
        {
            if (parameters == null)
            {
                return context.Position.Item2;
            }
            else throw new CodeError(ErrorType.Typing, parameters[0].Location, "GetActualY().");
        }
        public static int GetCanvasSize(List<Expression> parameters, Context context)
        {
            if (parameters == null)
            {
                return context.Size;
            }
            else throw new CodeError(ErrorType.Typing, parameters[0].Location, "GetCanvasSize().");
        }
        public static bool IsBrushColor(List<Expression> parameters, Context context)
        {
            if (parameters.Count != 1)
            {
                throw new CodeError(ErrorType.Count, parameters[0].Location, "IsBrushColor(string color).");
            }
            else if (parameters[0].Evaluate(context) is Color color)
            {
                return color == context.BrushColor;
            }
            else throw new CodeError(ErrorType.Typing, parameters[0].Location, "IsBrushColor(string color).");
        }
        public static bool IsBrushSize(List<Expression> parameters, Context context)
        {
            if (parameters.Count != 1)
            {
                throw new CodeError(ErrorType.Count, parameters[0].Location, "IsBrushSize(int size).");
            }
            else if (parameters[0].Evaluate(context) is int brush)
            {
                return brush == context.BrushSize;
            }
            else throw new CodeError(ErrorType.Typing, parameters[0].Location, "IsBrushSize(int size).");
        }
        public static bool IsCanvasColor(List<Expression> parameters, Context context)
        {
            if (parameters.Count != 3)
            {
                throw new CodeError(ErrorType.Count, parameters[0].Location, "IsCanvasColor(string color, int vertical, int horizontal).");
            }
            else if (parameters[0].Evaluate(context) is Color color)
            {
                if (parameters[1].Evaluate(context) is int vertical)
                {
                    if (parameters[2].Evaluate(context) is int horizontal)
                    {
                        int x = context.Position.Item1;
                        int y = context.Position.Item2;
                        bool Flag = true;

                        for (int i = x; i < x + horizontal; i++)
                        {
                            for (int j = y; y < y + vertical; j++)
                            {
                                if (context.Picture[i, j] != color)
                                {
                                    Flag = false;
                                    break;
                                }
                            }
                            if (Flag == false)
                            {
                                break;
                            }
                        }
                        return Flag;
                    }
                    else throw new CodeError(ErrorType.Typing, parameters[2].Location, "IsCanvasColor(string color, int vertical, int horizontal).");
                }
                else throw new CodeError(ErrorType.Typing, parameters[1].Location, "IsCanvasColor(string color, int vertical, int horizontal).");
            }
            else throw new CodeError(ErrorType.Typing, parameters[0].Location, "IsCanvasColor(string color, int vertical, int horizontal).");
        }
        public static int GetColorCount(List<Expression> parameters, Context context)
        {
            if (parameters.Count != 5)
            {
                throw new CodeError(ErrorType.Count, parameters[0].Location, "GetColorCount(string color, int x1, int y1, int x2, int y2)");
            }
            else if (parameters[0].Evaluate(context) is Color color)
            {
                if (parameters[1].Evaluate(context) is int x1)
                {
                    if (parameters[2].Evaluate(context) is int y1)
                    {
                        if (parameters[3].Evaluate(context) is int x2)
                        {
                            if (parameters[4].Evaluate(context) is int y2)
                            {
                                int count = 0;
                                for (int i = y1; i < y2; i++)
                                {
                                    for (int j = x1; j < x2; j++)
                                    {
                                        if (context.Picture[i, j] == color) count++;
                                    }
                                }
                                return count;
                            }
                            else throw new CodeError(ErrorType.Typing, parameters[4].Location, "GetColorCount(string color, int x1, int y1, int x2, int y2).");
                        }
                        else throw new CodeError(ErrorType.Typing, parameters[3].Location, "GetColorCount(string color, int x1, int y1, int x2, int y2).");
                    }
                    else throw new CodeError(ErrorType.Typing, parameters[2].Location, "GetColorCount(string color, int x1, int y1, int x2, int y2).");
                }
                else throw new CodeError(ErrorType.Typing, parameters[1].Location, "GetColorCount(string color, int x1, int y1, int x2, int y2).");
            }
            else throw new CodeError(ErrorType.Typing, parameters[0].Location, "GetColorCount(string color, int x1, int y1, int x2, int y2).");
        }
        #endregion
        #region Aux
        private static bool IsValid(int x, int y, Context context)
        {
            return x >= 0 && x < context.Size && y >= 0 && y < context.Size;
        }
        private static void Paint(int x, int y, Context context)
        {
            if (context.BrushColor.Predef == PredefColor.Transparent)
            {
                context.Flag = true;
                return;
            }

            int limit = context.BrushSize / 2;

            for (int i = -limit; i <= limit; i++)
            {
                for (int j = -limit; j <= limit; j++)
                {
                    int newX = x + i;
                    int newY = y + j;

                    if (IsValid(newX, newY, context))
                    {
                        context.Picture[newX, newY] = context.BrushColor;
                    }
                }
            }
            context.Flag = true;
        }
        #endregion
    }
}