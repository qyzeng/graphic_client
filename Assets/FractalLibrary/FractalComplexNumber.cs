using System;

namespace FractalLibrary
{
	public struct FractalComplexNumber
	{
		public float Real;
		public float Imaginary;

		public FractalComplexNumber(float real = 0f, float imaginary = 0f)
		{
			this.Real = real;
			this.Imaginary = imaginary;
		}

		public FractalComplexNumber(FractalComplexNumber v)
		{
			this.Real = v.Real;
			this.Imaginary = v.Imaginary;
		}

		public FractalComplexNumber(FractalVector2 v)
		{
			this.Real = v.x;
			this.Imaginary = v.y;
		}

		public static FractalComplexNumber operator +(FractalComplexNumber v1, FractalComplexNumber v2)
		{
			return new FractalComplexNumber (v1.Real + v2.Real, v1.Imaginary + v2.Imaginary);
		}

		public static FractalComplexNumber operator -(FractalComplexNumber v1, FractalComplexNumber v2)
		{
			return new FractalComplexNumber (v1.Real - v2.Real, v1.Imaginary - v2.Imaginary);
		}

		public static FractalComplexNumber operator *(FractalComplexNumber v, float f)
		{
			return new FractalComplexNumber(v.Real * f, v.Imaginary * f);
		}

		public static FractalComplexNumber operator *(float f, FractalComplexNumber v)
		{
			return new FractalComplexNumber(v.Real * f, v.Imaginary * f);
		}


		public static FractalComplexNumber operator *(FractalComplexNumber v1, FractalComplexNumber v2)
		{
			return new FractalComplexNumber ((v1.Real * v2.Real) - (v1.Imaginary * v2.Imaginary), (v1.Real * v2.Imaginary) + (v1.Imaginary * v2.Real));
		}


		public static FractalComplexNumber operator /(FractalComplexNumber v, float f)
		{
			return new FractalComplexNumber(v.Real / f, v.Imaginary / f);
		}

		public static bool operator == (FractalComplexNumber v1, FractalComplexNumber v2)
		{
			return v1.Real == v2.Real && v1.Imaginary == v2.Imaginary;
		}

		public static bool operator != (FractalComplexNumber v1, FractalComplexNumber v2)
		{
			return v1.Real != v2.Real | v1.Imaginary != v2.Imaginary;
		}

		public float Absolute
		{
			get
			{
				return (float)(Math.Sqrt (Real * Real + Imaginary * Imaginary));
			}
		}

		public override bool Equals (object obj)
		{
			FractalComplexNumber target = (FractalComplexNumber)obj;

			return this.Real == target.Real && this.Imaginary == target.Imaginary;
		}

		public override string ToString ()
		{
			return string.Format ("[FractalComplexNumber: Real={0}, Imaginary={1}]", Real, Imaginary);
		}
	}
}

