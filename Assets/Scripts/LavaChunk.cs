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

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private FloatRange _explosionRange = new FloatRange(2, 8);
        private Action DoExplosionStuff;
        // --- Properties -------------------------------------------------------------------------------------------------

        // --- Unity Functions --------------------------------------------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------

        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        protected override void HandleGrab(Players player)
        {
            PlayerStack stack = GameManager.PlayerStackForPlayer(player);
            //Debug.Log($"{Logger.GetPre(this)} hit Lava Chunk, now throwing at {stack.BlockStack.Peek().BType} ");
            if(stack.BlockStack.Count == 0)
            {
                Debug.Log($"{Logger.GetPre(this)} Nothing to shoot at!");
                return;
            }
            BuildingBlock block = stack.BlockStack.Peek();
            Vector2 dir = block.transform.position - transform.position;
            dir = dir.normalized;
            _rb.velocity = Vector2.zero;
            _rb.gravityScale = 0;
            _rb.AddForce(dir * _explosionRange.GetRandom(), ForceMode2D.Impulse);
            DoExplosionStuff += () =>
            {
                if(block.BType == BuildingBlock.BlockType.Wood)
                    stack.DestroyBlock(block);
            };
            //transform.forward = dir;

            // _rb.AddForceAtPosition(transform.forward * 20, new Vector2(block.transform.position.x, block.transform.position.y), ForceMode2D.Impulse);

        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            BuildingBlock block = collision.gameObject.GetComponent<BuildingBlock>();

            if(block != null)
            {
                Vector2 pos = collision.contacts[0].point;
                ParticleSystem psSystem = Instantiate(Resources.Load<ParticleSystem>("Particles/LavaChunkExplosion"));
                psSystem.transform.position = pos;
                DoExplosionStuff?.Invoke();


                MegaFactory.Instance.ReturnFactoryItem(this);
            }
        }
        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}