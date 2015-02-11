using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : Resettable {

	public Transform[] patrolPoints;

	public Spawnable spawnablePrefab;
	private List<Spawnable> spawnables = new List<Spawnable>();

	public float interval = 1;
	float elapsed = 0;

	public override bool paused {
		get {
			return base.paused;
		}
		set {
			base.paused = value;

			foreach( Spawnable obj in spawnables )
				if( obj != null )
					obj.paused = value;
		}
	}

	public override void PerformReset ()
	{
		foreach( Spawnable obj in spawnables ) {
			if( obj != null )
				Destroy ( obj.gameObject );
		}
		spawnables = new List<Spawnable>();

		elapsed = interval;
	}

	void Awake()
	{
		elapsed = interval;
	}

	void Update()
	{
		List<Spawnable> temp = spawnables;
		spawnables = new List<Spawnable>();
		foreach( Spawnable obj in temp ) {
			if( obj != null )
				spawnables.Add( obj );
		}

		if( !paused ) {
			elapsed += Time.deltaTime;

			if( elapsed > interval ) {
				elapsed -= interval;

				Spawnable spawnable = Instantiate( spawnablePrefab, transform.position, spawnablePrefab.transform.rotation ) as Spawnable;
				spawnables.Add( spawnable );

				spawnable.patrolPoints = patrolPoints;
			}
		}
	}
}
