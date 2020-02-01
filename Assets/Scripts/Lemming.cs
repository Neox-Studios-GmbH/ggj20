﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class Lemming : FactoryItem
    {
        // --- Enums ------------------------------------------------------------------------------------------------------
        public enum State
        {
            Idle = 0,
            Run = 1,
            Suicide = 2
        }

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [Header("Suicide")]
        [SerializeField] private float _suicideJumpStrength = 1f;
        [SerializeField, Range(1, 5)] private int _suicidalTendency = 1;
        [SerializeField] private FloatRange _suicideAngleRange = new FloatRange(30f, 80f);
        [SerializeField] private FloatRange _suicideTorqueRange = new FloatRange(1f, 10f);

        [SerializeField] private float _runSpeed = 2f;
        [SerializeField] private CapsuleCollider2D _collider;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Animator _animator;
        [SerializeField] private LayerMask _groundingMask;

        [Header("Cheers")]
        [SerializeField] private float _randomCheerChance;
        [SerializeField] private float _randomCheerInterval;

        private State _state = State.Idle;

        private int _runningDirection = 1;
        private float _suicideRate;
        private Delay _cheerCooldown;

        private readonly FloatRange SUICIDE_RANGE = new FloatRange(0.005f, 0.02f);
        private readonly int ANIM_IS_FLAILING = Animator.StringToHash("IsFlailing");
        private readonly int ANIM_SUICIDE = Animator.StringToHash("Suicide");

        // --- Properties -------------------------------------------------------------------------------------------------
        public int SuicidalTendency { get => _suicidalTendency; set { _suicidalTendency = Mathf.Clamp(value, 1, 5); } }

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Start()
        {
            _runningDirection = Random.Range(0, 2) * 2 - 1;
            _renderer.flipX = _runningDirection == -1;

            _cheerCooldown = new Delay(_randomCheerInterval);
            _suicideRate = SUICIDE_RANGE.GetRandom() * _suicidalTendency;

            SwitchState(State.Run);
        }

        private void Update()
        {
            switch(_state)
            {
                case State.Idle:
                    // TODO: Locate end of escalator and switch to Run
                    break;
                case State.Run:
                    Run();
                    CheckForEdges();
                    Cheer();
                    break;
                case State.Suicide:
                    break;
            }
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------        


        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        private void SwitchState(State state)
        {
            _state = state;
            switch(_state)
            {
                case State.Idle:
                    _animator.SetBool(ANIM_IS_FLAILING, false);
                    break;
                case State.Run:
                    _animator.SetBool(ANIM_IS_FLAILING, true);
                    break;
                case State.Suicide:
                    Suicide();
                    break;
            }
        }

        private void CheckForEdges()
        {
            Bounds bounds = _collider.bounds;
            Vector2 p1 = bounds.min;
            if(_runningDirection == 1)
            {
                p1 += Vector2.right * bounds.size.x;
            }

            Vector2 p2 = p1 + Vector2.down;
            RaycastHit2D groundHit = Physics2D.Linecast(p1, p2, _groundingMask);
            Debug.DrawLine(p1, p2, Color.green, Time.deltaTime);

            if(groundHit.collider == null)
            {
                if(Random.Range(0f, 1f) < _suicideRate)
                {
                    SwitchState(State.Suicide);
                }
                else
                {
                    Flip();
                }
            }
        }

        private void Suicide()
        {
            _animator.SetTrigger(ANIM_SUICIDE);

            _rigidbody.constraints = RigidbodyConstraints2D.None;
            Vector2 suicideDirection = Quaternion.Euler(0f, 0f, _suicideAngleRange.GetRandom() * _runningDirection)
                * (Vector2.right * _runningDirection * _suicideJumpStrength);

            _rigidbody.AddForce(suicideDirection, ForceMode2D.Impulse);
            _rigidbody.AddTorque(_suicideTorqueRange.GetRandom() * _suicideJumpStrength * _runningDirection,
                ForceMode2D.Impulse);

            SoundManager.PlayRandomAhh(transform.position);
        }

        private void Flip()
        {
            _renderer.flipX = !_renderer.flipX;
            _runningDirection *= -1;
        }

        private void Run()
        {
            transform.position += Vector3.right * _runSpeed * _runningDirection * Time.deltaTime;
        }

        private void Cheer()
        {
            if(_state == State.Suicide)
                return;

            if(_cheerCooldown.HasElapsed)
            {
                _cheerCooldown.Reset();

                if(Random.Range(0f, 1f) < _randomCheerChance)
                {
                    SoundManager.PlayRandomCheer(transform.position);
                }
            }
        }

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}