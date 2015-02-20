using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Ink/Pen")]
public class Pen : Ink {

	public static int layer = 11;

	public override Type type {
		get { return Type.PEN; }
	}

	public float mass = 1;

	public override void AddChild ( Ink child )
	{
		children.Add( child );
		child.parent = this;
		child.transform.parent = transform;
	}

	public override bool paused {
		get {
			return base.paused;
		}
		set {
			if( isRoot )
				base.paused = value;
		}
	}

	public override void OnCreated ()
	{
		if( isRoot ) {
			AttachRigidbody();
		}
	}

	public override void OnDrawEnd() {
		float mass = SumMass();

		if( isRoot ) {
			paused = true;
			rigidbody.mass = mass;
		}
	}

	public override bool ConnectTo (Connector connector)
	{
		if( connector.type == Type.CHARCOAL || connector.type == Type.DEFAULT )
			return false;

		if( connector.type == Type.PEN ) {
			Pen other = connector as Pen;

			if( other.root == root )
				return false;

			Destroy ( root.rigidbody );
			other.root.AddChild( root );
			return true;
		} else if( connector.type == Type.PENCIL ) {
			Pencil pencil = connector as Pencil;
			pencil.ConnectTo( this );
			return true;
		}

		return false;
	}

	protected float SumMass() {
		float mass = this.mass;

		foreach( Ink child in children )
			mass += (child as Pen).SumMass();

		return mass;
	}
}
