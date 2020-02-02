﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class CameraManager : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------
        public enum ScreenShakeStrength
        {
            Weak = 0,
            Medium = 1,
            Strong = 2
        }

        // --- Nested Classes ---------------------------------------------------------------------------------------------
        [Serializable]
        private class ScreenShakeOptions
        {
            public ScreenShakeStrength Strength;
            public float TimeLength;
            public Vector2 ShakeStrength;
        }

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private float _yOffset = 10f;
        [SerializeField, Range(1f, 8f)] private float _smoothingFactor = 2f;
        [SerializeField, Range(0.01f, 0.2f)] private float _sigma = 0.1f;
        [SerializeField] private List<ScreenShakeOptions> _options;

        private float _currentMaxHeight = 0;

        // --- Properties -------------------------------------------------------------------------------------------------
        private void Start()
        {
            AdjustCameraHeightToPlayerStack(GameManager.GetStack(Players.PlayerOne));
            AdjustCameraHeightToPlayerStack(GameManager.GetStack(Players.PlayerTwo));

            GameManager.Instance.onStackChanged += AdjustCameraHeightToPlayerStack;
        }

        private void Update()
        {
            float y = transform.position.y;

            //if(y == _currentMaxHeight)
            //{
            //    return;
            //}

            //y += (_currentMaxHeight - y) / _smoothingFactor;

            //y = Mathf.Abs(y - _currentMaxHeight) < _sigma ? _currentMaxHeight : y;
            y = Mathf.MoveTowards(y, _currentMaxHeight, _smoothingFactor * Time.deltaTime);

            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
        // --- Unity Functions --------------------------------------------------------------------------------------------

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void ShakeScreenByStrength(ScreenShakeStrength strength)
        {
            ScreenShakeOptions op = _options.FirstOrDefault(o => o.Strength == strength);

            if(op == null)
            {
                Debug.Log("Why are you doing this????????");
                return;
            }

            ShakeScreen(op.ShakeStrength, op.TimeLength);
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        private void AdjustCameraHeightToPlayerStack(PlayerStack stack)
        {
            _currentMaxHeight = Mathf.Max(_currentMaxHeight, stack.Height + _yOffset);
        }

        private void ShakeScreen(Vector2 strength, float t)
        {
            StartCoroutine(ShakeScreenCoroutine(strength, t));
        }

        private IEnumerator ShakeScreenCoroutine(Vector2 s, float t)
        {
            Vector3 before = transform.position;
            float wait = t / 8f;
            float falloff = 0.1f;
            float mult = 1f;

            yield return Shake(new Vector2(s.x , s.y), wait, mult);
            yield return Shake(new Vector2(-s.x, -s.y), wait, mult);
            mult -= falloff;
            yield return Shake(new Vector2(-s.x, -s.y), wait, mult);
            yield return Shake(new Vector2(s.x, s.y), wait, mult);
            mult -= falloff;
            yield return Shake(new Vector2(-s.x, s.y), wait, mult);
            yield return Shake(new Vector2(s.x, -s.y), wait, mult);
            mult -= falloff;
            yield return Shake(new Vector2(s.x, -s.y), wait, mult);
            yield return Shake(new Vector2(-s.x, s.y), wait, mult);

            transform.position = before;
        }

        private IEnumerator Shake(Vector2 s, float cd, float mult)
        {
            transform.position += new Vector3(s.x * mult, s.y * mult, 0);
            yield return new WaitForSeconds(cd);
        }

        // --------------------------------------------------------------------------------------------

        // **************************************************************************************************************************************************
    }
}