using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class SmoothCoroutines
{
    public delegate float easingFunc(float start, float end, float lerp);

    public static IEnumerator SmoothDisablePop(GameObject obj, float time, float delayTime, easingFunc lerpFunc)
    {
        yield return new WaitForSecondsRealtime(delayTime);

        float t = 0f;
        Vector3 startScale = obj.transform.localScale;

        while (t < time)
        {
            t += Time.deltaTime;

            obj.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, lerpFunc(0f, 1f, t / time));

            yield return null;
        }

        obj.SetActive(false);
    }

    public static IEnumerator SmoothScaleLerp(Transform obj, Vector3 newScale, float time, easingFunc lerpFunc)
    {
        float t = 0f;
        Vector3 startScale = obj.transform.localScale;

        while (t < time)
        {
            t += Time.deltaTime;

            obj.transform.localScale = Vector3.Lerp(startScale, newScale, lerpFunc(0f, 1f, t / time));

            yield return null;
        }
    }

    public static IEnumerator SmoothUIScaleLerp(Transform obj, float time, easingFunc lerpFunc)
    {
        float t = 0f;
        obj.localScale = Vector3.zero;
        obj.gameObject.SetActive(true);

        while (t < time)
        {
            t += Time.deltaTime;

            obj.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, lerpFunc(0f, 1f, t / time));

            yield return null;
        }

        obj.localScale = Vector3.one;
    }

    public static IEnumerator SmoothPositionLerp(Transform obj, Vector3 newPos, float delayTime, float time, easingFunc lerpFunc)
    {
        yield return new WaitForSeconds(delayTime);

        float t = 0f;
        Vector3 startPos = obj.transform.position;

        while(t < time)
        {
            t += Time.deltaTime;

            obj.transform.position = Vector3.Lerp(startPos, newPos, lerpFunc(0f, 1f, t / time));

            yield return null;
        }
    }

    public static IEnumerator SmoothPositionLerp(Transform obj, Vector3 localStartPos, Transform newPos, float delayTime, float time, easingFunc lerpFunc)
    {
        yield return new WaitForSeconds(delayTime);

        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;

            obj.transform.position = Vector3.Lerp(localStartPos, newPos.position, lerpFunc(0f, 1f, t / time));

            yield return null;
        }
    }
}
