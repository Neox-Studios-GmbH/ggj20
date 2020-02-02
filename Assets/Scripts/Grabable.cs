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
        //[SerializeField, Range(0.1f, 1.5f)] private float _fallSpeed;
        [SerializeField] private FloatRange _fallSpeed = new FloatRange(0.125f, .65f);
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
            _rb.gravityScale = _fallSpeed.GetRandom();
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
        public void Grab(Transform other)
        {
            if(_isGrabbed)
            {
                Debug.Log($"Is Already grabbed!");
                return;
            }
            _isGrabbed = true;
            SetDefaultValues();
            _rb.isKinematic = true;
            transform.position = other.position;
            transform.SetParent(other);
            HandleGrab();
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------   
        protected abstract void HandleGrab(Players player);
        protected abstract void HandleGrab();
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.name == "Bounds")
            {
                Return();
            }
        }
        private void SetDefaultValues()
        {    //Debug.Log($"Returning to Spawn");
            _rb.velocity = Vector2.zero;
            _rb.rotation = 0;
        }
        private void Return()
        {
            SetDefaultValues();
            _isGrabbed = false;
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