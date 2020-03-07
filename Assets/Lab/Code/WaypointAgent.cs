using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointAgent : AgentBase
{
    WayPointManager mWPM;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        mWPM = GetComponent<WayPointManager>();
        SetDestination(mWPM.NextWaypoint(), OnAgentResult);
        Selected = true;
    }

    //This is passed as a delegate to SetDestination, it will let us know when we are there, we get called when Agent has interesting stuff to report
    public bool OnAgentResult(AgentBase vAgent, AgentBase.Result vResult, GameObject vGO) {
        Debug.LogFormat("Agent:{0} {1}", vAgent.name, vResult);
        switch (vResult) {
            case AgentBase.Result.Arrived:
                vAgent.Selected = false; //Deselect once its at the destination or stuck
                SetDestination(mWPM.NextWaypoint(), OnAgentResult);
                return false; //We are done, so we can tell agent to stop Navigation

            case AgentBase.Result.Stuck:    //If stuck we can deal with it
            case AgentBase.Result.Aborted:
                return false;


            case AgentBase.Result.CollidedWith:
                break;
            default:
                break;
        }
        return false;
    }
}
