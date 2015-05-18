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
}
