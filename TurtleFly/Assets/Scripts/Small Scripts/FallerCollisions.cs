using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallerCollisions : MonoBehaviour
{
    public void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
            Main.Instance.KnockPlayerOff();
    }
}
