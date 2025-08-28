using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NS.Util
{
    public class CoroutineRunner : SingletonMonoBehaviour<CoroutineRunner>
    {
        public static IEnumerator WaitForSeconds(System.Action action, float second)
        {
            yield return new WaitForSeconds(second);
            action.Invoke();
        }

        public Coroutine WaitRun(System.Action action, float second)
        {
            return StartCoroutine(WaitForSeconds(action, second));
        }
        public static IEnumerator WaitForFrame(System.Action action, int frame)
        {
            for (int i = 0; i < frame; i++)
                yield return null;
            action.Invoke();
        }

        public Coroutine WaitFrameRun(System.Action action, int frame)
        {
            return StartCoroutine(WaitForFrame(action, frame));
        }

        public Coroutine Run(IEnumerator enumerator)
        {
            return StartCoroutine(enumerator);
        }
        public void StopTask(Coroutine enumerator)
        {
            StopCoroutine(enumerator);
        }
    }
}


