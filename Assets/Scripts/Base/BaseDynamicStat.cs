using UnityEngine;
using System.Collections;

namespace WP.Stat
{
	[System.Serializable]
	public class BaseDynamicStat
	{

		public string Name;
		public string Description;
		public object Value;

		public enum PROPERTY_TYPES
		{
			INT,
			FLOAT,
			STRING
		}

		public PROPERTY_TYPES ValueType;
		public string ValueString;
        
		public bool ValidateStatType ()
		{
			switch (ValueType) {
			case PROPERTY_TYPES.FLOAT:
				float newFloatValue = 0f;
				if (!(float.TryParse (ValueString, out newFloatValue))) {
					return !(ThrowEditorError ("Error", "Invalid value for type selected in " + Name, "Re-Enter"));
				}
				Value = newFloatValue;
				return true;
			case PROPERTY_TYPES.INT:
				int newIntValue = 0;
				if (!(int.TryParse (ValueString, out newIntValue))) {
					return !(ThrowEditorError ("Error", "Invalid value for type selected in " + Name, "Re-Enter"));
				}
				Value = newIntValue;
				return true;
			default:
				Value = ValueString;
				return true;
			}
		}

		public bool ThrowEditorError (string header, string message, string button)
		{
#if UNITY_EDITOR
			return UnityEditor.EditorUtility.DisplayDialog (header, message, button);
#else
            return true;
#endif
		}
	}
}