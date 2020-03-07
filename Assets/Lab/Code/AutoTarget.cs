using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTarget : MonoBehaviour
{

    Agent mAgent; //Cache Agent

    [SerializeField]
    float TimeOut = 20.0f;   //How long before Idle Agent will look

    // Start is called before the first frame update
    void Start()
    {
        mAgent = GetComponent<Agent>(); //Find Agent or make one
        if(mAgent==null)
        {
            mAgent = gameObject.AddComponent<Agent>();
        }

        StartCoroutine(AutoSearch());
    }


    IEnumerator AutoSearch()
    {
        bool tAllGone = false;  //Look for Pickups
        do
        {
            float tTimeOut = Time.time + TimeOut;
            do
            {
                yield return new WaitForSeconds(0.5f);  //Check every 1/2 second
            } while (Time.time < tTimeOut);
            if (!mAgent.Selected)
            {
                Pickup tPickup = FindPickup();
                if (tPickup != null)
                {
                    mAgent.SetDestination(tPickup.transform.position);
                }
                else
                {
                    tAllGone = true; //Exit loop
                }
            }
        } while (!tAllGone);
    }

    Pickup  FindPickup()
    {
        Pickup tPickup=null;

        Pickup[] tAllPickups = FindObjectsOfType<Pickup>();
        if(tAllPickups.Length>0)
        {
            tPickup = tAllPickups[Random.Range(0, tAllPickups.Length)]; //Pick a Random one
        }
        return tPickup;
    }
}
