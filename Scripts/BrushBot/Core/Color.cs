namespace BrushBot
{
    public class Color
    {
        public PredefColor? Predef;
        public float? R;
        public float? G;
        public float? B;
        public float? A;

        public Color(PredefColor? predef, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            Predef = predef;
            
            R = r;
            G = g;
            B = b;
            A = a;
        }
        public override string ToString()
        {
            if (Predef != null)
            {
                return Predef.ToString();
            }

            else
            {
                return R.ToString() + G.ToString() + B.ToString() + A.ToString();
            }
		}

        // Sobrecargas necesarias para comparar Colores.
        public override bool Equals(object obj)
        {
            if (obj is Color color)
            {
                if (Predef != null && color.Predef != null)
                {
                    return Predef == color.Predef;
                }
                else
                {
                    return R == color.R && G == color.G && B == color.B && A == color.A;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (Predef != null)
            {
                return Predef.GetHashCode();
            }
            else
            {
                return System.HashCode.Combine(R, G, B, A);
            }   
        }

        public static bool operator ==(Color left, Color right)
        {
            if (ReferenceEquals(left, right)) return true;
            
            if (left is null || right is null) return false;

            return left.Equals(right);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
        }

    }
	public enum PredefColor
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
}