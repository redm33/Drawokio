using UnityEngine;
using System.Collections;

public class Eraser : Ink {
	
	public static int layer = 20;
	
	public override Type type {
		get {
			return Type.DEFAULT;
		}
	}

	public override bool paused {
		get {
			return base.paused;
		}
		set {
			// Nothing!
		}
	}

	public float duration = 0.1f;
	float startTime;

	protected override void OnAwake ()
	{
		base.OnAwake ();

		startTime = Time.time;
	}
	
	public override void AddChild ( Ink child )
	{
		child.transform.parent = Drawer.instance.drawingParent;
		children.Add( child );
	}
	
	public override void OnCreated ()
	{
		base.OnCreated ();
	}
	
	public override void OnDrawEnd() {
		//renderer.enabled = false;
		foreach( Ink child in children )
			child.OnDrawEnd();
	}

	void OnTriggerEnter( Collider other )
	{
		other.GetComponent<Ink>().Erase();
	}

	protected override void OnUpdate ()
	{
		base.OnUpdate ();

		if( Time.time - startTime > duration ) {
			Destroy ( gameObject );
		}
	}
}
