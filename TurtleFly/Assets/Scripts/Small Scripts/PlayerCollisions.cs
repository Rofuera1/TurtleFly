using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Collectable" && other.GetComponent<CollectableScript>() != null && Main.Instance.CurrentState == GameStates.Playing)
        {
            other.GetComponent<CollectableScript>().OnCollisionFunction();
        }
    }
}
