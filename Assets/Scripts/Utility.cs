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
}
