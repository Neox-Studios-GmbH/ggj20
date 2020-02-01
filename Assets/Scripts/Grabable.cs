using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

namespace GGJ20
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public abstract class Grabable : MonoBehaviour, IPointerClickHandler
    {
        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [Header("Testing")]
        [SerializeField] private bool _isGrabbed;
        [SerializeField, Range(0.1f, 1.5f)] private float _fallSpeed;
        [Space]
        protected Rigidbody2D _rb;
        protected BoxCollider2D _collider;
        protected Vector3 _startPos;
        public bool IsGrabbed { get; protected set; }
        // --- Properties -------------------------------------------------------------------------------------------------

        // --- Unity Functions --------------------------------------------------------------------------------------------
        protected virtual void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = _fallSpeed;
            _startPos = transform.position;
        }
        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void Grab()
        {
            if(_isGrabbed)
            {
                Debug.Log($"Is Already grabbed!");
                return;
            }
            _isGrabbed = true;
            HandleGrab();
        }
        public void Spawn(float fallspeed, Vector2 spawnPos)
        {
            transform.position = spawnPos;
            _fallSpeed = fallspeed;
            _isGrabbed = false;
        }
        // --- Protected/Private Methods ----------------------------------------------------------------------------------   
        protected abstract void HandleGrab();
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.name == "Bounds")
            {
                Return();
            }
        }
        private void Return()
        {
            Debug.Log($"Returning to Spawn");
            _rb.velocity = Vector2.zero;
            transform.position = _startPos;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Grab();
        }
        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}