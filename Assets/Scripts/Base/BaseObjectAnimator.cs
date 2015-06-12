using UnityEngine;
using System.Collections;
using System.Reflection;

public class BaseObjectAnimator : MonoBehaviour
{
	private System.Type _animatorTypeRef = typeof(Animator);

	public delegate void AnimatorStateTransitionHandler (int layer,AnimatorTransitionInfo transition);
	/// <summary>
	/// Occurs when animator state transits.
	/// </summary>
	public event AnimatorStateTransitionHandler OnAnimatorStateTransit;

	/// <summary>
	/// The animator reference.
	/// </summary>
	protected Animator _animator = null;
    
	/// <summary>
	/// The model object. Better accessed via ModelObject if required.
	/// </summary>
	protected GameObject _modelObject = null;
    
	/// <summary>
	/// Gets or sets the model object.
	/// </summary>
	/// <value>The model object.</value>
	public GameObject ModelObject {
		get {
			return _modelObject;
		}
		set {
			if (_modelObject != value) {
				_modelObject = value;
				InitAnimator ();
			}
		}
	}
    
	/// <summary>
	/// Inits the animator.
	/// </summary>
	protected void InitAnimator ()
	{
		if (_modelObject != null) {
			_animator = _modelObject.GetComponent<Animator> ();
		}
	}
    
	/// <summary>
	/// Sets the bool. Hides the null reference.
	/// </summary>
	/// <param name="varName">Variable name.</param>
	/// <param name="val">If set to <c>true</c> value.</param>
	protected void SetBool (string varName, bool val)
	{
		if (_animator != null) {
			_animator.SetBool (varName, val);
		}
	}

	/// <summary>
	/// Gets the bool. Hides the null reference.
	/// </summary>
	/// <param name="varName"></param>
	/// <returns>false if animator is null</returns>
	protected bool GetBool (string varName)
	{
		if (_animator != null && !(Animator.StringToHash (varName) < 0)) {
			return _animator.GetBool (varName);
		}
		return false;
	}
    
	/// <summary>
	/// Sets the float. Hides the null reference.
	/// </summary>
	/// <param name="varName">Variable name.</param>
	/// <param name="val">Value.</param>
	protected void SetFloat (string varName, float val)
	{
		if (_animator != null) {
			_animator.SetFloat (varName, val);
		}
	}

	/// <summary>
	/// Gets the float. Hides the null reference.
	/// </summary>
	/// <param name="varName"></param>
	/// <returns>0 if animator is null</returns>
	protected float GetFloat (string varName)
	{
		if (_animator != null) {
			return _animator.GetFloat (varName);
		}
		return 0f;
	}
    
	/// <summary>
	/// Sets the int. Hides the null reference.
	/// </summary>
	/// <param name="varName">Variable name.</param>
	/// <param name="val">Value.</param>
	protected void SetInt (string varName, int val)
	{
		if (_animator != null) {
			_animator.SetInteger (varName, val);
		}
	}

	/// <summary>
	/// Gets the int. Hides the null reference.
	/// </summary>
	/// <param name="varName"></param>
	/// <returns>0 if animator is null</returns>
	protected int GetInt (string varName)
	{
		if (_animator != null) {
			return _animator.GetInteger (varName);
		}
		return 0;
	}
    
	/// <summary>
	/// Sets the trigger. Hides the null reference.
	/// </summary>
	/// <param name="varName">Variable name.</param>
	protected void SetTrigger (string varName)
	{
		if (_animator != null) {
			_animator.SetTrigger (varName);
		}
	}
    
	/// <summary>
	/// Determines whether this instance is in state the specified stateName in the blend layer.
	/// </summary>
	/// <returns><c>true</c> if this instance is in state the specified stateName in the blend layer; otherwise, <c>false</c>.</returns>
	/// <param name="stateName">State name.</param>
	/// <param name="layerIndex">Layer index.</param>
	protected bool IsInState (string stateName, int layerIndex = 0)
	{
		bool retValue = false;
		if (_animator != null) {
			retValue = _animator.GetCurrentAnimatorStateInfo (layerIndex).IsName (stateName);
		}
		return retValue;
	}

	/// <summary>
	/// Determines whether this instance is in state that has the tag tagName in the blend layer.
	/// </summary>
	/// <param name="tagName"></param>
	/// <param name="layerIndex"></param>
	/// <returns><c>true</c> if this instance is in state that has the tag tagName in the blend layer; otherwise, <c>false</c>.</returns>
	protected bool CurrentStateHasTag (string tagName, int layerIndex = 0)
	{
		bool retValue = false;
		if (_animator != null) {
			retValue = _animator.GetCurrentAnimatorStateInfo (layerIndex).IsTag (tagName);
		}
		return retValue;
	}

	protected bool IsCurrentAnimationEnd (int layerIndex= 0)
	{
		return _animator.GetCurrentAnimatorStateInfo (layerIndex).normalizedTime % 1 >= 0.9f;
	}

	public bool IsStateInTransition (int layer=0)
	{
		if (_animator != null) {
			return _animator.IsInTransition (layer);
		}
		return false;
	}
    
	protected virtual void Update ()
	{
		if (_animator != null) {
			for (int i=0; i<_animator.layerCount; ++i) {
				if (_animator.IsInTransition (i)) {
					if (OnAnimatorStateTransit != null) {
						OnAnimatorStateTransit (i, _animator.GetAnimatorTransitionInfo (i));
					}
				}
			}
		}
	}

	public virtual void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		if (_animator != null) {
			foreach (AnimatorControllerParameter param in _animator.parameters) {
				AnimatorControllerParameterType paramType = param.type;
				switch (paramType) {
				case AnimatorControllerParameterType.Bool:
					bool boolParam = _animator.GetBool (param.nameHash);
					stream.Serialize (ref boolParam);
					if (stream.isReading) {
						_animator.SetBool (param.nameHash, boolParam);
					}
					break;
				case AnimatorControllerParameterType.Float:
					float floatParam = _animator.GetFloat (param.nameHash);
					stream.Serialize (ref floatParam);
					if (stream.isReading) {
						_animator.SetFloat (param.nameHash, floatParam);
					}
					break;
				case AnimatorControllerParameterType.Int:
					int intParam = _animator.GetInteger (param.nameHash);
					stream.Serialize (ref intParam);
					if (stream.isReading) {
						_animator.SetInteger (param.nameHash, intParam);
					}
					break;
				}
			}
		}
	}
}
