using UnityEngine;
using System.Collections;

public class UnrollingPoster : Resettable 
{

	public float resetTime = 5;
	public float timeElapsed = 0;

	private enum State 
    {
		WAITING,
		UNROLLING,
		ROLLING
	}
	private State state = State.WAITING;

	public new Animation animation;
	public Transform transporter;
	public CameraFollowable followable;

	public override bool paused 
    {
		get 
        {
			return base.paused;
		}
		set 
        {
			base.paused = value;

			if( paused )
				animation.Stop();
			else
				animation.Play();
		}
	}

	public override void PerformReset ()
	{
		animation.Rewind();
		animation.Stop();

		animation["Take 001"].speed = 1;

		state = State.WAITING;
	}

	void OnTriggerEnter( Collider other ) 
    {
		if( state != State.WAITING )
			return;

		state = State.UNROLLING;
		timeElapsed = 0;
		animation.Play();

		Player.instance.StartTransport( transporter );
		CameraController.instance.AddToQueue( followable );
	}

	void Update() 
    {
		if( state == State.WAITING )
			return;

		if( (timeElapsed+=Time.deltaTime) > resetTime ) 
        {
			if( state == State.UNROLLING ) {
				state = State.ROLLING;
				timeElapsed = 0;
				animation["Take 001"].speed = -1;
				animation.Play();
				Player.instance.state = Player.State.WALKING;
				CameraController.instance.RemoveFromQueue( followable );
			} 
            else 
            {
				state = State.WAITING;
				animation["Take 001"].speed = 1;
				animation.Stop();
			}
		}
	}
}
