using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerClimbingController : MonoBehaviour 
{
	public Player player;
	public PlayerMovementController movementController;
	public PlayerAnimationController animationController;

	public float climbSpeed = 10;
	public float climbSwing = 10;
	
	public Transform climbCenter;
	
	Vector3 climbStartPos = Vector3.zero;
	float climbStartTime = 0;
	float climbShiftTime = 0;

	private List<Pencil> climbables = new List<Pencil>();

	void FixedUpdate()
    {
		if( player.paused )
			return;

		if( player.state != Player.State.CLIMBING ) 
        {
			CheckClimbing();
			return;
		}

		List<Pencil> temp = climbables;
		climbables = new List<Pencil>();
		foreach( Pencil pencil in temp ) 
        {
			if( pencil != null )
				climbables.Add( pencil );
		}

        if (Input.GetButton("Jump"))
        {
			player.state = Player.State.WALKING;

			rigidbody.velocity = transform.right * Input.GetAxis("Horizontal") * player.movementController.speed;
			movementController.Jump();

            lastJump = Time.time;
            return;
        }

        rigidbody.velocity = Vector3.zero;

        if (currentClimbable == null)
        {
            if (climbables.Count > 0)
				currentClimbable = closestClimbable;
            else
                currentClimbable = null;
        }

        if (currentClimbable == null)
        {
			player.state = Player.State.WALKING;
        }
        else
        {
            currentClimbable.rigidbody.AddForce( transform.right * Input.GetAxis("Horizontal") );

			float t =  ( Time.time - climbStartTime ) / climbShiftTime;

			Vector3 target = Vector3.Lerp( climbStartPos, currentClimbable.transform.position, t );
			rigidbody.MovePosition( transform.position + ( target - climbCenter.position ) );

			if( t > 1 ) {
				float climbInput = Input.GetAxis("Vertical");
				Pencil target2 = null;
				if( climbInput > 0 )
					target2 = bestClimbUp;
				else if( climbInput < 0 )
					target2 = bestClimbDown;
				if( target2 != null )
					currentClimbable = target2;
			}

            animationController.state = PlayerAnimationController.State.CLIMBING;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Pencil.layer)
        {
			climbables.Add( other.GetComponent<Pencil>() );
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == Pencil.layer)
        {
			Pencil obj = other.GetComponent<Pencil>();
			if( obj == null )
				return;

            climbables.Remove(obj);

            if (obj == currentClimbable)
                currentClimbable = null;
        }
    }

	private Pencil _currentClimbable = null;
	private Pencil currentClimbable {
		get {return _currentClimbable;}
		set {
			_currentClimbable = value;

			if( currentClimbable == null )
				return;

			climbStartPos = climbCenter.position;

			climbStartTime = Time.time;
			climbShiftTime = ( climbStartPos - currentClimbable.transform.position ).magnitude / climbSpeed;
			climbShiftTime = Mathf.Max( 0.1f, climbShiftTime );
		}
	}

	private Pencil closestClimbable 
    {
		get 
        {
			if( climbables.Count == 0 )
				return null;

			Pencil ret = null;
			float nearestDist = 0;
			foreach( Pencil pencil in climbables ) 
            {
				if( pencil == null )
					continue;

				float sqrDist = ( climbCenter.position - pencil.transform.position ).sqrMagnitude;
				if( ret == null || sqrDist < nearestDist ) 
                {
					ret = pencil;
					nearestDist = sqrDist;
				}
			}

			return ret;
		}
	}

	private Pencil bestClimbUp 
    {
		get 
        {
			if( currentClimbable == null )
				return null;

			Pencil ret = null;
			float xOffset = 0;
			if( currentClimbable.parent != null && currentClimbable.parent.transform.position.y > currentClimbable.transform.position.y ) 
            {
				ret = currentClimbable.parent as Pencil;
				xOffset = Mathf.Abs( ret.transform.position.x - currentClimbable.transform.position.x );
			}

			foreach( Ink child in currentClimbable.children ) 
            {
				if( child.transform.position.y > currentClimbable.transform.position.y ) 
                {
					float curXOffset = Mathf.Abs( child.transform.position.x - currentClimbable.transform.position.x );
					if( ret == null || curXOffset < xOffset ) 
                    {
						ret = child as Pencil;
						xOffset = curXOffset;
					}
				}
			}

			return ret;
		}
	}

	private Pencil bestClimbDown 
    {
		get {
			if( currentClimbable == null )
				return null;
			
			Pencil ret = null;
			float xOffset = 0;
			if( currentClimbable.parent != null && currentClimbable.parent.transform.position.y < currentClimbable.transform.position.y ) 
            {
				ret = currentClimbable.parent as Pencil;
				xOffset = Mathf.Abs( ret.transform.position.x - currentClimbable.transform.position.x );
			}
			
			foreach( Ink child in currentClimbable.children ) 
            {
				if( child.transform.position.y < currentClimbable.transform.position.y ) 
                {
					float curXOffset = Mathf.Abs( child.transform.position.x - currentClimbable.transform.position.x );
					if( ret == null || curXOffset < xOffset ) 
                    {
						ret = child as Pencil;
						xOffset = curXOffset;
					}
				}
			}
			
			return ret;
		}
	}

    float lastJump = 0;
    float climbDelay = 0.5f;

    private bool CheckClimbing()
    {
        if (Time.time - lastJump < climbDelay)
            return false;

        float climbInput = Input.GetAxis("Vertical");
        if (climbables.Count > 0) //&& climbInput != 0)
        {
            player.state = Player.State.CLIMBING;

            rigidbody.velocity = Vector3.zero;

			currentClimbable = closestClimbable;

            return true;
        }

        return false;
    }
}
