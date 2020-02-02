using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class LavaChunk : Grabable
    {

        // --- Enums ------------------------------------------------------------------------------------------------------
        public enum State
        {
            Falling = 0,
            Flung = 1
        }

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private float _flingDuration = 1f;
        [SerializeField] private FloatRange _explosionRange = new FloatRange(2, 8);

        private State _state = State.Falling;
        private Vector3 _startPosition, _targetPosition;
        private float _flingT;
        private PlayerStack _targetStack;
        private ParticleSystem _trail;

        // --- Properties -------------------------------------------------------------------------------------------------

        // --- Unity Functions --------------------------------------------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();
        }

        private void OnEnable()
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _state = State.Falling;
        }

        private void Update()
        {
            switch(_state)
            {
                case State.Falling:
                    break;
                case State.Flung:
                    FlyTowardsTarget();
                    break;
            }
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------



        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        public override void OnGrab(GrapplingHook hook)
        {
            _targetStack = GameManager.GetEnemyStack(hook.Lizard.Stack);
            if(_targetStack.Blocks.Count == 0)
            {
                Debug.Log($"{Logger.GetPre(this)} Nothing to shoot at!");
                //this.Return();
                return;
            }

            base.OnGrab(hook);
            SoundManager.Play(SFX.Grab_LavaChunk, transform.position);

            _startPosition = transform.position;

            BuildingBlock block = _targetStack.Blocks.Peek();
            _targetPosition = block.transform.position + Vector3.up * block.BlockHeight;

            _trail = Instantiate(Resources.Load<ParticleSystem>("Particles/LavaChunkTrail"), this.transform, false);

            _flingT = 0f;
            _state = State.Flung;
        }

        private void FlyTowardsTarget()
        {
            _flingT = Mathf.Clamp01(_flingT + (Time.deltaTime / _flingDuration));

            float tX = _flingT;
            float tY = _flingT * _flingT;
            transform.position = new Vector3(
                x: Mathf.Lerp(_startPosition.x, _targetPosition.x, tX),
                y: Mathf.Lerp(_startPosition.y, _targetPosition.y, tY));

            if(_flingT == 1f)
            {
                HitStack();
            }
        }

        private void HitStack()
        {
            _targetStack.HitByChunk();

            Destroy(_trail.gameObject);

            SoundManager.Play(SFX.LavaChunk_Explosion, transform.position);
            ParticleSystem psSystem = Instantiate(Resources.Load<ParticleSystem>("Particles/LavaChunkExplosion"));
            psSystem.transform.position = this.transform.position;

            MegaFactory.Instance.ReturnFactoryItem(this);
        }

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}