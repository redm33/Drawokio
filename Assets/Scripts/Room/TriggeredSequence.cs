using UnityEngine;
using System.Collections;

public class TriggeredSequence : Resettable 
{

	[System.Serializable]
	public class Sequence 
    {
		public float duration = 1;
		float elapsed = 0;

		public Transform toMove;
		public Transform moveTo;

		public Resettable objectToEnable;

		Vector3 startPos = Vector3.zero;
		Quaternion startRot = Quaternion.identity;

		public Animation animation;

		public CameraFollowable cameraFollowable;

		public void Start() 
        {
			elapsed = 0;

			if( toMove != null ) 
            {
				startPos = toMove.position;
				startRot = toMove.rotation;
			}

			if( animation != null )
				animation.Play();

			if( cameraFollowable != null )
				CameraController.instance.AddToQueue( cameraFollowable );

			if( objectToEnable != null )
				objectToEnable.gameObject.SetActive(!objectToEnable.pickedUp);
		}

		public bool Update(float dt) 
        {
			elapsed += dt;

			if( toMove != null ) 
            {
				float t = elapsed / duration;

				toMove.position = Vector3.Lerp ( startPos, moveTo.position, t );
				toMove.rotation = Quaternion.Lerp( startRot, moveTo.rotation, t );
			}

			return ( elapsed > duration );
		}

		public void End() 
        {
			if( cameraFollowable != null )
				CameraController.instance.RemoveFromQueue( cameraFollowable );

			if( toMove != null ) 
            {
				toMove.position = moveTo.position;
				toMove.rotation = moveTo.rotation;
			}
		}

		public void Reset() 
        {
			if( animation != null ) 
            {
				animation.Rewind();
				animation.Sample();
				animation.Stop();
			}

			if( objectToEnable != null )
				objectToEnable.gameObject.SetActive(false);
		}
	}

	public Sequence[] sequences;
	int cur = 0;

	bool triggered = false;

	public override void PerformReset ()
	{
		if( cur < sequences.Length ) 
			sequences[cur].End();

		foreach( Sequence obj in sequences ) 
			obj.Reset();

		triggered = false;
		cur = 0;
	}

	void OnTriggerEnter( Collider other ) 
    {
		if( triggered )
			return;

		triggered = true;
		sequences[0].Start();

		Player.instance.paused = true;
	}

	void Update()
	{
		if( triggered && cur < sequences.Length ) 
        {
			if( sequences[cur].Update( Time.deltaTime ) ) 
            {
				sequences[cur].End();

				if( ++cur < sequences.Length ) 
					sequences[cur].Start();
				else 
					Player.instance.paused = false;
			}
		}
	}
}
