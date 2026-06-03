using System;
using System.Collections;
using UnityEngine;

public class WaitAction
{
    public static IEnumerator wait(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    public static IEnumerator wait(Func<bool> condition, Action action, float timeOut = -1)
    {
        float startTime = Time.time;
        while (!condition())
        {
            if (timeOut > 0 && Time.time - startTime > timeOut)
                break;
            yield return null;
        }

        action();
    }

    public static IEnumerator waitRealtime(float waitTime, Action action)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        action();
    }

    public static IEnumerator waitOneFrame(Action action)
    {
        yield return null;
        action();
    }
}
