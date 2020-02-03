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
        [Space]
        [SerializeField] private float _grabRadius = .5f;
        [SerializeField] private LayerMask _grabMask;

        private State _state = State.Idle;
        private List<SpriteRenderer> _pieces;
        private float _extension = 0f;
        private float _currentSpeed;

        private Delay _forwardTimer;
        private AudioSource _extentSource;
        private Action _onFinish;

        private readonly int ANIMATOR_IS_CLOSED = Animator.StringToHash("IsClosed");

        // --- Properties -------------------------------------------------------------------------------------------------
        public Transform Head => _head;
        public BuildingBlock GrabbedBlock { get; private set; }
        public TechnoLizard Lizard { get; private set; }

        public Vector3 HeadCenter => _head.position + .5f * _head.up;

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            Lizard = GetComponentInParent<TechnoLizard>();

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
            _extentSource = SoundManager.Play(SFX.Grappling_Return, transform.position);

            _state = State.Forward;
            _onFinish = onFinish;
            //_onFinish += _extentSource.Stop;
        }

        public void ReleaseBlock()
        {
            GrabbedBlock = null;
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
            TryGrab(col);

            if(_extension == _maxLength || _forwardTimer.HasElapsed)
            {
                _animator.SetBool(ANIMATOR_IS_CLOSED, true);
                DelayedRetract();
            }
        }

        private void DelayedRetract()
        {
            _currentSpeed = 0f;
            _state = State.Idle;
            _extentSource.Stop();

            CoroutineRunner.ExecuteDelayed(_backwardDelay, () =>
            {
                _state = State.Backward;
                _extentSource = SoundManager.Play(SFX.Grappling_Return, transform.position);                
            });
        }

        // --------------------------------------------------------------------------------------------
        private Collider2D CheckCollision()
        {
            Debug.DrawLine(HeadCenter + _head.transform.up * _grabRadius,
                HeadCenter - _head.transform.up * _grabRadius, Color.green,
                Time.deltaTime);
            Debug.DrawLine(HeadCenter + _head.transform.right * _grabRadius,
                HeadCenter - _head.transform.right * _grabRadius, Color.red,
                Time.deltaTime);

            return Physics2D.OverlapCircle(HeadCenter, _grabRadius, _grabMask);
        }

        private void TryGrab(Collider2D col)
        {
            if(col == null)
                return;

            Debug.Log($"{Logger.GetPre(this)} Hit {col.name}");

            DelayedRetract();

            Grabable grab = col.GetComponent<Grabable>();
            if(grab == null)
                return;

            if(grab is BuildingBlock block)
            {
                if(block.Stack != null)
                    return;

                if(block.Hook != null && block.Hook != this)
                {
                    block.Hook.ReleaseBlock();
                }

                GrabbedBlock = block;
            }

            _animator.SetBool(ANIMATOR_IS_CLOSED, true);            
            grab.OnGrab(this);
        }

        // --------------------------------------------------------------------------------------------
        private void Retract()
        {
            ChangeExtension(0f, -1, _forwardAcceleration, _backwardSpeed);
            UpdateVisibility();

            if(_extension == 0f)
            {
                _state = State.Idle;
                _extentSource.Stop();

                if(GrabbedBlock != null)
                {
                    Lizard.Stack.ReceiveBlock(GrabbedBlock);
                    GrabbedBlock = null;
                }

                _animator.SetBool(ANIMATOR_IS_CLOSED, false);
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