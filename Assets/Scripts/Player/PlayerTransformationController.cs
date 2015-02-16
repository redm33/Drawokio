using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Player/Transformation Controller")]
public class PlayerTransformationController : MonoBehaviour 
{

	public Player player;
	public PlayerMovementController movementController;
	public ParticleSystem poof3D;

	public enum State 
    {
		IN_2D,
		TRANSFORMING_2D_TO_3D,
		IN_3D,
		TRANSFORMING_3D_TO_2D
	};
	public State state { get; private set; }

	public bool isTransforming 
    {
		get { return ( state == State.TRANSFORMING_2D_TO_3D || state == State.TRANSFORMING_3D_TO_2D ); }
	}
	public bool in2D 
    {
		get { return ( state == State.IN_2D ); }
	}
	public bool in3D 
    {
		get { return ( state == State.IN_3D ); }
	}

	public float transformationSpeed = 1.0f;
	float transformationProgress = 0;
	
	public Transform modelTransform;
	public Vector3 modelScale2D = new Vector3(1, 1, 0.01f);
	public Vector3 modelPosition2D = new Vector3(0, 0, -.006f);

	Vector3 startPos;
	Quaternion startRot;
	Vector3 targetPos;
	Quaternion targetRot;

	public void Become2D( DrawingCanvas.LockType lockType )
	{
		state = State.IN_2D;
	}

	public void Become3D()
	{
		poof3D.Play ();
		Debug.Log ("triggered particles");
		state = State.IN_3D;

		gameObject.layer = Player.layer3D;

		rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}

	public void StartTransform( Transform target, bool to3D, bool ignoreX = false, bool ignoreY = false, bool ignoreZ = false ) 
    {
		player.state = Player.State.TRANSFORMING;

		targetPos = target.position;
		if( to3D ) 
        {
			if( ignoreX )
				targetPos.x = transform.position.x;
			if( ignoreY )
				targetPos.y = transform.position.y;
			if( ignoreZ )
				targetPos.z = transform.position.z;
		}
		targetRot = target.rotation;

		startPos = transform.position;
		startRot = transform.rotation;

		if( to3D ) 
        {
			transformationProgress = 0;
			state = State.TRANSFORMING_2D_TO_3D;
		} 
        else 
        {
			transformationProgress = 1;
			state = State.TRANSFORMING_3D_TO_2D;
		}
	}

	void FixedUpdate() 
    {
		switch( state ) 
        {
		    case State.IN_2D:
			    transformationProgress = 0;
			    UpdateTransform();
			    break;
		    case State.IN_3D:
			    transformationProgress = 1;
			    UpdateTransform();
			    break;
		    case State.TRANSFORMING_2D_TO_3D:
				
				if(!poof3D.isPlaying) {
				//poof3D.Play();
				}

			    transformationProgress += Time.fixedDeltaTime*transformationSpeed;
			    UpdateTransform();
			    UpdatePosition();

			    if( transformationProgress >= 1 ) 
                {
				    gameObject.layer = Player.layer3D;
				    state = State.IN_3D;
				    player.state = Player.State.WALKING;
			    }
			    break;
		    case State.TRANSFORMING_3D_TO_2D:
			    transformationProgress -= Time.fixedDeltaTime*transformationSpeed;
			    UpdateTransform();
			    UpdatePosition( true );

			    if( transformationProgress <= 0 ) 
                {
				    gameObject.layer = Player.layer2D;
				    state = State.IN_2D;
				    player.state = Player.State.WALKING;
			    }
			    break;
		}
	}

	void UpdateTransform() 
    {
		modelTransform.localScale = Vector3.Lerp( modelScale2D, Vector3.one, transformationProgress );
		modelTransform.localPosition = Vector3.Lerp( modelPosition2D, Vector3.zero, transformationProgress );
	}

	void UpdatePosition( bool inverse = false ) 
    {
		float t = ( inverse ? 1-transformationProgress : transformationProgress );

		transform.position = Vector3.Lerp( startPos, targetPos, t );
		transform.rotation = Quaternion.Lerp( startRot, targetRot, t );
	}

	void OnTriggerEnter( Collider other ) 
    {
		int layer = other.gameObject.layer;

		if( layer == Transformer.layer ) 
        {
			Transformer transformer = other.GetComponent<Transformer>();
			if( transformer == null ) 
            {
				Debug.LogWarning( "Entered a non-transformer object in the transformer layer!" );
				return;
			}

			if( state == State.IN_2D ) 
            {
				if( transformer.target3D != null ) 
                {
					rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
					StartTransform( transformer.target3D, true, transformer.ignore3DX, transformer.ignore3DY, transformer.ignore3DZ );
				}
			} 
            else if( state == State.IN_3D ) 
            {
				if( transformer.target2D != null ) 
                {
					movementController.ApplyLockType( transformer.lockType );
					StartTransform( transformer.target2D, false );
				}
			}
		}
	}
}
