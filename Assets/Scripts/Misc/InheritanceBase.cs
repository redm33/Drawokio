using UnityEngine;
using System.Collections;

/**
 * If you ever use inheritance, use this as your base class, and override OnAwake, OnUpdate, etc, instead of the standard Awake/Update/whatever.
 * 
 * This is because if you use inheritance, the base class doesn't receive Update/Awake/whatever commands - only the end class does, and there's no way to call it manually.
 * This class lets us call base.OnUpdate(), for example, to call the base class' OnUpdate function.
 */

[AddComponentMenu("Game/Misc/Inheritance Base")]
public class InheritanceBase : MonoBehaviour 
{
	protected virtual void OnAwake() {}
	protected virtual void OnStart() {}
	protected virtual void OnUpdate() {}
	protected virtual void OnFixedUpdate() {}
	protected virtual void OnLateUpdate() {}

	void Awake() 
    {
		OnAwake();
	}

	void Start() 
    {
		OnStart ();
	}

	void Update() 
    {
		OnUpdate ();
	}

	void FixedUpdate() 
    {
		OnFixedUpdate();
	}

	void LateUpdate() 
    {
		OnLateUpdate();
	}
}
