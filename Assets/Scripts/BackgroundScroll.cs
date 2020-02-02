using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class BackgroundScroll : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        private Transform _cam;
        private Transform _upperBackground;

        // --- Properties -------------------------------------------------------------------------------------------------
        public float Threshold => _upperBackground.localPosition.y;

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            _cam = Camera.main.transform;
            _upperBackground = transform.GetChild(0);
        }

        private void FixedUpdate()
        {
            if(_cam.position.y - transform.position.y > Threshold)
            {
                transform.position += Vector3.up * Threshold;
            }
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------

        // --- Protected/Private Methods ----------------------------------------------------------------------------------

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}