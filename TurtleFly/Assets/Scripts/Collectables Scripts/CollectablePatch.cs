using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePatch : CollectableScript
{
    public Transform ChildBoostInfo;
    public override void OnCollisionFunction()
    {
        ProtectManager.Instance.PatchAddBoost(gameObject);

        UIManager.Instance.LerpCollectableInfoFunc(ChildBoostInfo, transform.position + Vector3.up * 1.5f, 1f, 360f);
    }
}
