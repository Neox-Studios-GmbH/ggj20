using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        public int PlayerIndex => (int)_playerStack.Player + 1;

        private string InputHorizontal => $"Player{PlayerIndex}_Horizontal";
        private string InputVertical => $"Player{PlayerIndex}_Vertical";
        private string InputLeft => $"Player{PlayerIndex}_Left";
        private string InputRight => $"Player{PlayerIndex}_Right";
        private string InputGrapple => $"Player{PlayerIndex}_Grapple";

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
                    OnGrappleInput();
                    break;

                case State.Firing:
                    break;
            }
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void OnGrappleInput()
        {
            if(!_grapplingDelay.HasElapsed)
                return;

            bool isPress = Input.GetButtonDown(InputGrapple);
            if(isPress && CurrentState == State.Rotating)
            {
                _chargeStartTime = Time.time;
                CurrentState = State.Charging;
            }
            else if(CurrentState == State.Charging && Input.GetButtonUp(InputGrapple))
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
            float xInput = 0f;
            if(Input.GetButton(InputLeft))
                xInput += 1f;
            if(Input.GetButton(InputRight))
                xInput -= 1f;

            if(xInput != 0f)
            {
                _targetAngle = xInput * _maxAngle;
            }
            else
            {
                Vector2 axis = new Vector2(Input.GetAxis(InputHorizontal), Input.GetAxis(InputVertical));
                if(axis == Vector2.zero)
                    return;

                axis.Normalize();

                _targetAngle = Mathf.Clamp(Vector2.SignedAngle(Vector2.up, axis), -_maxAngle, _maxAngle);
            }

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