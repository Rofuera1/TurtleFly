using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectManager : MonoBehaviour
{
    public static ProtectManager Instance;

    public Transform BaloonTR;
    public GameObject PatchPrefab;
    public GameObject OxygenExhaleEffectPrefab;

    private Stack<int> shieldStack = new Stack<int>(4);
    private List<GameObject> oxygenLeaks = new List<GameObject>();

    private void Awake()
    {
        if (Instance)
            Destroy(Instance);
        if (!Instance)
            Instance = this;
    }

    private void Start()
    {
        shieldStack.Push(1);
        shieldStack.Push(2);
        shieldStack.Push(1);
        shieldStack.Push(0);
    }

    public void ShieldAddBoost(GameObject boostObj)
    {
        if (shieldStack.Count == 0)
            return;

        StartCoroutine(SmoothCoroutines.SmoothDisablePop(boostObj, 0.3f, 0.5f, EasingFunction.EaseInOutCirc));
        Destroy(boostObj, 1f);

        AnimatorManager.Instance.ActivateShieldAnimation(shieldStack.Pop());
    }

    public void PatchAddBoost(GameObject boostObj)
    {
        StartCoroutine(SmoothCoroutines.SmoothDisablePop(boostObj, 0.3f, 0.5f, EasingFunction.EaseInOutCirc));
        Destroy(boostObj, 1f);

        if (oxygenLeaks.Count != 0)
        {
            GameObject patchTemp = Instantiate(PatchPrefab, oxygenLeaks[0].transform.position + oxygenLeaks[0].transform.forward * 0.1f, Quaternion.identity, BaloonTR);
            patchTemp.transform.right = oxygenLeaks[0].transform.forward;

            StartCoroutine(SmoothCoroutines.SmoothPositionLerp(patchTemp.transform, patchTemp.transform.localPosition, oxygenLeaks[0].transform, 0.1f, 0.2f, EasingFunction.EaseInOutCirc));

            oxygenLeaks[0].SetActive(false);
            Destroy(oxygenLeaks[0], 1f);

            oxygenLeaks.RemoveAt(0);
        }

        Main.Instance.StopLoosingOxygenNeedleCause();
    }

    public void ShieldNeedleCollision(GameObject shieldObject, NeedleCollisions needleCS)
    {
        needleCS.DisableNeedle();

        GameObject temporaryShieldObj = Instantiate(shieldObject, shieldObject.transform.parent);
        temporaryShieldObj.transform.parent = null;
        Destroy(temporaryShieldObj.GetComponent<Animation>());
        temporaryShieldObj.GetComponent<Collider>().isTrigger = false;
        temporaryShieldObj.AddComponent<Rigidbody>();

        shieldStack.Push(shieldObject.GetComponent<ShieldInfo>().ShieldIndex);
        AnimatorManager.Instance.DeactivateShieldAnimation(shieldObject.GetComponent<ShieldInfo>().ShieldIndex);
    }

    public void BaloonNeedleCollision(NeedleCollisions needleCS, Vector3 contactPoint, Vector3 contactNormal)
    {
        needleCS.DisableNeedle();

        GameObject oxygenLeak = Instantiate(OxygenExhaleEffectPrefab, contactPoint, Quaternion.identity, BaloonTR);
        oxygenLeak.transform.forward = contactNormal;
        oxygenLeaks.Add(oxygenLeak);

        Main.Instance.StartLoosingOxygenNeedleCause();
    }
}
