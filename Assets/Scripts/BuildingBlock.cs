using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class BuildingBlock : Grabable
    {
        // --- Enums ------------------------------------------------------------------------------------------------------
        public enum BlockType
        {
            Wood = 0,
            Rock = 1,
            Unobtainium = 2
        }

        public enum BuildingBlockHeight
        {
            Small = 1,
            Medium = 2,
            Large = 3
        }
        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private BuildingBlockHeight _blockHeight;
        [SerializeField] private BlockType _blockType;
        [SerializeField] private SpriteRenderer _stairSprite;
        // --- Properties -------------------------------------------------------------------------------------------------
        public float BlockUpperBounds => transform.position.y + _collider.bounds.size.y;
        public float BlockHeight => _collider.size.y;
        public BlockType Type => _blockType;
        // --- Unity Functions --------------------------------------------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------

        // --- Protected/Private Methods ----------------------------------------------------------------------------------

        protected override void HandleGrab()
        {
            _rb.velocity = Vector2.zero;
            _rb.isKinematic = true;
            Debug.Log($"Grabbing {this._blockHeight}");
            GameManager.StackPlayerOne.AddBlock(this);

        }
        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}