using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ20
{
    [Serializable]
    public class Delay
    {
        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField, Min(0f)] private float _duration = 1f;
        [SerializeField] private bool _realtime = false;

        private float _startTime;
        private float _durationLeft;

        // --- Properties -------------------------------------------------------------------------------------------------		
        public float Duration => _duration;
        public float TimeLeft => Mathf.Max(0f, _durationLeft - TimeElapsed);
        public bool HasElapsed => TimeElapsed >= _durationLeft;
        public bool IsPaused { get; private set; }

        private float _Time => _realtime ? Time.realtimeSinceStartup : Time.time;
        private float TimeElapsed => IsPaused ? 0f : _Time - _startTime;

        // --- Constructors -----------------------------------------------------------------------------------------------
        public Delay(float duration, bool realtime = false, bool isPaused = false)
        {
            _duration = duration;
            _realtime = realtime;

            Reset();

            if(isPaused)
            {
                Pause();
            }
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void Reset()
        {
            _startTime = _Time;
            _durationLeft = _duration;
        }

        public void Restart()
        {
            Reset();
            Resume();
        }

        /// <summary>
        /// Restart the timer and execute the given action after its duration.
        /// Note that pausing the timer afterwards, will not affect execution of the action.
        /// </summary>
        public void RestartMonitored(Action onElapsed)
        {
            Restart();
            CoroutineRunner.ExecuteDelayed(_duration, onElapsed, _realtime);
        }

        // --------------------------------------------------------------------------------------------
        public void Pause()
        {
            _durationLeft = TimeLeft;
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
            _startTime = _Time;
        }

        // --------------------------------------------------------------------------------------------
        /// <summary>Change's the timer's length and resets the timer.</summary>
        public void ChangeDuration(float duration)
        {
            _duration = Mathf.Max(0f, duration);
            Reset();
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------		

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}