using UnityEngine;
using System.Collections;

public class Plane : Patrol 
{

	public CameraFollowable cameraFollowable;
	public Transform playerPosition;

	void OnTriggerEnter( Collider other ) 
    {
		Player.instance.StartTransport( playerPosition );

		CameraController.instance.AddToQueue( cameraFollowable );

		running = true;
	}

	public override void PerformReset ()
	{
		if( running ) 
			Player.instance.state = Player.State.WALKING;

		CameraController.instance.RemoveFromQueue( cameraFollowable );

		base.PerformReset ();
	}
}
