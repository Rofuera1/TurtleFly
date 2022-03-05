using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public static AnimatorManager Instance;

    public Animation[] ShieldAnimations;

    private void Awake()
    {
        if (Instance)
            Destroy(Instance);
        if (!Instance)
            Instance = this;
    }

    public void ActivateShieldAnimation(int shieldIndex)
    {
        ShieldAnimations[shieldIndex].gameObject.SetActive(true);
    }

    public void DeactivateShieldAnimation(int shieldIndex)
    {
        ShieldAnimations[shieldIndex].gameObject.SetActive(false);
    }
}
