using UnityEngine;
using System.Collections;

public static class Utility
{
	public static Texture2D SwapXY (this Texture2D textureToSwap)
	{
		Texture2D newTex = new Texture2D (textureToSwap.height, textureToSwap.width);
		Color[] newColors = new Color[textureToSwap.GetPixels ().Length];
		Color[] oldColors = newTex.GetPixels ();
		for (int y=0; y<newTex.height; ++y) {
			for (int x=0; x<newTex.width; ++x) {
				newColors [y * newTex.width + x] = oldColors [x * textureToSwap.height + y];
			}
		}
		newTex.SetPixels (newColors);
		return newTex;
	}

	public static int RoundUpToPowerTwo (this int value)
	{
		int powerTwoValue = Mathf.ClosestPowerOfTwo (value);
		return (powerTwoValue < value) ? (powerTwoValue << 1) : powerTwoValue; 
	}

	public static void ParseStringForNDimArray (string strToParse, ref int dim, ref object array, string delim = "|")
	{
		string[] parameters = strToParse.Split (delim.ToCharArray ());
		dim = int.TryParse (parameters [0], out dim) ? dim : 1;
		int[] arraySizes = new int[dim];

	}

	public static Vector3 ParseFromString (string strToParse, string seperaator = "|")
	{
		Vector3 returnVec = new Vector3 ();
		string[] datastr = strToParse.Split (seperaator.ToCharArray ());
		if (datastr.Length > 3) {
			returnVec.x = float.TryParse (datastr [0], out returnVec.x) ? returnVec.x : default(float);
			returnVec.y = float.TryParse (datastr [1], out returnVec.y) ? returnVec.y : default(float);
			returnVec.z = float.TryParse (datastr [2], out returnVec.z) ? returnVec.z : default(float);
		}

		return returnVec;
	}

	public static float[,] SwapXY (this float[,] sourceArr)
	{
		float[,] targetArr = new float[sourceArr.GetUpperBound (1) + 1, sourceArr.GetUpperBound (0) + 1];
		for (int targetY = 0; targetY<=targetArr.GetUpperBound(1); ++targetY) {
			for (int targetX=0; targetX<=targetArr.GetUpperBound(0); ++targetX) {
				targetArr [targetX, targetY] = sourceArr [targetY, targetX];
			}
		}
		return targetArr;
	}
}
