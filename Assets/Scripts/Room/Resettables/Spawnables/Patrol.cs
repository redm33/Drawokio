using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Patrol : Spawnable 
{

	public bool startRunning = false;
	public bool running = false;

	public float speed = 1.0f;

	public enum EndBehaviour 
    {
		DESTROY,
		STOP,
		LOOP,
		RESET
	}
	public EndBehaviour endBehaviour = EndBehaviour.DESTROY;

	[System.Serializable]
	public class Stop 
    {
		public float duration = 1;
		public int step = 0;
	}
	public Stop[] stops;

	bool stopped = false;
	float stopLeft = 0;

	public int cur = 0;

	Vector3 startPos;

	void Awake()
	{
		startPos = transform.position;
		running = startRunning;
	}

	public override void PerformReset ()
	{
		transform.position = startPos;
		cur = 0;
		running = startRunning;
	}

	void FixedUpdate() 
	{
		if( stopped ) 
        {
            if ((stopLeft -= Time.fixedDeltaTime) < 0)
            {
                stopped = false;
                OnEnd();
            }
            else
                return;
		}

		if( running && !paused && cur < patrolPoints.Length )
        {
			float movement = speed * Time.fixedDeltaTime;

			Vector3 diff = patrolPoints[cur].position - transform.position;

			if( diff.sqrMagnitude < movement * movement ) 
            {
				rigidbody.MovePosition( patrolPoints[cur].position );

				cur++;

				foreach( Stop stop in stops ) 
                {
					if( stop.step == cur ) 
                    {
						stopped = true;
						stopLeft = stop.duration;
						break;
					}
				}

				if( !stopped )
					OnEnd();
			} 
            else 
            {
				rigidbody.MovePosition( transform.position + diff.normalized * movement );
			}
		}
	}

	void OnEnd()
	{
		if( cur == patrolPoints.Length ) 
        {
			switch( endBehaviour ) 
            {
			case EndBehaviour.DESTROY:
				Destroy ( gameObject );
				break;
			case EndBehaviour.LOOP:
				cur = 0;
				break;
			case EndBehaviour.RESET:
				PerformReset();
				break;
			}
		}
	}
}
