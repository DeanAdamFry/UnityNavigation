using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [SerializeField]
    float Height = 10;

    [SerializeField]
    float MaxHeight = 50;

    [SerializeField]
    float MinHeight = 5;

    [SerializeField]
    float Speed = 1;

    [SerializeField]
    float HeightSpeed = 5;

    [SerializeField]
    float AngularSpeed = 5;

    [SerializeField]
    float MouseTumbleSpeed = 5;

    Camera mCamera;

    Quaternion mStartRotation;
    Vector3 mStartPosition;
    float mStartHeight;

    private void Start()
    {
        mCamera = GetComponent<Camera>();
        if(mCamera==null) //If we have Camera use this, if not add one
        {
            mCamera = gameObject.AddComponent<Camera>();        //Add Camera with code
        }
        mStartRotation = transform.rotation;
        mStartPosition = transform.position;    //Reset point
        mStartHeight = Height;
    }

    void LateUpdate()
    {
        ProcessCameraMove();
        if(Input.GetMouseButtonDown(0))
        {
            SetDestination();
        }
    }

    void    ProcessCameraMove()
    {
        Vector2 tMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Height = Mathf.Clamp(Height - (Input.GetAxis("Mouse ScrollWheel") * HeightSpeed), MinHeight, MaxHeight);

        Vector2 tMouseTumber = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (Input.GetMouseButton(2)) //If middle mouse pressed rotate around view vector (tumble)
        {
            Quaternion tRotation = Quaternion.AngleAxis(tMouseTumber.x* AngularSpeed, transform.forward);
            tRotation = tRotation * Quaternion.AngleAxis(tMouseTumber.y* AngularSpeed, transform.right);
            transform.rotation = transform.rotation * tRotation;
            transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        }
        else //if not then pan
        {
            if(Input.GetMouseButton(1))
            {
                tMove = tMouseTumber * MouseTumbleSpeed;
            }
            Vector3 tPosition = transform.position;
            tPosition.x += tMove.x * Time.deltaTime * Speed;
            tPosition.y = Height;
            tPosition.z += tMove.y * Time.deltaTime * Speed;
            transform.position = tPosition;
        }
        if (Input.GetKeyDown(KeyCode.Return)) //In case we get lost
        {
            Reset();
        }
    }

    void SetDestination()
    {
        RaycastHit tHit;
        Ray tRay = mCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(tRay, out tHit))
        {
            Debug.DrawRay(tRay.origin, tRay.direction, Color.red);
            Agent tAgent = tHit.collider.GetComponent<Agent>(); //Did we hit an agent
            if (tAgent != null)
            {
                tAgent.Selected = !tAgent.Selected;
                Debug.Log(tHit.collider.name);
            }
            else
            {
                Agent[] tAgents = FindObjectsOfType<Agent>(); //Get all the agents in the scene
                foreach (Agent tFoundAgent in tAgents)
                {
                    if (tFoundAgent.Selected)   //Command selected ones
                    {
                        tFoundAgent.SetDestination(tHit.point);
                        tFoundAgent.Selected = false; //Deselect once its been commanded
                    }
                }
            }
        }
    }

    private void Reset() //Reset Camera in case we get lost
    {
        transform.rotation=mStartRotation;
        transform.position=mStartPosition;    //Reset point
        Height=mStartHeight;
    }
}
