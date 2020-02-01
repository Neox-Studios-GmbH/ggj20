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
            Small = 0,
            Medium = 1,
            Large = 2
        }
        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private BuildingBlockHeight _blockHeight;
        [SerializeField] private BlockType _blockType;
        [SerializeField] private SpriteRenderer _stairSprite;
        // --- Properties -------------------------------------------------------------------------------------------------
        public float BlockUpperBounds => transform.position.y + _collider.bounds.size.y;
        public float BlockHeight => _collider.size.y;
        public BlockType BType => _blockType;
        // --- Unity Functions --------------------------------------------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------

        // --- Protected/Private Methods ----------------------------------------------------------------------------------

        protected override void HandleGrab(Players player)
        {
            _rb.velocity = Vector2.zero;
            _rb.isKinematic = true;
            Debug.Log($"Grabbing {this._blockHeight}");
            GameManager.PlayerStackForPlayer(player).AddBlock(this);

        }
        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}