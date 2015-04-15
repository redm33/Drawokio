using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour 
{

	public static int layer = 17;
	public int index = 0;

	public bool spawnIn3D = false;
	public DrawingCanvas.LockType lockType;
	private static bool spawnTutorial = false;
	public bool hasPencil = false;
	public bool hasCharcoal = false;

	public bool isLatestSpawn = false;
	void OnTriggerEnter()
	{
		SpawnPointController.instance.ResetSpawnPoints();
		isLatestSpawn = true;
		if (!spawnTutorial) {
			spawnTutorial = true;
			PopupController.QueuePopup(5, 0.0f, 5.0f);
		}
		if( Room.instance.Save( index ) ){
            FadingText.instance.QueueGeneralPopup(0, 5, "Spawnpoint Set!");
            //CheckpointText.instance.Show();
		} else {
            FadingText.instance.QueueGeneralPopup(0, 5, "Spawnpoint Set!");
            //CheckpointText.instance.Show();
		}
	}
}
