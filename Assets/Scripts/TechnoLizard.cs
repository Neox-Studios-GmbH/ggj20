using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

namespace GGJ20
{
    public class TechnoLizard : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------
        public enum State
        {
            Rotating,
            Charging,
            Firing
        }

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private PlayerStack _playerStack = null;
        [SerializeField] private GrapplingHook _hook;
        [SerializeField] private float _stackOffset = 5f;
        [Space]
        [SerializeField] private Animator _bodyAnimator = null;
        [SerializeField, Min(0f)] private float _rotationSpeed = 90f;
        [SerializeField, Range(0f, 180f)] private float _maxAngle = 100f;
        [Space]
        [SerializeField] private float _maxGrapplingChargeDuration = 1.5f;
        [SerializeField] private float _grappleCancelCooldown = .4f;

        private float _angle = 0f;
        private float _targetAngle = 0f;

        private float _chargeStartTime;
        private Delay _grapplingDelay;

        private readonly int ANIM_BODY_IS_WALKING = Animator.StringToHash("IsWalking");

        // --- Properties -------------------------------------------------------------------------------------------------
        public State CurrentState { get; private set; } = State.Rotating;

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            _grapplingDelay = new Delay(_grappleCancelCooldown);
        }

        private void Update()
        {
            switch(CurrentState)
            {
                case State.Rotating:
                case State.Charging:
                    UpdateRotation();
                    break;

                case State.Firing:
                    break;
            }
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void OnAimInput(InputAction.CallbackContext context)
        {
            Vector2 dir = context.ReadValue<Vector2>();
            if(dir == Vector2.zero)
                return;

            _targetAngle = Mathf.Clamp(Vector2.SignedAngle(Vector2.up, dir), -_maxAngle, _maxAngle);
        }

        public void OnGrappleInput(InputAction.CallbackContext context)
        {
            if(CurrentState == State.Firing || !_grapplingDelay.HasElapsed)
                return;

            bool isPress = context.ReadValue<float>() == 1f;
            if(isPress && CurrentState == State.Rotating)
            {
                _chargeStartTime = Time.time;
                CurrentState = State.Charging;
            }
            else if(!isPress && CurrentState == State.Charging)
            {
                float chargeDuration = Time.time - _chargeStartTime;
                CurrentState = State.Firing;

                float chargeT = Mathf.InverseLerp(0f, _maxGrapplingChargeDuration, chargeDuration);
                _hook.Fire(chargeT, () =>
                {
                    Debug.Log("Gotcha!");
                    CurrentState = State.Rotating;
                    if(_hook.GrabbedBlock != null)
                    {
                        _playerStack.ReceiveBlock(_hook.GrabbedBlock);
                        _hook.GrabbedBlock = null;
                    }
                    //_playerStack.ReceiveBlock(_hook.cu)
                });
            }
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        private void UpdateRotation()
        {
            _bodyAnimator.SetBool(ANIM_BODY_IS_WALKING, _angle != _targetAngle);

            if(_angle == _targetAngle)
                return;

            _angle = Mathf.MoveTowards(_angle, _targetAngle, _rotationSpeed * Time.deltaTime);
            transform.up = Quaternion.Euler(0f, 0f, _angle) * Vector3.up;
        }

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}