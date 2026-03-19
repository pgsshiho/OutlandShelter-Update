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

    public static IEnumerator wait(Func<bool> condition, Action action)
    {
        yield return new WaitUntil(condition);
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
