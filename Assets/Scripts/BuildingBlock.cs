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
        [Header("Building Block")]
        [SerializeField] private BuildingBlockHeight _blockHeight;
        [SerializeField] private BlockType _blockType;
        [SerializeField] private SpriteRenderer _stairSprite;
        [SerializeField] private SpriteRenderer _mainSprite;
        [SerializeField] private FloatRange _rotationRange = new FloatRange(-30f, 30f);

        private float _rotationSpeed;

        // --- Properties -------------------------------------------------------------------------------------------------
        public BlockType BType => _blockType;
        public Bounds Bounds => _collider.bounds;
        public float BlockHeight => _collider.bounds.size.y;
        public int Score => (int)_blockType * 5;

        public GrapplingHook Hook { get; private set; }
        public PlayerStack Stack { get; set; }

        // --- Unity Functions --------------------------------------------------------------------------------------------        
        private void OnEnable()
        {
            transform.rotation = Randomizer.ZRotation();
            _rotationSpeed = _rotationRange.GetRandom();

            _rb.bodyType = RigidbodyType2D.Dynamic;
        }

        private void FixedUpdate()
        {
            if(_rb.bodyType == RigidbodyType2D.Dynamic)
            {
                transform.RotateAround(_collider.bounds.center, Vector3.forward, _rotationSpeed * Time.fixedDeltaTime);
            }
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void LoadData(Transform spawner)
        {
            BuildingBlockData data = GetRandomBlockData();
            if(data == null)
            {
                Debug.Log($"Theres no data!");
                return;
            }

            transform.position = spawner.position;

            //Debug.Log($"Loading Data {data.BlockColor} {data.Orientation}");
            _mainSprite.color = data.BlockColor;
            _stairSprite.color = data.BlockColor;

            // TODO: Make LemmingEscalators work with flipping
            //_stairSprite.flipX = data.Orientation == BuildingBlockData.SpriteOrientation.Flipped;
        }

        public void DisableSprites()
        {
            _mainSprite.enabled = _stairSprite.enabled = false;
        }

#if UNITY_EDITOR
        private void OnMouseDown()
        {
            SetDefaultValues();
            _rb.isKinematic = true;
            GameManager.GetStack(Players.PlayerTwo).ReceiveBlock(this);
        }
#endif

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

        public override void OnGrab(GrapplingHook hook)
        {
            base.OnGrab(hook);

            this.Hook = hook;

            transform.SetParent(hook.Head);
            transform.position = hook.HeadCenter - .5f * transform.up * BlockHeight;

            SoundManager.Play(BType == BlockType.Wood ? SFX.Grab_Block_Light
                : BType == BlockType.Rock ? SFX.Grab_Block_Normal
                : SFX.Grab_Block_Hard, transform.position);
        }

        // --------------------------------------------------------------------------------------------
    }



    // **************************************************************************************************************************************************
}