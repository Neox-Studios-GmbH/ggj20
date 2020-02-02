using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class LavaPool : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private FloatRange _speedRamge;
        [SerializeField] private float _maxSpeedDuration;

        private float _elapsed = 0f;

        // --- Properties -------------------------------------------------------------------------------------------------
        private float CameraMinY => Camera.main.transform.position.y - Camera.main.orthographicSize;

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void FixedUpdate()
        {
            _elapsed += Time.deltaTime;
            float lavaT = Mathf.InverseLerp(0f, _maxSpeedDuration, _elapsed);
            float speed = _speedRamge.Lerp(lavaT);

            Vector3 lavaPos = transform.position;
            if(lavaPos.y < CameraMinY)
            {
                lavaPos.y = CameraMinY;
            }

            lavaPos.y += speed * Time.deltaTime;
            transform.position = lavaPos;

            bool playerOneSubmerged = HasSubmerged(GameManager.GetStack(Players.PlayerOne));
            bool playerTwoSubmerged = HasSubmerged(GameManager.GetStack(Players.PlayerTwo));

            if(playerOneSubmerged && playerTwoSubmerged)
            {
                GameManager.OnDraw();
            }
            else if(playerOneSubmerged)
            {
                GameManager.OnPlayerWon(Players.PlayerTwo);
            }
            else if(playerTwoSubmerged)
            {
                GameManager.OnPlayerWon(Players.PlayerOne);
            }
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public bool HasSubmerged(PlayerStack stack)
        {
            return transform.position.y > stack.NextBlockPosition.y;
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}