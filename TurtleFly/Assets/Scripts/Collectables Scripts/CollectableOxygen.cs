using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableOxygen : CollectableScript
{
    public int OxygenAmount;
    public Transform ChildBoostInfo;
    public override void OnCollisionFunction()
    {
        Main.Instance.CollectedOxygen(gameObject);

        UIManager.Instance.LerpCollectableInfoFunc(ChildBoostInfo, transform.position + Vector3.up * 1.5f, 1f, 360f);
    }
}
