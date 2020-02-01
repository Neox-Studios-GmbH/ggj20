using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

namespace GGJ20
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public abstract class Grabable : FactoryItem, IPointerEnterHandler, IPointerExitHandler
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
        private bool _isHovering;
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
        private void Update()
        {
            if(!_isHovering)
                return;
            if(Input.GetKeyDown(KeyCode.Mouse0))
                Grab(Players.PlayerOne);
            if(Input.GetKeyDown(KeyCode.Mouse1))
                Grab(Players.PlayerTwo);
        }
        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void Grab(Players player)
        {
            if(_isGrabbed)
            {
                Debug.Log($"Is Already grabbed!");
                return;
            }
            _isGrabbed = true;
            HandleGrab(player);
        }
        public void Spawn(float fallspeed, Vector2 spawnPos)
        {
            transform.position = spawnPos;
            _fallSpeed = fallspeed;
            _isGrabbed = false;
        }
        // --- Protected/Private Methods ----------------------------------------------------------------------------------   
        protected abstract void HandleGrab(Players player);
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
            MegaFactory.Instance.ReturnFactoryItem(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
        }

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}