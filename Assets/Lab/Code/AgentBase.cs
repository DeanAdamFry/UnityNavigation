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
        protected   Color SelectedColour = Color.yellow;

        [SerializeField]
        protected float RayLenght=5.0f;

        protected Color mDefaultColor;

        protected DebugText mDebugText; //For Component added in code

        protected float mMinDistance = 0.1f; //Acceptable distance

        public enum Result
        {
            Stuck
            , Navigating
            , Arrived
            ,Aborted
            ,CollidedWith
            ,CanSee
        } ;

        public delegate    bool AgentStatusChange(AgentBase vAgent,Result vResult, GameObject vOther=null); //Action to call when we have arrived, returns true if this stop the Navigation

        protected AgentStatusChange mOnAgentStatusChange;

        Coroutine mCheckProgress;   //Check progress of Agent

        protected virtual void Start()
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

        bool mSelected = false; //Kepp track of if we are selected
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

        //Have we arrived, if we have a path and are close, if we dont have a path this will always be false, we can override this to define other Arrived conditions
        public  virtual bool    hasArrived
        {
            get
            {
                if(mNMA.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    return (mNMA.remainingDistance < mMinDistance);
                }
                return false; //Can have arrived as we dont have path
            }
        }
        //is it stuck, if it has a path but not moved, yes, we can override this if needed to define other Stuck conditions
        public virtual bool isStuck
        {
            get
            {
                if(mNMA.hasPath)
                {
                    return LastDistance>0.1f; //Are we moving
                }
                return true;
            }
        }


        //Allows a destination to be set and will use delegate to signal progress
        public void SetDestination(Vector3 vPosition, AgentStatusChange vArrived=null) //Set Destination on NavMesh
        {
            if(mCheckProgress != null)
            {
                StopCoroutine(mCheckProgress);
                mCheckProgress = null;
                if(mOnAgentStatusChange!=null)
                {
                    mOnAgentStatusChange(this,Result.Aborted); //Abort last path
                    mOnAgentStatusChange = null;
                }
            }
            mOnAgentStatusChange = vArrived;    //Set callback delegate
            mNMA.SetDestination(vPosition);
            mCheckProgress = StartCoroutine(CheckProgress()); //Check on Navigation progress
        }

        //Distance since last move
        protected float    LastDistance {
            get {
                return (mLastPosition - transform.position).magnitude;  //Since last move
            }
        }

        protected void ClearPath() //Reset Path & remove callback delegate
        {
            mNMA.ResetPath(); //Clear path
            mOnAgentStatusChange = null; //Clear Delegate
        }
        //Update Navigation progress
        bool    UpdateProgress()
        {
            if (hasArrived) {
                if (mOnAgentStatusChange != null) {
                    if (mOnAgentStatusChange(this, Result.Arrived)) //Signal we are there
                    {
                        ClearPath();
                        return true;  //Quit CoRoutine
                    }
                }
            }
            if (isStuck) {
                if (mOnAgentStatusChange != null) //Is there a callback delegate
                {
                    if (mOnAgentStatusChange(this, Result.Stuck)) //Signal we are there
                    {
                        ClearPath();
                        return true;  //Quit CoRoutine
                    }
                }
            } else {
                if (mOnAgentStatusChange != null) //Is there a callback delegate
                {
                    if (mOnAgentStatusChange(this, Result.Navigating)) //Signal Still tracking
                    {
                        ClearPath();
                        return true;  //Quit CoRoutine
                    }
                }
            }
            return false;
        }
        Vector3 mLastPosition; //Keep track of last location so we can work out if we are still moving
        IEnumerator CheckProgress() //Runs in background to check on progress
        {
            mLastPosition=transform.position;
            for(; ; )
            {
                yield return new WaitForSeconds(0.5f); //Check every 1/2 second
                mLastPosition = transform.position; //New Last Position
                if(UpdateProgress()) break;     //Quit CoRoutine if navigation over
                if(Perception()) break; //Check path ahead
            }
        }
        //Use same method for collsion and trigger, for simplicity
        private void OnCollisionEnter(Collision collision)
        {
            OnCollision(collision.gameObject);
        }
        private void OnTriggerEnter(Collider other)
        {
            OnCollision(other.gameObject);
        }
        protected virtual void OnCollision(GameObject vGO) //We can override
        {
            if (mOnAgentStatusChange != null) //Is there a callback delegate
            {
                if (mOnAgentStatusChange(this, Result.CollidedWith, vGO)) //We have collided
                {
                    ClearPath();
                    if (mCheckProgress != null) {
                        StopCoroutine(mCheckProgress);
                        mCheckProgress = null;
                    }
                }
            }
        }
        bool   Perception()
        {
            Ray tForwardRay = new Ray(transform.position,transform.forward); //Ray pointing forward from agent
            RaycastHit tHit;
            if(Physics.Raycast(tForwardRay, out tHit, RayLenght))
            {
                if (mOnAgentStatusChange(this, Result.CanSee, tHit.collider.gameObject)) //We can see something
                {
                    ClearPath();
                    if (mCheckProgress != null)
                    {
                        StopCoroutine(mCheckProgress);
                        mCheckProgress = null;
                    }
                }
            }
            return  false;
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position,transform.position+transform.forward*RayLenght);
        }
    }
