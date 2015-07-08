using System;

namespace FractalLibrary
{
	public struct FractalVector2
	{
		public float x;
		public float y;

		public FractalVector2(float x = 0f, float y = 0f)
		{
			this.x = x;
			this.y = y;
		}

		public FractalVector2(FractalVector2 v)
		{
			this.x = v.x;
			this.y = v.y;
		}

		public FractalVector2(FractalComplexNumber v)
		{
			this.x = v.Real;
			this.y = v.Imaginary;
		}

		public static FractalVector2 operator +(FractalVector2 v1, FractalVector2 v2)
		{
			return new FractalVector2 (v1.x + v2.x, v1.y + v2.y);
		}

		public static FractalVector2 operator -(FractalVector2 v1, FractalVector2 v2)
		{
			return new FractalVector2 (v1.x - v2.x, v1.y - v2.y);
		}

		public static FractalVector2 operator *(FractalVector2 v, float f)
		{
			return new FractalVector2(v.x * f, v.y * f);
		}

		public static FractalVector2 operator *(float f, FractalVector2 v)
		{
			return new FractalVector2(v.x * f, v.y * f);
		}

		public static FractalVector2 operator /(FractalVector2 v, float f)
		{
			return new FractalVector2(v.x / f, v.y / f);
		}

		public static bool operator == (FractalVector2 v1, FractalVector2 v2)
		{
			return v1.x == v2.x && v1.y == v2.y;
		}

		public static bool operator != (FractalVector2 v1, FractalVector2 v2)
		{
			return v1.x != v2.x | v1.y != v2.y;
		}

		public float Magnitude
		{
			get {
				return (float)Math.Sqrt (x * x + y * y);
			}
		}

		public override bool Equals (object obj)
		{
			FractalVector2 target = (FractalVector2)obj;

			return target.x == this.x && target.y == this.y;
		}

		public FractalVector2 Normalize()
		{
			return new FractalVector2 (x / Magnitude, y / Magnitude);
		}

		public override string ToString ()
		{
			return string.Format ("[FractalVector2: x={0}, y={1}]", x, y);
		}
	}
}

