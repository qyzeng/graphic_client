using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using WP.Stat;

namespace WP.Stat
{
	public interface IDamageable
	{
		void TakeDamage (GameObject attacker, params StatValueEventArgs[] args);
	}

	public class StatValueEventArgs : System.EventArgs
	{
		public string Id {
			get {
				return _id;
			}
		}
		public object Value {
			get {
				return _value;
			}
		}
        
		private string _id;
		private object _value;
        
		public void SetID (string id)
		{
			_id = id;
		}
        
		public void SetValue (object value)
		{
			_value = value;
		}
        
	}
    
	public class BaseStatHolder : MonoBehaviour
	{
		protected Dictionary<string,BaseDynamicStat> _DynamicStats = new Dictionary<string,BaseDynamicStat> ();
            
		public delegate void StatValueChangeHandler (object sender,StatValueEventArgs e);
            
		protected Dictionary<string, List<StatValueChangeHandler>> _StatDelegateList = new Dictionary<string, List<StatValueChangeHandler>> ();
        
		public virtual object GetStatValue (string id)
		{
			if (!HasDynamicStats) {
				return null;
			}
			if (_DynamicStats.ContainsKey (id)) {
				return _DynamicStats [id].Value;
			}
			return null;
		}

		public T GetStatValue<T> (string id)
		{
			return (T)((GetStatValue (id) == null) ? default(T) : GetStatValue (id));
		}
        
		public virtual string GetStatDescription (string id)
		{
			if (!HasDynamicStats) {
				return"";
			}
			if (_DynamicStats.ContainsKey (id)) {
				return _DynamicStats [id].Description;
			}
			return "";
		}

		public void SetStat (BaseDynamicStat stat)
		{
			if (_DynamicStats.ContainsKey (stat.Name)) {
				_DynamicStats [stat.Name] = stat;
			} else {
				_DynamicStats.Add (stat.Name, stat);
			}
		}
        
		public virtual void SetStat (string id, object value, string desc = null)
		{
			BaseDynamicStat statToSet = null;
			if (_DynamicStats.ContainsKey (id)) {
				statToSet = _DynamicStats [id];
			}
			if (statToSet == null) {
				statToSet = AddNewStat (id, value);
				if (_StatDelegateList.ContainsKey (id)) {
					StatValueEventArgs arg = new StatValueEventArgs ();
					arg.SetID (id);
					arg.SetValue (value);
					foreach (StatValueChangeHandler delegateFunc in _StatDelegateList[id]) {
						delegateFunc (this, arg);
					}
				}
			}
			statToSet.Description = desc != null ? desc : statToSet.Description;
			if (statToSet.Value != value) {
				statToSet.Value = value;
				if (_StatDelegateList.ContainsKey (id)) {
					StatValueEventArgs arg = new StatValueEventArgs ();
					arg.SetID (id);
					arg.SetValue (value);
					foreach (StatValueChangeHandler delegateFunc in _StatDelegateList[id]) {
						delegateFunc (this, arg);
					}
				}
			}
		}
        
		public virtual void SetStatDescription (string id, string desc)
		{
			if (_DynamicStats.ContainsKey (id)) {
				_DynamicStats [id].Description = desc;
			}
		}

		public virtual void AddToStat (string id, object value)
		{
			object currentVal = GetStatValue (id);
			if (currentVal != null) {
				System.Type statType = currentVal.GetType ();
				if (!statType.Equals (value.GetType ())) {
					throw new System.Exception ("Invalid Comparison in CharacterStats.Compare: Type of the compared objects are different");
				}
				object newVal = currentVal;
				if (statType.Equals (typeof(int))) {
					newVal = (int)currentVal + (int)value;
				}
				if (statType.Equals (typeof(float))) {
					newVal = (float)currentVal + (float)value;
				}
				SetStat (id, newVal);
			}
		}

		public virtual void SubtractFromStat (string id, object value)
		{
			object currentVal = GetStatValue (id);
			if (currentVal != null) {
				System.Type statType = currentVal.GetType ();
				if (!statType.Equals (value.GetType ())) {
					throw new System.Exception ("Invalid Comparison in CharacterStats.Compare: Type of the compared objects are different");
				}
				object newVal = currentVal;
				if (statType.Equals (typeof(int))) {
					newVal = (int)currentVal - (int)value;
				}
				if (statType.Equals (typeof(float))) {
					newVal = (float)currentVal - (float)value;
				}
				SetStat (id, newVal);
			}
		}
        
		public BaseDynamicStat AddNewStat (string id, object value, string desc = null)
		{
			if (_DynamicStats.ContainsKey (id)) {
				return _DynamicStats [id];
			}
			BaseDynamicStat newStat = new BaseDynamicStat ();
			newStat.Name = id;
			newStat.Value = value;
			newStat.Description = desc != null ? desc : id;
			_DynamicStats.Add (id, newStat);
			return newStat;
		}
        
		public void RemoveStat (string id)
		{
			if (_DynamicStats.ContainsKey (id)) {
				_DynamicStats.Remove (id);
			}

			if (_StatDelegateList.ContainsKey (id)) {
				_StatDelegateList [id].Clear ();
				_StatDelegateList.Remove (id);
			}
		}
        
		public void RegisterStatChangeListener (string id, StatValueChangeHandler delegateFunction)
		{
			if (!_StatDelegateList.ContainsKey (id)) {
				_StatDelegateList.Add (id, new List<StatValueChangeHandler> ());
			}
			_StatDelegateList [id].Add (delegateFunction);
		}
        
		public void RemoveStatChangeListener (string id, StatValueChangeHandler delegateFunction)
		{
			if (_StatDelegateList.ContainsKey (id)) {
				_StatDelegateList [id].Remove (delegateFunction);
			}
		}
        
		public bool HasDynamicStats {
			get {
				return _DynamicStats.Count > 0;
			}
		}
        
		protected virtual void OnDestroy ()
		{
			foreach (KeyValuePair<string,List<StatValueChangeHandler>> delegateList in _StatDelegateList) {
				delegateList.Value.Clear ();
			}
			_StatDelegateList.Clear ();
		}
	}
}
