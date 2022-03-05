using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Image OxygenSlider;
    public Text CoinsText;

    [Space]
    public Transform StartGame;
    public Transform GameLost;
    public Transform GameWon;

    private void Awake()
    {
        if (Instance)
            Destroy(Instance);
        if (!Instance)
            Instance = this;
    }

    private bool oxygenTracking = false;

    public void StartTrackingOxygenUISlider()
    {
        oxygenTracking = true;
        StartCoroutine(lerpSliderEachSecond());
    }

    public void StopTrackingOxygenUISlider()
    {
        oxygenTracking = false;
    }

    public void UpdateCoinsUI(int newValue)
    {
        CoinsText.text = newValue.ToString();
    }

    public void ShowGameState(GameStates newState)
    {
        if(newState == GameStates.Playing)
        {
            StartCoroutine(SmoothCoroutines.SmoothDisablePop(StartGame.gameObject, 0.3f, 0f, EasingFunction.EaseInOutCirc));
        }
        else if (newState == GameStates.Finished)
        {
            StartCoroutine(SmoothCoroutines.SmoothUIScaleLerp(GameWon, 0.5f, EasingFunction.EaseInOutCirc));
        }
        else if (newState == GameStates.DiedKnock || newState == GameStates.DiedOxygen)
        {
            StartCoroutine(SmoothCoroutines.SmoothUIScaleLerp(GameLost, 0.5f, EasingFunction.EaseInOutCirc));
        }
    }

    private IEnumerator lerpSliderEachSecond()
    {
        while (oxygenTracking)
        {
            StartCoroutine(lerpSliderLinear(OxygenSlider, 1f, Main.Instance.OxygenPercentageCurrent));
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator lerpSliderLinear(Image slider, float time, float newValue)
    {
        float t = 0f, startValue = slider.fillAmount;

        while (t < time)
        {
            t += Time.deltaTime;

            slider.fillAmount = Mathf.Lerp(startValue, newValue, t / time);

            yield return null;
        }
    }

    public void LerpCollectableInfoFunc(Transform objectTR, Vector3 endPos, float timeLerp, float rotateDegrees)
    {
        StartCoroutine(LerpCollectableInfo(objectTR, endPos, timeLerp, rotateDegrees));
    }

    public IEnumerator LerpCollectableInfo(Transform objectTR, Vector3 endPos, float timeLerp, float rotateDegrees)
    {
        objectTR.parent = null;
        Vector3 startPos = objectTR.position, rotatePerTime = new Vector3(0f, rotateDegrees / timeLerp, 0f);
        float t = 0f;

        while (t < timeLerp)
        {
            t += Time.deltaTime;

            objectTR.position = Vector3.Lerp(startPos, endPos, EasingFunction.EaseInOutCirc(0f, 1f, t / timeLerp));
            objectTR.localEulerAngles += rotatePerTime * Time.deltaTime;

            yield return null;
        }

        StartCoroutine(SmoothCoroutines.SmoothDisablePop(objectTR.gameObject, 0.3f, 0f, EasingFunction.EaseInOutCirc));
        Destroy(objectTR.gameObject, 0.35f);
    }
}
