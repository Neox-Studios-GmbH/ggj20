using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class PlayerStack : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private Players _player;
        private Stack<BuildingBlock> _blockStack;
        // --- Properties -------------------------------------------------------------------------------------------------
        public Stack<BuildingBlock> BlockStack => _blockStack;
        public int CombinedStackHeight => GetCombinedStackHeight();
        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            _blockStack = new Stack<BuildingBlock>();
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void AddBlock(BuildingBlock block)
        {
            //Debug.Log($"Adding {block.BType} with height {block.BlockUpperBounds} to stack");
            PlaceBlock(block);
        }
        public void DestroyBlock()
        {
            Debug.Log($"Boom");
            //check if block beneath is same type and destroy if its wood
        }
        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        private void PlaceBlock(BuildingBlock block)
        {
            if(_blockStack.Count == 0)
            {
                block.transform.position = transform.position;
            }
            else
            {
                BuildingBlock lastblock = _blockStack.Peek();
                Vector3 newPos = lastblock.transform.position;
                newPos.y = lastblock.BlockUpperBounds;
                block.transform.position = newPos;
            }
            ParticleSystem pSystem = Instantiate(Resources.Load<ParticleSystem>("Particles/BoxPlace"), block.transform);
            pSystem.transform.SetParent(block.transform);
            _blockStack.Push(block);
            GameManager.AddPlayerScore(_player, block.Score);
        }

        private int GetCombinedStackHeight()
        {
            int height = 0;
            foreach(BuildingBlock block in _blockStack)
            {
                height += block.Score;
            }
            return height;
        }
        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}