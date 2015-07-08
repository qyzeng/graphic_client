using System;

namespace FractalLibrary
{
	public class HofstadterFractal : BaseFractal
	{
		private int GreatestCommonDivisor(int a, int b)
		{
			if (b == 0) {
				return a;
			}
			return GreatestCommonDivisor (b, a % b);
		}

		public HofstadterFractal ()
		{
		}

		public override void RefreshDataSamples ()
		{
			int arraySize = mData.GetUpperBound (0) + 1;
			for (int q = 4; q < mData.GetUpperBound (0); q += 2) {
				
			}
			//throw new NotImplementedException ();
		}
	}
}

