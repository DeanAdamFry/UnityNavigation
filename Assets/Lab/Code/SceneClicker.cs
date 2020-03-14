using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class SceneClicker : MonoBehaviour
{


    Camera mCamera;

    [SerializeField]
    LayerMask ValidLayers;

    void Start()
    {
        mCamera = GetComponent<Camera>();
        Debug.AssertFormat(mCamera != null, "Must be attached to camera");
    }


    // Update is called once per frame
    void Update()
    {
        RaycastHit tHit;
        Ray tRay = mCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(tRay, out tHit,100, ValidLayers))
        {
            IClickable tInterface = (IClickable)tHit.collider.GetComponent(typeof(IClickable)); //Find Clickable object Interface
            if (tInterface != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    tInterface.Clicked(tRay, tHit, true);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    tInterface.Clicked(tRay, tHit, false);
                }
                else
                {
                    tInterface.Hover(tRay, tHit);
                }
            }
            else //Player clicked somewhere else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    AgentBase[] tAgents = FindObjectsOfType<AgentBase>(); //Get all the agents in the scene
                    foreach (AgentBase tFoundAgent in tAgents)
                    {
                        if (tFoundAgent.Selected)   //Command selected ones
                        {
                            tFoundAgent.SetDestination(tHit.point);
                        }
                    }
                }
            }
        }
    }
}
