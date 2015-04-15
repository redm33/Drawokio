using UnityEngine;
using System.Collections;

public class DiscoverableAreasController : MonoBehaviour {

    public static DiscoverableAreasController instance;
    public Resettable[] discoverableAreas; // The discoverable zones in the room.

	void Awake () {
        instance = this;
	}

    public void SetAreaIndex() {
        for(int i = 0; i < discoverableAreas.Length; i++) {
            discoverableAreas[i].index = i;
        }
    }

    public int GetNumAreas() {
        return discoverableAreas.Length;
    }

    public Resettable GetDiscoverableArea(int area) {
        return discoverableAreas[area];
    }
}
