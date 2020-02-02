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
        [Header("Grabable")]
        [SerializeField] private FloatRange _fallSpeed = new FloatRange(0.125f, .65f);

        protected Rigidbody2D _rb;
        protected BoxCollider2D _collider;
        protected Vector3 _startPos;

        private const float SPLASH_CHANCE = .33f;

        // --- Properties -------------------------------------------------------------------------------------------------

        // --- Unity Functions --------------------------------------------------------------------------------------------
        protected virtual void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _rb = GetComponent<Rigidbody2D>();

            _rb.gravityScale = _fallSpeed.GetRandom();
            _startPos = transform.position;
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public virtual void OnGrab(GrapplingHook hook)
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.velocity = Vector2.zero;
            _rb.rotation = 0f;
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------   
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(!Active)
                return;

            if(collision.name == "Bounds")
            {
                // NOTE: Don't clear blocks that were built!
                if(this is BuildingBlock bb && bb.Stack != null)
                {
                    //bb.gameObject.SetActive(false);
                    //bb.DisableSprites();
                    return;
                }

                if(Randomizer.Chance(SPLASH_CHANCE))
                {
                    ParticleSystem ps = Instantiate(Resources.Load<ParticleSystem>("Particles/LavaBlob"));
                    ps.transform.position = transform.position;

                    SFX sfx = Randomizer.Pick(SFX.Lava_Splash, SFX.Lava_Plop);
                    SoundManager.Play(sfx, transform.position + Vector3.up);
                }

                Return();
            }
        }

        protected void SetDefaultValues()
        {
            //Debug.Log($"Returning to Spawn");
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.velocity = Vector2.zero;
            _rb.rotation = 0f;
        }

        protected void Return()
        {
            SetDefaultValues();
            MegaFactory.Instance.ReturnFactoryItem(this);
        }

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}