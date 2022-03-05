using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleCollisions : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag == "Baloon")
        {
            ProtectManager.Instance.BaloonNeedleCollision(this, other.contacts[0].point, other.contacts[0].normal);
        }
        else if (other.transform.tag == "Shield")
        {
            ProtectManager.Instance.ShieldNeedleCollision(other.gameObject, this);
        }
    }

    public void DisableNeedle()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponentInParent<Animation>().enabled = false;

        Destroy(transform.parent.gameObject, 1f);
    }
}
