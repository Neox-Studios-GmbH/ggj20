using System;
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
        [SerializeField, Range(1f, 8f)] private float _smoothingFactor = 2f;
        [SerializeField, Range(0.01f, 0.2f)] private float _sigma = 0.1f;
        [SerializeField] private List<ScreenShakeOptions> _options;

        private float _currentMaxHeight = 0;
        private float _shakeCooldown = 0.05f;

        // --- Properties -------------------------------------------------------------------------------------------------
        private void Start()
        {
            GameManager.Instance.OnStackChanged += AdjustCameraHeightToPlayerStack;
            _currentMaxHeight = 0;
        }

        private void Update()
        {
            float y = transform.position.y;

            if(y == _currentMaxHeight)
            {
                return;
            }

            y += (_currentMaxHeight - y) / _smoothingFactor;

            y = Mathf.Abs(y - _currentMaxHeight) < _sigma ? _currentMaxHeight : y;

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
            _currentMaxHeight = Mathf.Max(_currentMaxHeight, stack.CombinedStackHeight);
        }

        private void ShakeScreen(Vector2 strength, float t)
        {
            StartCoroutine(ShakeScreenCoroutine(strength, t));
        }

        private IEnumerator ShakeScreenCoroutine(Vector2 s, float t)
        {
            float elapsed = 0f;
            while(t < elapsed)
            {
                transform.position += new Vector3();
                yield return new WaitForSeconds(_shakeCooldown);
                transform.position += new Vector3();
                yield return new WaitForSeconds(_shakeCooldown);
                transform.position += new Vector3();
                yield return new WaitForSeconds(_shakeCooldown);
                transform.position += new Vector3();
                yield return new WaitForSeconds(_shakeCooldown);
                elapsed += 4 * _shakeCooldown;
            }
        }
        // --------------------------------------------------------------------------------------------

        // **************************************************************************************************************************************************
    }
}