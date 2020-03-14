using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;   //Needed for Navmesh

#pragma warning disable CS0649  //Stop the warning about no assignment, as it will be assigned in IDE

public class Door : MonoBehaviour , IClickable
{

    NavMeshObstacle mObstacle;

    public  bool    isOpen {
        get {
            return DefaultIsOpen;
        }
    }

    public  void    SetOpen(bool vOpen,bool vSwing=true)
    {
        if (mDoorCoRoutine == null)
        {
            mDoorCoRoutine = StartCoroutine(SetDoor(vOpen, vSwing)); //Open with or without swing
        }
    }

    public bool isBusy {
        get {
            return mDoorCoRoutine != null;
        }
    }

    [SerializeField] 
    bool DefaultIsOpen;


    [SerializeField]
    Vector3 OpenRotation;


    Quaternion mInitialRotation;

    float mAlpha = 0;

    Coroutine mDoorCoRoutine=null;


    // Start is called before the first frame update
    void Start()
    {
        mObstacle = gameObject.AddComponent<NavMeshObstacle>();
        gameObject.AddComponent<BoxCollider>().isTrigger=true;

        mInitialRotation = transform.rotation; //Store Inital rotation
        MeshFilter tMF = GetComponent<MeshFilter>(); //So we can get mesh bounds
        mObstacle.shape = NavMeshObstacleShape.Box; //Most appropriate shape, seems to fit itself to the mesh
        SetOpen(DefaultIsOpen, false);
    }



    IEnumerator SetDoor(bool vOpen,bool vSwing=true)
    {
        Quaternion mFromRotation;
        Quaternion mToRotation;
        mObstacle.enabled = true;
        mObstacle.carving = true;
        if (vOpen)  //Set correct start and end positions
        {
            mFromRotation = mInitialRotation;
            mToRotation = Quaternion.Euler(OpenRotation) * mInitialRotation;
        }
        else
        {
            mFromRotation = Quaternion.Euler(OpenRotation) * mInitialRotation;
            mToRotation = mInitialRotation;
        }
        mAlpha = 0;
        if(vSwing)  //If we need a nice swing, do it
        {
            for (; ; )
            {
                transform.rotation = Quaternion.Lerp(mFromRotation, mToRotation, mAlpha); //LERP with Ease In/Out
                mAlpha += Time.deltaTime;
                if (mAlpha >= 1.0f) break;
                yield return new WaitForEndOfFrame();
            }
        }
        transform.rotation =mToRotation; //Set absolute end position
        mDoorCoRoutine = null;  //Ready to run again
        DefaultIsOpen = vOpen;
    }

    float   NiceSin(float vTime)    //Sin wave which goes from 0-1-0
    {
        return (Mathf.Sin(vTime*Mathf.PI) + 1.0f) / 2.0f;
    }

    //Allow Door to be clicked on
    public void Clicked(Ray vRay, RaycastHit vHit, bool vDown)
    {
        if(vDown)
        {
            SetOpen(!isOpen); //Toggle Door
        }
    }

    public void Hover(Ray vRay, RaycastHit vHit) //No hover functionality
    {
    }
}
