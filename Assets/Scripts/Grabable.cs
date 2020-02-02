using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

namespace GGJ20
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public abstract class Grabable : FactoryItem
    {
        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [Header("Testing")]
        [SerializeField] protected bool _isGrabbed;
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


        }
        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        //public void Grab(Transform other)
        //{
        //    if(_isGrabbed)
        //    {
        //        Debug.Log($"Is Already grabbed!");
        //        return;
        //    }
        //    _isGrabbed = true;
        //    SetDefaultValues();
        //    _rb.isKinematic = true;
        //    transform.position = other.position;
        //    transform.SetParent(other);
        //    HandleGrab();
        //}
        public void Grab(GrapplingHook hook)
        {
            HandleGrab(hook);
        }
        // --- Protected/Private Methods ----------------------------------------------------------------------------------   
        private void Grab(Players player)
        {
            if(_isGrabbed)
            {
                Debug.Log($"Is Already grabbed!");
                return;
            }
            _isGrabbed = true;
            HandleGrab(player);
        }
        protected abstract void HandleGrab(Players player);
        protected virtual void HandleGrab(GrapplingHook hook)
        {

        }
        protected virtual void HandleGrab()
        {
            Debug.Log($"Do grab stuff");
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.name == "Bounds")
            {
                Return();
            }
        }
        protected void SetDefaultValues()
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


        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}