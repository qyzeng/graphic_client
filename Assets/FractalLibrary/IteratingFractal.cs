using System;

namespace FractalLibrary
{
	public class IteratingFractal : BaseFractal
	{
		public int Iterations = 100000;

		protected float[,] mDataToReturn;
		protected int mDataSizeX;
		protected int mDataSizeY;

		private bool mDataInit = false;

		public override float[,] Data {
			get {
				return (float[,])mDataToReturn.Clone();
			}
		}

		protected FractalVector2 mInitialPoint = new FractalVector2(-5f, -5f);
		public FractalVector2 InitialPoint
		{
			get{ return mInitialPoint; }
		}

		public void SetInitialIterationPoint(float x, float y)
		{
			mInitialPoint.x = x;
			mInitialPoint.y = y;
		}

		public virtual void SetDataResolution(int resX, int resY)
		{
			base.SetDataSize (resX, resY);
		}

		public override void SetDataSize (int sizeX, int sizeY)
		{
			mDataSizeX = sizeX;
			mDataSizeY = sizeY;
			if (!mDataInit) {
				SetDataResolution (sizeX, sizeY);
				mDataInit = true;
			}
		}

		protected override void InvokeDataGeneratedComplete ()
		{
			int cores = System.Environment.ProcessorCount > 1 ? System.Environment.ProcessorCount - 1 : 1;
			if (mData.GetUpperBound (0) + 1 != mDataSizeX || mData.GetUpperBound (1) + 1 != mDataSizeY) {
				FloatArray2dResize.Resize (base.Data, mDataSizeX, mDataSizeY, cores, OnResizeDataDone);
			}
			else {
				mDataToReturn = base.Data;
				base.InvokeDataGeneratedComplete ();
			}
		}

		protected virtual void OnResizeDataDone(System.Object obj)
		{
			mDataToReturn = (float[,])obj;
			base.InvokeDataGeneratedComplete ();
		}

		public override void RefreshDataSamples ()
		{
			throw new NotImplementedException ();
		}
	}
}

