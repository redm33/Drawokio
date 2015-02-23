using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Ink : Connector {

	/**
	 * Drawing Parameters
	 */
	public float connectRadius = 1;
	public float distanceBetweenNodes = 1;

	public bool connects = true;

	public AudioClip drawingSound;

	[HideInInspector]
	public bool pinned = false;

	/**
	 * Timeout
	 */
	public bool timesOut = false;
	public float baseTimeout = 5.0f;
	public float timeoutIncrement = 0.1f;
	public float timeoutRemaining = 0;

	protected override void OnUpdate ()
	{
		base.OnUpdate ();

		if( timesOut && !paused && (timeoutRemaining -= Time.deltaTime) < 0 ) {
			Erase();
		}
	}

	/**
	 * Various Drawing Events
	 */
	public virtual void OnCreated() {}
	public virtual void OnDrawEnd() {}

	public virtual void Erase() { 
		if( parent != null ) {
			parent.children.Remove( this );
		}

		foreach( Ink child in children ) {
			if( child != null )
				child.transform.parent = Drawer.instance.drawingParent;
		}

		Destroy ( gameObject ); 
	}

	/**
	 * Parent/children.
	 */
	[HideInInspector]
	public List<Ink> children;
	public Ink parent;

	public Ink root {
		get {
			Ink ret = this;
			while( ret.parent != null )
				ret = ret.parent;
			return ret;
		}
	}
	public bool isRoot {
		get { return ( parent == null ); }
	}
	
	public abstract void AddChild( Ink child );

	/**
	 * Pausing
	 */
	protected Vector3 pauseVelocity;
	protected Vector3 pauseRotVelocity;

	protected bool _paused = true;
	public virtual bool paused {
		get { return _paused; }
		set {
			_paused = value;

            if (rigidbody != null)
            {
                if (value)
                {
                    pauseVelocity = rigidbody.velocity;
                    pauseRotVelocity = rigidbody.angularVelocity;
                }

                rigidbody.useGravity = (!pinned && !paused);
                rigidbody.isKinematic = (pinned || paused);

                if (!rigidbody.isKinematic)
                {
                    rigidbody.velocity = pauseVelocity;
                    rigidbody.angularVelocity = pauseRotVelocity;
                }
            }

			foreach( Ink child in children )
				if( child != null )
					child.paused = value;
		}
	}

	/**
	 * Connecting to objects
	 */
	public virtual bool ConnectTo( Connector connector ) { return false; }

	/**
	 * Rigidbodies
	 */
	public void AttachRigidbody() {
		if( rigidbody == null )
			gameObject.AddComponent<Rigidbody>();


		PrepRigidbody();
	}

	protected virtual void PrepRigidbody() {
		if( rigidbody == null ) {
			Debug.LogWarning( "Attempted to prep non-existant rigidbody - creating new one!" );
			AttachRigidbody();
		}

		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;

	}
}
