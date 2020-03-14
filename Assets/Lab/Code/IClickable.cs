using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickable
{
    void Clicked(Ray vRay, RaycastHit vHit,bool VDown);
    void Hover(Ray vRay, RaycastHit vHit);
}
