using System;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

namespace FractalLibrary
{
	public delegate void ArrayResizeCompleteHandler (System.Object obj);

	public static class FloatArray2dResize
	{
		private class Array2dWork
		{
			public float[,] ArrayToResize;
			public int TargetHeight;
			public int TargetWidth;
			public ArrayResizeCompleteHandler CompleteHandler;
		}

		private static float[,] _oldArray;
		private static float[,] _newArray;

		private static float _ratioX;
		private static float _ratioY;

		private static int _oldWidth;
		private static int _newWidth;

		private static Mutex _mutex = null;
		private static int _finishedCount;

		private static BackgroundWorker _worker = new BackgroundWorker ();
		private static Queue<Array2dWork> _workerQueue = new Queue<Array2dWork> ();
		private static int _processorCount = 1;

		public static void Resize (float[,] data, int targetWidth, int targetHeight, int coresToUse, ArrayResizeCompleteHandler completeHandler = null)
		{
			_processorCount = coresToUse;
			Array2dWork work = new Array2dWork (){
				ArrayToResize = (float[,])(data.Clone()),
				TargetHeight = targetHeight,
				TargetWidth = targetWidth,
			};
			work.CompleteHandler = completeHandler;
			if (_worker.IsBusy) {
				_workerQueue.Enqueue (work);
			} else {
				_worker.DoWork += WorkerResize;
				_worker.RunWorkerCompleted += WorkerComplete;
				_worker.RunWorkerAsync (work);
			}
		}

		private static void WorkerComplete (object sender, RunWorkerCompletedEventArgs args)
		{
			_worker.RunWorkerCompleted -= WorkerComplete;
		}

		private static void WorkerResize (object sender, DoWorkEventArgs args)
		{
			Array2dWork workargs = (Array2dWork)(args.Argument);
			ProceesArray2dWork (workargs);
			while (_workerQueue.Count>0) {
				workargs = _workerQueue.Dequeue ();
				ProceesArray2dWork (workargs);
			}
		}

		private static void ProceesArray2dWork (Array2dWork work)
		{
			ThreadedResize (ref work.ArrayToResize, work.TargetWidth, work.TargetHeight);
			if (work.CompleteHandler != null) {
				work.CompleteHandler (work.ArrayToResize);
			}
		}

		private static void ThreadedResize (ref float[,] data, int newWidth, int newHeight)
		{
			if (_mutex == null) {
				_mutex = new Mutex (false);
			}

			_oldArray = (float[,])data.Clone();
			_oldWidth = _oldArray.GetUpperBound (0) + 1;
			_newArray = new float[newWidth, newHeight];
			_newWidth = newWidth;
			_ratioX = (float)(data.GetUpperBound (0)) / (float)_newWidth;
			_ratioY = (float)(data.GetUpperBound (1)) / (float)newHeight;

			int coresToUse = _processorCount > 1 ? _processorCount : 1;
			int cores = Math.Min (coresToUse, newHeight);
			int slice = (int)(Math.Floor ((float)newHeight / cores));

			_finishedCount = 0;

			for (int i = 0; i < cores-1; ++i) {
				ArrayResizeThreadData td = new ArrayResizeThreadData (slice * i, slice * (i + 1));
				ParameterizedThreadStart threadStart = new ParameterizedThreadStart (ResizeFuntion);
				Thread newThread = new Thread (threadStart);
				newThread.Start (td);
			}
			ArrayResizeThreadData lastSlice = new ArrayResizeThreadData (slice * (cores - 1), newHeight);
			ResizeFuntion (lastSlice);
			while (_finishedCount < cores) {
				Thread.Sleep (1);
			}
			data = _newArray;
		}

		private static void ResizeFuntion (System.Object obj)
		{
			ArrayResizeThreadData td = (ArrayResizeThreadData)obj;
			int oldArrayWidth = _oldArray.GetUpperBound (0) + 1;
			for (int y = td.Start; y < td.End; ++y) {
				int yFloor = (int)(y * _ratioY);
				int yCeil = (int)Math.Ceiling (y * _ratioY);
				for (int x = 0; x <= _newArray.GetUpperBound(0); ++x) {
					int xFloor = (int)(x * _ratioX);
					int xCeil = (int)Math.Ceiling (x * _ratioX);
					float xLerp = (x * _ratioX) - (float)xFloor;
					_newArray [x, y] = Lerp(
						Lerp (_oldArray [xFloor, yFloor], _oldArray [xCeil, yFloor], xLerp),
						Lerp (_oldArray [xFloor, yCeil], _oldArray [xCeil, yCeil], xLerp),
						(y * _ratioY) - (float)yFloor);
				}
			}
			_mutex.WaitOne ();
			++_finishedCount;
			_mutex.ReleaseMutex ();
		}

		private static float Lerp(float v1, float v2, float lerpVal)
		{
			float lerp = lerpVal > 1f ? 1f : lerpVal;
			lerp = lerp < 0f ? 0f : lerp;
			return v1 + ((v2 - v1) * lerp);
		}
	}

	public class ArrayResizeThreadData
	{
		public int Start;
		public int End;
		public ArrayResizeThreadData (int start, int end)
		{
			Start = start;
			End = end;
		}
	}
}

