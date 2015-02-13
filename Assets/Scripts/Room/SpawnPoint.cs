using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour 
{

	public static int layer = 17;
	public int index = 0;

	public bool spawnIn3D = false;
	public DrawingCanvas.LockType lockType;

	public bool hasPencil = false;
	public bool hasCharcoal = false;

	public bool isLatestSpawn = false;
	void OnTriggerEnter()
	{
		Room.instance.ResetSpawnPoints();
		isLatestSpawn = true;

		if( Room.instance.Save( index ) )
			CheckpointText.instance.Show();
	}
}
