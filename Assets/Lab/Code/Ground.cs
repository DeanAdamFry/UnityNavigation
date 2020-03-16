using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour, IClickable
{

    [SerializeField]
    GameObject Prefab;

    public void Clicked(Ray vRay, RaycastHit vHit, bool VDown)
    {
        Instantiate(Prefab,vHit.point,Quaternion.identity);
    }

    public void Hover(Ray vRay, RaycastHit vHit)
    {
        Instantiate(Prefab, vHit.point, Quaternion.identity);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
