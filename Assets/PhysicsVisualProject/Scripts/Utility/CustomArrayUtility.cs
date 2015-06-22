using UnityEngine;
using System.Threading;
using System.Collections;

namespace CustomArrayUtility
{
	public static class FloatArray2dResize
	{
		private static float[,] _oldArray;
		private static float[,] _newArray;

		private static float _ratioX;
		private static float _ratioY;

		private static int _oldWidth;
		private static int _newWidth;

		private static Mutex _mutex;
		private static int _finishedCount;

		private static void ThreadedResize (float[,] data, int newWidth, int newHeight)
		{
			_oldArray = data;
			_oldWidth = data.GetUpperBound (0) + 1;
			_newArray = new float[newWidth, newHeight];
			_newWidth = newWidth;
			_ratioX = (float)(data.GetUpperBound (0)) / (float)_newWidth;
			_ratioY = (float)(data.GetUpperBound (1)) / (float)newHeight;

			int cores = Mathf.Min (SystemInfo.processorCount, newHeight);
			int slice = (int)(Mathf.Floor ((float)newHeight / cores));

			_finishedCount = 0;

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
