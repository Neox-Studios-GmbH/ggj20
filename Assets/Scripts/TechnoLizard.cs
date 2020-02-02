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
        [Space]
        [SerializeField] private Animator _bodyAnimator = null;
        [SerializeField, Min(0f)] private float _movementSpeed = 2f;
        [SerializeField, Min(0f)] private float _rotationSpeed = 90f;
        [SerializeField, Range(0f, 180f)] private float _maxAngle = 100f;
        [Space]
        [SerializeField] private float _maxGrapplingChargeDuration = 1.5f;
        [SerializeField] private float _grappleCancelCooldown = .4f;

        private float _angle = 0f;
        private float _targetAngle = 0f;
        private float _stackOffset = 5f;

        private float _chargeStartTime;
        private Delay _grapplingDelay;

        private readonly int ANIM_BODY_IS_WALKING = Animator.StringToHash("IsWalking");

        // --- Properties -------------------------------------------------------------------------------------------------
        public State CurrentState { get; private set; } = State.Rotating;
        public PlayerStack Stack => _playerStack;        

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Start()
        {
            _grapplingDelay = new Delay(_grappleCancelCooldown);

            _stackOffset = this.transform.position.y - Stack.NextBlockPosition.y;
    }

        private void Update()
        {
            switch(CurrentState)
            {
                case State.Rotating:
                case State.Charging:
                    UpdateRotation();
                    UpdateHeight();
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
                _hook.Fire(chargeT, () => CurrentState = State.Rotating);
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

        private void UpdateHeight()
        {
            Vector3 pos = transform.position;
            float targetY = Stack.NextBlockPosition.y + _stackOffset;

            if(pos.y != targetY)
            {
                _bodyAnimator.SetBool(ANIM_BODY_IS_WALKING, true);
                pos.y = Mathf.MoveTowards(pos.y, targetY, _movementSpeed * Time.deltaTime);
                transform.position = pos;
            }
        }

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}