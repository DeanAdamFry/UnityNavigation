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

 
    Quaternion mStartRotation;
    Vector3 mStartPosition;
    float mStartHeight;

    private void Start()
    {

        mStartRotation = transform.rotation;
        mStartPosition = transform.position;    //Reset point
        mStartHeight = Height;
    }

    void LateUpdate()
    {
        ProcessCameraMove();
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


    private void Reset() //Reset Camera in case we get lost
    {
        transform.rotation = mStartRotation;
        transform.position = mStartPosition;    //Reset point
        Height = mStartHeight;
    }

}
