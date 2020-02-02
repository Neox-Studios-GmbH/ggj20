using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class GrapplingHook : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------
        public enum State
        {
            Idle = 0,
            Forward = 1,
            Backward = 2
        }

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private float _targetRange;
        [Header("Generation")]
        [SerializeField, Min(0f)] private float _maxLength = 10f;
        [SerializeField] private SpriteRenderer _firstPiece;
        [SerializeField] private float _pieceOffset = .451f;
        [SerializeField] private float _enableOffset = 1;
        [Space]
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _head;
        [Space]
        [SerializeField] private FloatRange _forwardDuration = new FloatRange(.15f, .3f);
        [SerializeField] private float _forwardSpeed = 50f;
        [SerializeField] private float _forwardAcceleration = 200f;
        [SerializeField] private float _backwardDelay = .3f;
        [SerializeField] private float _backwardSpeed = 5f;

        private State _state = State.Idle;
        private List<SpriteRenderer> _pieces;
        private float _extension = 0f;
        private float _currentSpeed;

        private Delay _forwardTimer;
        private Action _onFinish;

        // --- Properties -------------------------------------------------------------------------------------------------
        public BuildingBlock GrabbedBlock { get; set; }
        public Players Player { get; set; }
        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            _forwardTimer = new Delay(0f);

            GeneratePieces();
            UpdateVisibility();
        }

        void Update()
        {
            switch(_state)
            {
                case State.Idle:
                    break;
                case State.Forward:
                    Extend();
                    break;
                case State.Backward:
                    Retract();
                    break;
            }
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void Fire(float chargeT, Action onFinish = null)
        {
            _currentSpeed = 0f;
            _forwardTimer.ChangeDuration(_forwardDuration.Lerp(chargeT));

            _state = State.Forward;
            _onFinish = onFinish;
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        private void GeneratePieces()
        {
            _pieces = new List<SpriteRenderer> { _firstPiece };

            int amount = Mathf.CeilToInt(_maxLength / _pieceOffset);
            for(int i = 1; i <= amount; i++)
            {
                SpriteRenderer copy = Instantiate(_firstPiece, _firstPiece.transform.parent, true);
                copy.transform.position -= transform.up * i * _pieceOffset;
                copy.sortingOrder++;
                _pieces.Add(copy);
            }
        }

        // --------------------------------------------------------------------------------------------

        private void Extend()
        {
            ChangeExtension(_maxLength, 1, _forwardAcceleration, _forwardSpeed);
            UpdateVisibility();

            Collider2D col = CheckCollision();
            if(col != null)
            {
                Grabable graber = col.GetComponent<Grabable>();
                if(graber != null)
                {
                    DelayedRetract();
                    Debug.Log($"{Logger.GetPre(this)} Hit {graber.name}");

                    if(graber is BuildingBlock grab)
                    {
                        DelayedRetract();
                        GrabbedBlock = grab;
                        grab.Grab(_head);
                    }
                }
            }

            if(_extension == _maxLength || _forwardTimer.HasElapsed)
            {
                DelayedRetract();
            }
        }
        private void DelayedRetract()
        {
            _currentSpeed = 0f;
            _state = State.Idle;
            CoroutineRunner.ExecuteDelayed(_backwardDelay, () =>
            {
                _state = State.Backward;
            });
        }
        private Collider2D CheckCollision()
        {
            Collider2D[] colls = Physics2D.OverlapCircleAll(_head.transform.position, _targetRange);

            if(colls.Length != 0)
                return colls[0];
            return null;
        }
        private void Retract()
        {
            ChangeExtension(0f, -1, _forwardAcceleration, _backwardSpeed);
            UpdateVisibility();

            if(_extension == 0f)
            {
                _state = State.Idle;
                _onFinish?.Invoke();
            }
        }

        private void ChangeExtension(float target, int direction, float acceleration, float targetSpeed)
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, direction * acceleration * Time.deltaTime);
            _extension = Mathf.MoveTowards(_extension, target, direction * _currentSpeed * Time.deltaTime);

            _head.localPosition = Vector3.up * _extension;
        }

        // --------------------------------------------------------------------------------------------
        private void UpdateVisibility()
        {
            for(int i = 0; i < _pieces.Count; i++)
            {
                _pieces[i].gameObject.SetActive(_pieces[i].transform.localPosition.y + _enableOffset >= -_extension);
            }
        }

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}