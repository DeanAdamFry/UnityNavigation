using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentClicker : MonoBehaviour
{

    Camera mCamera;

    private void Start()
    {
        mCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetDestination();
        }
    }


    void SetDestination() {
        RaycastHit tHit;
        Ray tRay = mCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(tRay, out tHit)) {
            Debug.DrawRay(tRay.origin, tRay.direction, Color.red);
            AgentBase tAgent = tHit.collider.GetComponent<AgentBase>(); //Did we hit an agent
            if (tAgent != null) {
                tAgent.Selected = !tAgent.Selected;
            } else {
                AgentBase[] tAgents = FindObjectsOfType<AgentBase>(); //Get all the agents in the scene
                foreach (AgentBase tFoundAgent in tAgents) {
                    if (tFoundAgent.Selected)   //Command selected ones
                    {
                        tFoundAgent.SetDestination(tHit.point, OnAgentResult);
                    }
                }
            }
        }
    }


    //This is passed as a delegate to SetDestination, it will let us know when we are there, we get called when Agent has interesting stuff to report
    public bool OnAgentResult(AgentBase vAgent, AgentBase.Result vResult, GameObject vGO) {
        Debug.LogFormat("Agent:{0} {1}", vAgent.name, vResult);

        DebugText tDebug = vAgent.GetComponent<DebugText>();
        if(tDebug!=null)
        {
            tDebug.Text = vResult.ToString();
        }

        switch (vResult) {
            case AgentBase.Result.Arrived:
                vAgent.Selected = false; //Deselect once its at the destination or stuck
                return true; //We are done, so we can tell agent to stop Navigation

            case AgentBase.Result.Stuck:    //If stuck we can deal with it
            case AgentBase.Result.Aborted:
                return false;


            case AgentBase.Result.CollidedWith:
                if (vGO != null) {
                    Pickup tPickup = vGO.GetComponent<Pickup>(); //Get collided pickup
                    if (tPickup != null) {
                        vAgent.Selected = false; //If Pickup hit on route to pickup Stop
                        return true;
                    }
                }
                break;
            default:
                break;
        }
        return false;
    }
}
