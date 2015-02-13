using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Player/Animation Controller")]
public class PlayerAnimationController : MonoBehaviour 
{
	public Player player;
	public PlayerTransformationController transformationController;

	public new Animation animation;

	public enum State 
    {
		NONE,
		IDLE,
		WALKING,
		RUNNING,
		JUMPING,
		AIRBORNE,
		CLIMBING,
		T
	}

	private State _state = State.NONE;
	public State state 
    {
		get { return _state; }
		set 
        {
			if( _state == value )
				return;

			switch( _state ) 
            {
			case State.IDLE:
			case State.WALKING:
			case State.RUNNING:
			case State.JUMPING:
			case State.AIRBORNE:
				break;
			}

			_state = value;
			
			switch( _state ) 
            {
			case State.IDLE:
				animation.CrossFade( "Idle" );
				break;
			case State.CLIMBING:
				animation.CrossFade( "Climb" );
				break;
			case State.WALKING:
				animation.CrossFade( "Walk" );
				break;
			case State.RUNNING:
				animation.CrossFade( "Run" );
				break;
			case State.JUMPING:
				animation.CrossFade( "Jump" );
				break;
			case State.AIRBORNE:
				animation.CrossFade( "Jump" );
				break;
			case State.T:
				animation.CrossFade( "T" );
				break;
			}
		}
	}

	public enum Direction 
    {
		LEFT,
		RIGHT,
		FORWARD
	}
	[HideInInspector]
	public Direction direction = Direction.RIGHT;

	void Awake() 
    {
		state = State.IDLE;
	}
	
	void Update() 
    {
		Vector3 euler;

		if( state == State.JUMPING && !animation.isPlaying )
			state = State.AIRBORNE;

		if( transformationController.in3D ) 
        {
			euler = animation.transform.localEulerAngles;
			euler.y = 0;
			animation.transform.localEulerAngles = euler;
			return;
		}

		switch( state ) 
        {
		case State.IDLE:
		case State.CLIMBING:
			euler = animation.transform.localEulerAngles;
			euler.y = 180;
			animation.transform.localEulerAngles = euler;
			break;
		case State.WALKING:
		case State.RUNNING:
		case State.AIRBORNE:
			euler = animation.transform.localEulerAngles;
			switch( direction ) 
            {
			case Direction.FORWARD:
				euler.y = 180;
				break;
			case Direction.RIGHT:
				euler.y = 90;
				break;
			case Direction.LEFT:
				euler.y = -90;
				break;
			}
			animation.transform.localEulerAngles = euler;
			break;
		case State.JUMPING:
			break;
		}
	}

	public void Pause() 
    {
		animation.Stop();
	}

	public void Start() 
    {
		State temp = state;
		state = State.NONE;
		state = temp;
	}
}
