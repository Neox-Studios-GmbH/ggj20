using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class Lemming : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [Header("Suicide")]
        [SerializeField] private float _suicideJumpStrengh = 1f;
        [SerializeField] private float _suicideRate = 0.05f;

        [SerializeField] private float _runSpeed = 2f;
        [SerializeField] private CapsuleCollider2D _collider;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private LayerMask _groundingMask;

        [Header("Cheers")]
        [SerializeField] private float _randomCheerChance;
        [SerializeField] private float _randomCheerInterval;

        private int _runningDirection = 1;
        private bool _isSuiciding = false;
        private float _cheerCooldown;
        // --- Properties -------------------------------------------------------------------------------------------------

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Start()
        {
            _cheerCooldown = _randomCheerInterval;
            _runningDirection = UnityEngine.Random.Range(0, 2)*2 - 1;
        }
        private void Update()
        {
            if(_isSuiciding)
                return;

            CheckForEdges();
            Run();
            Cheer();
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------

        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        private void CheckForEdges()
        {
            Vector3 startLeft = transform.position + new Vector3(-_collider.size.x/2 * transform.localScale.x, -_collider.size.y/2 * transform.localScale.y, 0);
            Vector3 startRight = transform.position + new Vector3(_collider.size.x/2 * transform.localScale.x, -_collider.size.y/2 * transform.localScale.y, 0);

            RaycastHit2D hitLeft = Physics2D.Linecast(startLeft, startLeft + new Vector3(0, -1, 0), _groundingMask);
            RaycastHit2D hitRight = Physics2D.Linecast(startRight, startRight + new Vector3(0, -1, 0), _groundingMask);

            //Debug.DrawLine(startLeft, startLeft + new Vector3(0, -1, 0), Color.green, 0.1f);
            //Debug.DrawLine(startRight, startRight + new Vector3(0, -1, 0), Color.red, 0.1f);

            if(hitRight.collider != null && hitLeft.collider != null) 
            {
                return;
            }

            if(UnityEngine.Random.Range(0f, 1f) - _suicideRate < 0)
            {
                Suicide();
                return;
            }

            Flip();
        }

        private void Suicide()
        {
            _isSuiciding = true;
            _rigidbody.constraints = RigidbodyConstraints2D.None;
            _rigidbody.AddForceAtPosition(
                new Vector2(_suicideJumpStrengh * _runningDirection / 2, _suicideJumpStrengh * 3),
                (Vector2)transform.position - _collider.size/2,
                ForceMode2D.Impulse);

            //TODO: Scream Sound
        }

        private void Flip()
        {
            _renderer.flipX = !_renderer.flipX;
            _runningDirection = _runningDirection == 1 ? -1 : 1;
        }

        private void Run()
        {
            transform.position += new Vector3(Time.deltaTime * _runningDirection * _runSpeed, 0, 0);
        }

        private void Cheer()
        {
            _cheerCooldown = Mathf.Max(0f, _cheerCooldown - Time.deltaTime);

            if(_cheerCooldown == 0f)
            {
                _cheerCooldown = _randomCheerInterval;

                if(UnityEngine.Random.Range(0f,1f) - _randomCheerChance > 0)
                {
                    return;
                }

                //TODO: Cheer Sound
            }
        }

        public void ButtonClick()
        {
            Suicide();
        }
        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}