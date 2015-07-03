using UnityEngine;
using System.Collections;
using System.Reflection;
using WP.Stat;

namespace WP.Character
{
	public class CharacterStats : BaseStatHolder, IComparer
	{
		private float _movementSpeed = 1.0f;
		public float MovementSpeed {
			get {
				return _movementSpeed;
			}
			set {
				if (_movementSpeed != value) {
					_movementSpeed = value;
					if (_StatDelegateList.ContainsKey ("MovementSpeed")) {
						StatValueEventArgs arg = new StatValueEventArgs ();
						arg.SetID ("MovementSpeed");
						arg.SetValue (_movementSpeed);
						foreach (StatValueChangeHandler delegFunc in _StatDelegateList["MovementSpeed"]) {
							delegFunc (this, arg);
						}
					}
				}
			}
		}

		private void Awake ()
		{
		}
    
		public override object GetStatValue (string id)
		{
			PropertyInfo propertyInfo = this.GetType ().GetProperty (id, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (propertyInfo == null) {
				return base.GetStatValue (id);
			} else {
				return propertyInfo.GetValue (this, null);
			}
		}
    
		public override void SetStat (string id, object value, string desc = null)
		{
			PropertyInfo propertyInfo = this.GetType ().GetProperty (id, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (propertyInfo == null) {
				base.SetStat (id, value, desc);
			} else {
				propertyInfo.SetValue (this, value, null);
			}
		}

		public int Compare (object currentValue, object comparedValue)
		{
			if (!currentValue.GetType ().Equals (comparedValue.GetType ())) {
				throw new System.Exception ("Invalid Comparison in CharacterStats.Compare: Type of the compared objects are " + currentValue.GetType () + " and " + comparedValue.GetType ());
			}
			System.IComparable currentComparable = (System.IComparable)currentValue;
			if (currentComparable == null) {
				throw new System.Exception ("Invalid Comparison in CharacterStats.Compare: currentValue is not Comparable");
			}
			System.IComparable comparedComparable = (System.IComparable)comparedValue;
			if (comparedComparable == null) {
				throw new System.Exception ("Invalid Comparison in CharacterStats.Compare: comparedValue is not Comparable");
			}
			return currentComparable.CompareTo (comparedComparable);
		}
    
		protected override void OnDestroy ()
		{
			base.OnDestroy ();
		}
	}
}