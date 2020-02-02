using System.Collections;
using UnityEngine;

namespace GGJ20
{
    public class CoroutineRunner : MonoBehaviour 
    {
        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------

        // --- Properties -------------------------------------------------------------------------------------------------
        public static CoroutineRunner Instance { get; private set; }

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public static Coroutine Run(IEnumerator routine, float delay = 0f)
        {
            if(delay <= 0f)
            {
                return Instance.StartCoroutine(routine);
            }
            else
            {
                return Instance.StartCoroutine(DelayedCoroutine(delay, routine));
            }
        }

        public static void Stop(Coroutine routine)
        {
            if(routine != null)
            {
                Instance.StopCoroutine(routine);
            }
        }

        public static void StopAll()
        {
            Instance.StopAllCoroutines();
        }

        // --------------------------------------------------------------------------------------------
        public static Coroutine ExecuteDelayed(float seconds, System.Action action, bool unscaledTime = true)
        {
            if(action == null)
                return null;

            if(seconds <= 0f)
            {
                action.Invoke();
                return null;
            }
            else
            {
                return Instance.StartCoroutine(DelayedActionRoutine(seconds, action, unscaledTime));
            }
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        private static IEnumerator DelayedCoroutine(float delay, IEnumerator routine)
        {
            yield return new WaitForSeconds(delay);
            yield return Instance.StartCoroutine(routine);
        }

        private static IEnumerator DelayedActionRoutine(float seconds, System.Action action, bool unscaledTime)
        {
            if(unscaledTime)
            {
                yield return new WaitForSecondsRealtime(seconds);
            }
            else
            {
                yield return new WaitForSeconds(seconds);
            }

            action?.Invoke();
        }

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}