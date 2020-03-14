using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649  //Stop the warning about no assignment, as it will be assigned in IDE

public class Pickup : MonoBehaviour, IClickable
{
    public float Speed=360.0f;

    [SerializeField]
    int Score = 10;

    float mTimeout;


    // Add RB & set it to Kinematic
    void Start()
    {
        Rigidbody tRB = GetComponent<Rigidbody>();
        if (tRB == null)
        {
            tRB = gameObject.AddComponent<Rigidbody>();
        }
        tRB.isKinematic = true; //Make it non physics
    }

    // Update is called once per frame
    void Update()
    {
        if(mTimeout>=0) //Only spin for so long
        {
            transform.Rotate(0, Speed * Time.deltaTime, 0);
            mTimeout -= Time.deltaTime;

        }
    }


    private void OnTriggerEnter(Collider vOtherCol)
    {
        DealWithPickup(vOtherCol.gameObject);   //Call pickup handler
    }

    private void OnCollisionEnter(Collision vOtherCol)
    {
        DealWithPickup(vOtherCol.gameObject);   //Call pickup handler
    }

    void DealWithPickup(GameObject vPlayer)
    {
        if (vPlayer.GetComponent<AgentBase>() == null) return;
        GM.Score += Score;
        Destroy(gameObject);
    }

    public void Clicked(Ray vRay, RaycastHit vHit, bool VDown)
    {
        mTimeout = 5.0f;
    }

    public void Hover(Ray vRay, RaycastHit vHit)
    {
    }
}
