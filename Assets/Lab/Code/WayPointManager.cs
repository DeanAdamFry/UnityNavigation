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
    
    Waypoint mCurrentWP;


    public Vector3 NextWaypoint()
    {
        Vector3 tDestination = Vector3.zero; //Default
        if (Waypoints.Length > 0)
        {

            mCurrentWP = Waypoints[mCurrent];
            tDestination = mCurrentWP.transform.position;
            mCurrent = (mCurrent + 1) % Waypoints.Length; //Next waypoint & loop at end of array
        }
        return tDestination;
    }

    public Vector3 RandomWaypoint()
    {
        Vector3 tDestination = Vector3.zero; //Default
        if (Waypoints.Length > 0)
        {
            mCurrentWP = Waypoints[UnityEngine.Random.Range(0, Waypoints.Length)];
            tDestination = mCurrentWP.transform.position;
        }
        return tDestination;
    }

    public  Vector3 NearWayPoint(Vector3 VWhere)
    {
        Vector3 tDestination = Vector3.zero; //Default
        List<Waypoint> tList = Waypoints.ToList<Waypoint>();
        tList.Sort((t1, t2) => {   //Sort Waypoints list by closest item using Lamda function which is called for all items in the list to decide if to swap
            Vector3 tFirst = tDestination - t1.transform.position; //Distance to Destination from first one
            Vector3 tSecond = tDestination - t2.transform.position; //Distance to Destination from second one
            float tDifference = tSecond.magnitude  - tFirst.magnitude ; // +ve means 1st bigger than 2nd -ve means 1st smaller than second 
            return Math.Sign(tDifference);
        }
);
        foreach(Waypoint tNearWp in Waypoints) //return closest waypoint other than the one we are on
        {
            if(tNearWp!=mCurrentWP)
            {
                mCurrentWP = tNearWp;
                return mCurrentWP.transform.position;
            }
        }
        return Vector3.zero;
    }


}
