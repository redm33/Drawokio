using UnityEngine;
using System.Collections;

public class SpawnPointController : MonoBehaviour {

    public static SpawnPointController instance;

    public SpawnPoint[] spawnPoints;

    // Use this for initialization
	void Awake () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// 
    /// </summary>
    public void ResetSpawnPoints() {
        foreach(SpawnPoint obj in spawnPoints)
            obj.isLatestSpawn = false;
    }

    public void SetLatestPoint(int point) {
        if(point < spawnPoints.Length) {
            spawnPoints[point].isLatestSpawn = true;
        } else {
            spawnPoints[0].isLatestSpawn = true;
        }
    }

    public void SetSpawnIndex() {
        for(int i = 0; i < spawnPoints.Length; i++) {
            spawnPoints[i].index = i;
        }
    }

    public SpawnPoint GetSpawnPoint(int point) {
        if(point < spawnPoints.Length) {
            return spawnPoints[point];
        }
        return spawnPoints[0];
    }
}
