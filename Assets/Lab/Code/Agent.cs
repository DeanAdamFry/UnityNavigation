using System; //Needed for call back Action
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Agent : MonoBehaviour
{

    NavMeshAgent        mNMA; //Cache NavMeshAgent

    MeshRenderer mMR; //Cache MeshRenderer

    [SerializeField]
    Color SelectedColour = Color.yellow;

    Color mDefaultColor;


    Text DebugText;

    // Start is called before the first frame update
    void Start()
    {

        mNMA = GetComponent<NavMeshAgent>(); //If we have one use it 
        {
            mNMA = gameObject.AddComponent<NavMeshAgent>(); //If not add one
        }

        mMR = GetComponent<MeshRenderer>(); //So we can select the peep

        Rigidbody tRB = GetComponent<Rigidbody>();  //For Collision we need a RB
        if(tRB==null)
        {
            tRB = gameObject.AddComponent<Rigidbody>();
        }
        tRB.isKinematic = true; //Make it Kinematic so we can driver

        mDefaultColor = mMR.material.color; //Save Default colour

        Selected = false;   //Set to default

        DebugText = GetComponentInChildren<Text>();

    }

    bool mSelected = false;
    public  bool    Selected {      //Set Agent as Selected/Deslected also change colour
        get 
        {        
            return mSelected;
        }
        set 
        {
            mSelected = value;
            mMR.material.color = (mSelected) ? SelectedColour : mDefaultColor;  // ? is a shorthand Shorthand for if 
        }
    }

    public  void SetDestination(Vector3 vPosition)
    { 
        mNMA.SetDestination(vPosition);
    }


    public void SetDestination(Vector3 vPosition, Action vArrived)
    {
        mNMA.SetDestination(vPosition);

    }

    private void Update()
    {
        DebugText.text = string.Format("{0}", mNMA.pathStatus);
        DebugText.text += "\n";
        DebugText.text += string.Format("{0:f2}", mNMA.remainingDistance);

    }

    private void LateUpdate()
    {//Make Canvas face camera
        DebugText.transform.parent.LookAt(2 * transform.position - Camera.main.transform.position);

    }
}
