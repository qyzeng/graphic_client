using System;

namespace FractalLibrary
{
	public class CircleInversionFractal : IteratingFractal
	{
		private class Circle
		{
			public FractalVector2 Center;
			public float Radius;
		}

		private Random mRandom;
			
		private int mNumberOfMainCircles = 3;
		private float mViewportExtents = 1f;

		private float mStepValue = 0.01f;

		private Circle[] mMainCircles;

		public CircleInversionFractal()
		{
			mRandom = new Random (System.DateTime.Now.Millisecond);
		}

		public void SetStepValue(float value)
		{
			mStepValue = value;
		}

		public CircleInversionFractal(int mainCircleCount)
		{
			mNumberOfMainCircles = mainCircleCount;
		}

		public void SetNumberOfCircles(int mainCircleCount)
		{
			mNumberOfMainCircles = mainCircleCount;
		}

		private void PopulateCircles()
		{
			float startAngle = (float)Math.PI * 2f / mNumberOfMainCircles;
			float radiusOfMainCircles = (float)(Math.Sin (startAngle) / Math.Sin (((float)Math.PI - startAngle) * 0.5f)) * 0.5f;
			mViewportExtents = (float)Math.Sqrt (1f - (radiusOfMainCircles * radiusOfMainCircles));
			mMainCircles = new Circle[mNumberOfMainCircles + 1];
			mMainCircles [0] = new Circle () {
				Center = new FractalVector2(0,0),
				Radius = 1f - radiusOfMainCircles,
			};

			for (int i = 0; i < mNumberOfMainCircles; ++i) {
				mMainCircles [i + 1] = new Circle () {
					Center = new FractalVector2 ((float)Math.Sin (i * startAngle), (float)Math.Cos (i * startAngle)),
					Radius = radiusOfMainCircles,
				};
			}
		}

		private void PopulateData()
		{
			FractalVector2 interpolatedPoint = mInitialPoint;
			for (int i = 0; i < Iterations; ++i) {
				int selectedCircle = mRandom.Next (mNumberOfMainCircles + 1);
				FractalVector2 delta = interpolatedPoint - mMainCircles [selectedCircle].Center;
				float deltaMagnitude = delta.Magnitude;
				FractalVector2 deltaNormalized = delta.Normalize ();
				float deltaRadius = mMainCircles [selectedCircle].Radius * mMainCircles [selectedCircle].Radius / deltaMagnitude;
				interpolatedPoint = deltaNormalized * deltaRadius + mMainCircles [selectedCircle].Center;
				int coordX = (int)(mData.GetUpperBound (0) * (interpolatedPoint.x + mViewportExtents) / (mViewportExtents * 2f));
				int coordY = (int)(mData.GetUpperBound (1) * (interpolatedPoint.y + mViewportExtents) / (mViewportExtents * 2f));
				try
				{
					mData[coordX,coordY] += mStepValue;
					mData[coordX, coordY] = (mData[coordX, coordY] >1f) ? 1f : mData[coordX,coordY];
				}
				catch(Exception) {
					continue;
				}
			}
		}

		public override void RefreshDataSamples ()
		{
			PopulateCircles ();
			PopulateData ();
			//throw new NotImplementedException ();
		}
	}
}

