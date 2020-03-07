using System; //Needed for call back Action
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class AgentBase : MonoBehaviour
{

    protected NavMeshAgent mNMA; //Cache NavMeshAgent

    protected MeshRenderer mMR; //Cache MeshRenderer

    [SerializeField]
    Color SelectedColour = Color.yellow;

    Color mDefaultColor;

    protected DebugText mDebugText; //For Component added in code


    protected float mMinDistance = 0.1f; //Acceptable distance
    public enum Result
    {
        Stuck
        , Navigating
        , Arrived
        ,Aborted
    } ;

    public delegate    bool AgentStatusChange(Result vResult); //Action to call when we have arrived

    protected AgentStatusChange mOnAgentStatusChange;

    Coroutine mCheckProgress;   //Check progress of Agent

    void Start()
    {

        mNMA = GetComponent<NavMeshAgent>(); //If we have one use it 
        {
            mNMA = gameObject.AddComponent<NavMeshAgent>(); //If not add one
        }

        mMR = GetComponent<MeshRenderer>(); //So we can select the peep

        Rigidbody tRB = GetComponent<Rigidbody>();  //For Collision we need a RB
        if (tRB == null) //IF we dont have one, add it
        {
            tRB = gameObject.AddComponent<Rigidbody>();
        }
        tRB.isKinematic = true; //Make it Kinematic so we can driver

        mDefaultColor = mMR.material.color; //Save Default colour

        Selected = false;   //Set to default

        mDebugText = gameObject.AddComponent<DebugText>(); //Add our DebugText Component in code
    }

    bool mSelected = false;
    public bool Selected
    {      //Set Agent as Selected/Deslected also change colour
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

    public void SetDestination(Vector3 vPosition)
    {
        mNMA.SetDestination(vPosition);
    }

    public  virtual bool    hasArrived
    {
        get
        {
            if(mNMA.pathStatus == NavMeshPathStatus.PathInvalid || mNMA.pathStatus == NavMeshPathStatus.PathPartial)
            {
                return false; //Both are pathing errors
            }
            return (mNMA.remainingDistance < mMinDistance);
        }
    }

    public  virtual bool isStuck
    {
        get
        {
            return false;
        }
    }

    public void SetDestination(Vector3 vPosition, AgentStatusChange vArrived) //Set Destination on NavMesh
    {
        mNMA.SetDestination(vPosition);
        if(mCheckProgress != null)
        {
            StopCoroutine(mCheckProgress);
            mCheckProgress = null;
            if(mOnAgentStatusChange!=null)
            {
                mOnAgentStatusChange(Result.Aborted);
                mOnAgentStatusChange = null;
            }
        }
        mOnAgentStatusChange = vArrived;    //Set callback delegate
        mCheckProgress = StartCoroutine(CheckProgress());
    }

    IEnumerator CheckProgress()
    {
        for(; ; )
        {
            yield return new WaitForSeconds(0.5f); //Check every 1/2 second
            if (hasArrived)
            {
                if (mOnAgentStatusChange != null)
                {
                    mOnAgentStatusChange(Result.Arrived); //Signal issue
                    mOnAgentStatusChange = null;
                }
                break;  //Quit CoRoutine
            }
            if (isStuck)
            {
                if (mOnAgentStatusChange != null) //Is there a callback delegate
                {
                    mOnAgentStatusChange(Result.Stuck); //Signal issue
                    mOnAgentStatusChange = null;
                }
                break;  //Quit CoRoutine, as we cant recover
            }
            else
            {
                if (mOnAgentStatusChange != null) //Is there a callback delegate
                {
                    mOnAgentStatusChange(Result.Navigating); //Signal Still tracking
                    mOnAgentStatusChange = null;
                }
            }
        }
    }
}
