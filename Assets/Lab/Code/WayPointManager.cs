using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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

    public Vector3 RandomWaypoint()
    {
        Vector3 tDestination = Vector3.zero; //Default
        if (Waypoints.Length > 0)
        {

            tDestination = Waypoints[UnityEngine.Random.Range(0,Waypoints.Length)].transform.position;
        }
        return tDestination;
    }

    public  Vector3 NearWayPoint(Vector3 VWhere)
    {
        Vector3 tDestination = Vector3.zero; //Default
        List<Waypoint> tList = Waypoints.ToList<Waypoint>();
        tList.Sort((t1, t2) => {   //Sort Waypoints list by closest item
            Vector3 tFirst = tDestination - t1.transform.position;
            Vector3 tSecond = tDestination - t2.transform.position;
            float tDifference = tSecond.magnitude  - tFirst.magnitude ; //Smallest distance
            return Math.Sign(tDifference);
        }
);
        return tList[0].transform.position; //Return closest
    }

}
