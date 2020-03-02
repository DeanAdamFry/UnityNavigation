using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649  //Stop the warning about no assignment, as it will be assigned in IDE

public class Pickup : MonoBehaviour
{
    public float Speed=360.0f;

    [SerializeField]
    int Score = 10;


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
        transform.Rotate(0, Speed * Time.deltaTime, 0);
    }


    private void OnTriggerEnter(Collider vOtherCol)
    {
        Debug.LogFormat("{1} Triggered by {0}", vOtherCol.gameObject.name,gameObject.name);

        DealWithPickup(vOtherCol.gameObject);   //Call pickup handler
    }

    private void OnCollisionEnter(Collision vOtherCol)
    {
        Debug.LogFormat("{1} Collided with {0}", vOtherCol.gameObject.name,gameObject.name);
        DealWithPickup(vOtherCol.gameObject);   //Call pickup handler
    }

    void DealWithPickup(GameObject vPlayer)
    {
        if (vPlayer.GetComponent<Agent>() == null) return;
        GM.Score += Score;
        Destroy(gameObject);
    }

}
