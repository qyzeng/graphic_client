using System;

namespace FractalLibrary
{
	public static class FractalUtility
	{
		public class ThreadData
		{
			public int start;
			public int end;
			public object[] Arguments;
			public ThreadData (int s, int e, params object[] args)
			{
				start = s;
				end = e;
				Arguments = args;
			}
		}

		public static void QuadMandelbrotIterate(int iterations, out float returnVal, params FractalComplexNumber[] complexNos)
		{
			returnVal = -1f;
			FractalComplexNumber z = new FractalComplexNumber ();
			FractalComplexNumber c = complexNos [0];
			for (int i = 0; i < iterations + 1; ++i)
			{
				z = z * z + c;
				if (z.Absolute >= 2f)
				{
					returnVal = (float)i / (float)iterations;
					break;
				}
			}
		}

		public static void QuadJulietIterate(int iterations, out float returnVal, params FractalComplexNumber[] complexNos)
		{
			returnVal = -1f;
			FractalComplexNumber z = new FractalComplexNumber ();
			if (complexNos.Length > 1)
			{
				z = complexNos [1];
			}
			FractalComplexNumber c = complexNos [0];
			for (int i = 1; i < iterations + 1; ++i)
			{
				z = z * z + c;
				if (z.Absolute >= 2f)
				{
					returnVal = (float)i / (float)iterations;
					break;
				}
			}
		}

		static float[] GenerateBoxForGauss(float sigma, int n)  // standard deviation, number of boxes
		{
			var wIdeal = Math.Sqrt((12*sigma*sigma/n)+1);  // Ideal averaging filter width 
			var wl = Math.Floor(wIdeal);  if(wl%2==0) wl--;
			var wu = wl+2;

			var mIdeal = (12*sigma*sigma - n*wl*wl - 4*n*wl - 3*n)/(-4*wl - 4);
			var m = Math.Round(mIdeal);
			// var sigmaActual = Math.sqrt( (m*wl*wl + (n-m)*wu*wu - n)/12 );

			var sizes = new float[n];  
			for(var i=0; i<n; i++) 
				sizes[i] = (float)(i<m?wl:wu);
			return sizes;
		}

		public static void GaussBlurData (float[,] scl, float[,]tcl, float r) {
			int w = scl.GetUpperBound (0) + 1;
			int h = scl.GetUpperBound (1) + 1;
		    var bxs = GenerateBoxForGauss(r, 3);
		    boxBlur_4 (scl, tcl, w, h, (bxs[0]-1)/2);
		    boxBlur_4 (tcl, scl, w, h, (bxs[1]-1)/2);
		    boxBlur_4 (scl, tcl, w, h, (bxs[2]-1)/2);
		}
		static void boxBlur_4 (float[,] scl, float[,] tcl, int w, int h, float r) 
		{
			    //for(var i=0; i<scl.length; i++) tcl[i] = scl[i];
			tcl = (float[,])scl.Clone();
		    boxBlurH_4(tcl, scl, w, h, r);
		    boxBlurT_4(scl, tcl, w, h, r);
		}

		static void boxBlurH_4 (float[,] scl, float[,] tcl, int w, int h, float r) 
		{
		    var iarr = 1 / (r+r+1);
			for(var i=0; i<scl.GetUpperBound(1) + 1; i++) {
				int ti = 0;//i * w; 
				int li = ti;
				int ri = (int)r;//ti+r;
				float fv = scl [0, i];
				float lv = scl [w - 1, i];
				float val = (r+1)*fv;
		        for(var j=0; j<r; j++)
					val += scl[j, i];
				for(var j=0  ; j<=r ; j++) 
				{ 
					val += scl[j, i] - fv;
					tcl[j, i] = (float)(val*iarr); 
					//Console.WriteLine (String.Format ("Data {0}, {1}: {2}", j, i, tcl [j, i]));
				}
				for(var j=r+1; j<w-r; j++) 
				{ 
					val += scl[(int)j, i] - scl[(int)(j-r-1), i];
					tcl[(int)j, i] = (float)(val*iarr); 
					//Console.WriteLine (String.Format ("Data {0}, {1}: {2}", j, i, tcl [(int)j, i]));
				}
				for(var j=w-r; j<w  ; j++) 
				{ 
					val += lv - scl[(int)(w-j), i];
					tcl[(int)j, i] = (float)(val*iarr); 
					//Console.WriteLine (String.Format ("Data {0}, {1}: {2}", j, i, tcl [(int)j, i]));
				}
		    }
		}

		static void boxBlurT_4 (float[,] scl, float[,] tcl, int w, int h, float r) 
		{
		    var iarr = 1 / (r+r+1);

		    for(var i=0; i<w; i++) 
			{
				var ti = i;
				var li = ti;
				var ri = r;//ti+r*w;
				var fv = scl [ti, 0];
				var lv = scl [ti, (h - 1)];
				var val = (r+1)*fv;
		        for(var j=0; j<r; j++) 
					val += scl[ti, j];
		        for(var j=0  ; j<=r ; j++) 
				{ 
					val += scl[i,j] - fv;
					tcl[i,j] = (float)(val*iarr);
					//Console.WriteLine (String.Format ("Data {0}, {1}: {2}", i, j, tcl [i, j]));
					//ri+=w; 
					//ti+=w; 
				}
		        for(var j=r+1; j<h-r; j++) 
				{ 
					val += scl[i, (int)j] - scl[i, (int)(j-r-1)];
					tcl[i, (int)j] = (float)(val*iarr);
					//Console.WriteLine (String.Format ("Data {0}, {1}: {2}", i, j, tcl [i, (int)j]));
					//li+=w;
					//ri+=w;
					//ti+=w;
				}
		        for(var j=h-r; j<h  ; j++) 
				{ 
					val += lv - scl[i, (int)(h-j)];
					tcl[i, (int)j] = (float)(val*iarr);
					//Console.WriteLine (String.Format ("Data {0}, {1}: {2}", i, j, tcl [i, (int)j]));
					//li+=w;
					//ti+=w;
				}
		    }
		}
	}
}

