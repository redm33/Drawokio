using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Game/Ink/Pencil")]
public class Pencil : Ink {

	public LineRenderer lineRenderer;

	public static int layer = 12;
    public static int inkAmount = 150;
    public bool climbable = true;

	public override Type type {
		get {
			return Type.PENCIL;
		}
	}

	public override void OnCreated ()
	{
		base.OnCreated ();

		PrepRigidbody();

	}

	protected override void OnLateUpdate ()
	{
		base.OnLateUpdate();

		if( isRoot && children.Count > 0 ) 
        {
			lineRenderer.enabled = true;

			int i = 1;

			lineRenderer.SetPosition(0,transform.position);

			Ink child = children[0];
			while( child != null ) 
            {
				lineRenderer.SetVertexCount(++i);

				lineRenderer.SetPosition(i-1, child.transform.position);

				if( child.children.Count > 0 )
					child = child.children[0];
				else
					child = null;
			}
		} 
        else 
        {
			lineRenderer.enabled = false;
		}
	}

	public override void AddChild( Ink child )
	{
		BuildJoint( child.rigidbody );
		BuildJointOn( child.gameObject, rigidbody );

		children.Add( child );
		child.parent = this;
		child.transform.parent = transform;
	}

	public override bool ConnectTo( Connector other ) {

        if( other.type == Type.CHARCOAL)
            return false;

		if( other == this )
			return false;

		if( other.type == Type.PENCIL && ( parent == (other as Ink) || children.Contains( other as Ink ) ) )
			return false;

		BuildJoint( other.rigidbody );
        if (other.type != Connector.Type.DEFAULT)
        {
            BuildJointOn(other.gameObject, rigidbody);
            Debug.Log("Pen Connected");
        }
		return true;
	}
	
	public override void OnDrawEnd() {
		base.OnDrawEnd();
		foreach( Ink child in children )
			child.OnDrawEnd();
	}

	//Joint Settings
	public Vector3 swingAxis = Vector3.forward;
	public Vector3 twistAxis = Vector3.right;
	public float lowTwistLimit = -100.0F;
	public float highTwistLimit = 100.0F;
	public float swing1Limit  = 20.0F;

	public CharacterJoint BuildJointOn( GameObject obj, Rigidbody connectTo ) {
		CharacterJoint ph = obj.AddComponent<CharacterJoint>();
		ph.axis = transform.forward;
		ph.swingAxis = transform.forward;
		

		SoftJointLimit limit_setter = ph.lowTwistLimit;
		limit_setter.limit = lowTwistLimit;
		ph.lowTwistLimit = limit_setter;
		
		limit_setter = ph.highTwistLimit;
		limit_setter.limit = highTwistLimit;
		ph.highTwistLimit = limit_setter;
		
		limit_setter = ph.swing1Limit;
		limit_setter.limit = swing1Limit;
		ph.swing1Limit = limit_setter;
		
		ph.connectedBody = connectTo;
		
		return ph;
	}

	public CharacterJoint BuildJoint( Rigidbody connectTo ) {
		return BuildJointOn( gameObject, connectTo );
	}
}
