using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649  //Stop the warning about no assignment, as it will be assigned in IDE

public class WayPointManager : MonoBehaviour
{

    [SerializeField]
    Waypoint[] Waypoints;

    int mCurrent = 0;

    public Vector3 NextWaypoint()
    {
        Vector3 tDestination = Vector3.zero; //Default
        if (Waypoints.Length > 0)
        {

            tDestination = Waypoints[mCurrent].transform.position;
            mCurrent = (mCurrent + 1) % Waypoints.Length; //Next waypoint & loop at end of array
        }
        return tDestination;
    }
}
