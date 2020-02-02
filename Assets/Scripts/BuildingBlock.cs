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
            Wood = 1,
            Rock = 2,
            Unobtainium = 3
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
        [SerializeField] private SpriteRenderer _mainSprite;
        // --- Properties -------------------------------------------------------------------------------------------------
        public float BlockUpperBounds => transform.position.y + _collider.bounds.size.y;
        public float BlockHeight => _collider.size.y;
        public int Score => (int)_blockType * 5;
        public BlockType BType => _blockType;
        // --- Unity Functions --------------------------------------------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void LoadData(Transform position)
        {
            BuildingBlockData data = GetRandomBlockData();
            if(data == null)
            {
                Debug.Log($"Theres no data!");
                return;
            }
            transform.position = position.position;
            Debug.Log($"Loading Data {data.BlockColor} {data._SpriteOrientation}");
            _mainSprite.color = data.BlockColor;
            _stairSprite.color = data.BlockColor;

            _stairSprite.flipX = data._SpriteOrientation == BuildingBlockData.SpriteOrientation.Flipped;


        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        private BuildingBlockData GetRandomBlockData()
        {
            BuildingBlockData data;
            int random = UnityEngine.Random.Range(0, 5);
            if(random == 0)
            {
                _blockType = BlockType.Rock;
                data = Resources.Load<BuildingBlockData>("BlockData/StoneBlocks");
            }
            else if(random == 1)
            {
                _blockType = BlockType.Unobtainium;
                data = Resources.Load<BuildingBlockData>("BlockData/UnobtainiumBlocks");
            }
            else
            {
                _blockType = BlockType.Wood;
                data = Resources.Load<BuildingBlockData>("BlockData/WoodBlocks");
            }

            return data;
        }

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