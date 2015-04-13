using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Player/Transformation Controller")]
public class PlayerTransformationController : MonoBehaviour 
{

	public Player player;
	public PlayerMovementController movementController;
	public ParticleSystem poof3D;
	public ParticleSystem dissolveParticles;

	private static bool movementTutorial3D = false;

	private bool isHidden;

	public GameObject projectorShadow;

	public enum TransformType 
	{
		FROM_2D_TO_3D,
		FROM_3D_TO_2D,
		FROM_2D_TO_2D,
		FROM_3D_TO_3D
	};

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


		//Debug.Log ("triggered particles");
		state = State.IN_3D;

		gameObject.layer = Player.layer3D;

		rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}

	public void StartTransform( Transform target, bool to3D, bool ignoreX = false, bool ignoreY = false, bool ignoreZ = false ) 
    {
		poof3D.Play();

		if (isHidden) {
			projectorShadow.SetActive (false);
			modelTransform.gameObject.SetActive (false);
		}
		player.state = Player.State.TRANSFORMING;
		dissolveParticles.Stop ();

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
					modelTransform.gameObject.SetActive(true);
					projectorShadow.SetActive(true);
					dissolveParticles.Play();
					if (!movementTutorial3D) {
						movementTutorial3D = true;
						PopupController.QueuePopup(2, 3.0f, 5.0f);
						//PopupController.QueuePopup(3, 3.0f, 5.0f);

					}
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
					modelTransform.gameObject.SetActive(true);
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


	//Checks if an object that enters the trigger is on the layer "Transformer"
	void OnTriggerEnter( Collider other ) 
    {
		int layer = other.gameObject.layer; //Layer of the object entering the trigger

		if( layer == Transformer.layer ) //Check if the object is on the Transformer layer
        {
			Transformer transformer = other.GetComponent<Transformer>();
			if( transformer == null ) //Checks if the object isn't actually a transformer and logs a warning if true
            {
				Debug.LogWarning( "Entered a non-transformer object in the transformer layer!" );
				return;
			}
				isHidden = transformer.isHidden;

			if( state == State.IN_2D ) 
            {
				if(transformer.transformType.Equals(TransformType.FROM_2D_TO_3D) && transformer.target3D != null ) 
                {
					rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
					StartTransform( transformer.target3D, true, transformer.ignore3DX, transformer.ignore3DY, transformer.ignore3DZ );
				} else if(transformer.transformType.Equals(TransformType.FROM_2D_TO_2D) && transformer.target2D != null ) {
					movementController.ApplyLockType( transformer.lockType );
					StartTransform( transformer.target2D, false );
				}
			} 
            else if( state == State.IN_3D ) 
            {
				if(transformer.transformType.Equals(TransformType.FROM_3D_TO_2D) && transformer.target2D != null ) 
                {
					movementController.ApplyLockType( transformer.lockType );
					StartTransform( transformer.target2D, false );
				} else if(transformer.transformType.Equals(TransformType.FROM_3D_TO_3D) && transformer.target3D != null ) {
					rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
					StartTransform( transformer.target3D, true, transformer.ignore3DX, transformer.ignore3DY, transformer.ignore3DZ );
				}
			}
		}
	}
}
