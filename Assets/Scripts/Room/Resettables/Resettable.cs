using UnityEngine;
using System.Collections;

public abstract class Resettable : MonoBehaviour {
	public int index = -1;

	public abstract void PerformReset();

	private bool _paused = false;
	public virtual bool paused {
		get { return _paused; }
		set {
			_paused = value;
		}
	}

	public bool pickedUp = false;
	public virtual bool isPickup {
		get {
			return false;
		}
	}
	public virtual void ForcePickup() {	pickedUp = true; }
}
