using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(Airplane))]
public class PlanePoints : Editor {

    void OnSceneGUI() {
        Airplane plane = (Airplane)target;
        for(var i = 1; i < plane.planeWaypoints.Length; i++)
            if(plane.planeWaypoints[i])
                Handles.DrawLine(plane.planeWaypoints[i-1].transform.position,
                            plane.planeWaypoints[i].transform.position);
            //else
                //Handles.DrawLine(target.transform.position, Vector3.zero);
    }
}