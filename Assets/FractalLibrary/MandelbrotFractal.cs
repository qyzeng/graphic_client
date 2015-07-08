using System;
using System.ComponentModel;
using System.Threading;

namespace FractalLibrary
{
	public class MandelbrotFractal : IteratingFractal
	{
		private class ComputeBounds
		{
			public FractalVector2 Max;
			public FractalVector2 Min;
		}

		public delegate void MandelbrotFractalIterateFunction (int numberOfIterations,out float returnValue,params FractalComplexNumber[] complexNos);
		private MandelbrotFractalIterateFunction mCurrentIterateFunction = null;

		private FractalVector2 mCenter = new FractalVector2 (1.5f, 1.5f);

		private int mFinishedThreadCount = 0;

		private Mutex mMutex = null;

		public bool UseGaussianSmooth = false;

		BackgroundWorker mWorker = new BackgroundWorker ();

		private System.Collections.Generic.Queue<ComputeBounds> mWorkQueue = new System.Collections.Generic.Queue<ComputeBounds> ();

		public MandelbrotFractal ()
		{
			mWorker.WorkerSupportsCancellation = true;
			mWorker.DoWork += StartComputation;
			mWorker.RunWorkerCompleted += ComputationEnd;
		}

		~MandelbrotFractal ()
		{
			mWorker.RunWorkerCompleted -= ComputationEnd;
			mWorker.DoWork -= StartComputation;
		}

		public void SetIteratingFunction (MandelbrotFractalIterateFunction func)
		{
			mCurrentIterateFunction = func;
		}

		public void SetCenter (float x, float y)
		{
			mCenter.x = x;
			mCenter.y = y;
		}

		private float Iterate (params FractalComplexNumber[] complexNos)
		{
			float returnVal = -1f;
			if (mCurrentIterateFunction != null) {
				mCurrentIterateFunction (Iterations, out returnVal, complexNos);
			}
			return returnVal;
		}

		public override void RefreshDataSamples ()
		{
			ComputeBounds boundsToCompute = new ComputeBounds () {
				Max = new FractalVector2 (mMaxX, mMaxY),
				Min = new FractalVector2 (mMinX, mMinY),
			};
			if (!mWorker.IsBusy) {
				mWorker.RunWorkerAsync (boundsToCompute);
			} else {
				mWorkQueue.Enqueue (boundsToCompute);
			}
			//StartComputation ();
		}

		private void ComputationEnd (object sender, RunWorkerCompletedEventArgs args)
		{
			InvokeDataGeneratedComplete ();
			if (mWorkQueue.Count > 0) {
				ComputeBounds queuedWork = mWorkQueue.Dequeue ();
				mWorker.RunWorkerAsync (queuedWork);
			}
		}

		private void StartComputation (object sender, DoWorkEventArgs args)
		{
			BackgroundWorker currentWorker = sender as BackgroundWorker;

			mFinishedThreadCount = 0;
			if (mMutex == null) {
				mMutex = new Mutex (false);
			}

			int coresToUse = System.Environment.ProcessorCount < 2 ? 1 : System.Environment.ProcessorCount - 1;
			int cores = Math.Min (coresToUse, mData.GetUpperBound (1) + 1);
			ComputeBounds boundsToCompute = args.Argument as ComputeBounds;
			if (cores > 1) {
				int slice = (int)(Math.Floor ((float)(mData.GetUpperBound (1) + 1) / cores));

				//Console.WriteLine ("Number of parallel threads used: {0}", cores.ToString ());

				for (int i = 0; i < cores - 1; ++i) {
					FractalUtility.ThreadData td = new FractalUtility.ThreadData (slice * i, slice * (i + 1), boundsToCompute);
					ParameterizedThreadStart ts = new ParameterizedThreadStart (this.ThreadedIterate);
					Thread newthread = new Thread (ts);
					newthread.Start (td);
					if (currentWorker.CancellationPending)
						break;
				}

				FractalUtility.ThreadData lasttd = new FractalUtility.ThreadData (slice * (cores - 1), mData.GetUpperBound (1) + 1, boundsToCompute);
				ThreadedIterate (lasttd);


				while (mFinishedThreadCount < cores && !currentWorker.CancellationPending) {
					Thread.Sleep (1);
				}
			} else {
				FractalUtility.ThreadData lasttd = new FractalUtility.ThreadData (0, mData.GetUpperBound (1) + 1, boundsToCompute);
				ThreadedIterate (lasttd);
			}
		}

		private void ThreadedIterate (System.Object obj)
		{
			FractalUtility.ThreadData td = (FractalUtility.ThreadData)obj;
			ComputeBounds boundsToUse = td.Arguments [0] as ComputeBounds;
			float xStep = (boundsToUse.Max.x - boundsToUse.Min.x) / (mData.GetUpperBound (0) + 1);
			float yStep = (boundsToUse.Max.y - boundsToUse.Min.y) / (mData.GetUpperBound (1) + 1);
			for (int y = td.start; y < td.end; ++y) {
				for (int x = 0; x <= mData.GetUpperBound (0); ++x) {
					FractalComplexNumber complexPoint = new FractalComplexNumber (boundsToUse.Min.x + (x * xStep) - mCenter.x, boundsToUse.Min.y + (y * yStep) - mCenter.y);
					float iterated = Iterate (complexPoint, new FractalComplexNumber (mInitialPoint));
					float value = 1;
					if (iterated >= 0) {
						value = iterated;
					} 

					mData [x, y] = value;
				}
			}
			mMutex.WaitOne ();
			mFinishedThreadCount++;
			mMutex.ReleaseMutex ();
		}

		public System.Collections.IEnumerator UnityRefreshData ()
		{
			ComputeBounds boundsToCompute = new ComputeBounds () {
				Max = new FractalVector2 (mMaxX, mMaxY),
				Min = new FractalVector2 (mMinX, mMinY),
			};
			float xStep = (boundsToCompute.Max.x - boundsToCompute.Min.x) / (mData.GetUpperBound (0) + 1);
			float yStep = (boundsToCompute.Max.y - boundsToCompute.Min.y) / (mData.GetUpperBound (1) + 1);
			float[,] dataToReturn = (float[,])mData.Clone ();
			for (int y = 0; y < dataToReturn.GetUpperBound(1) + 1; ++y) {
				for (int x = 0; x <= dataToReturn.GetUpperBound (0); ++x) {
					FractalComplexNumber complexPoint = new FractalComplexNumber (boundsToCompute.Min.x + (x * xStep) - mCenter.x, boundsToCompute.Min.y + (y * yStep) - mCenter.y);
					float iterated = Iterate (complexPoint, new FractalComplexNumber (mInitialPoint));
					float value = 1;
					if (iterated >= 0) {
						value = iterated;
					} 
					dataToReturn [x, y] = value;
					yield return null;
				}
			}
			InvokeDataGeneratedComplete ();
		}
	}
}

