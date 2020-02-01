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
        private Stack<BuildingBlock> _blockStack;
        // --- Properties -------------------------------------------------------------------------------------------------
        public Stack<BuildingBlock> BlockStack => _blockStack;
        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            _blockStack = new Stack<BuildingBlock>();
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void AddBlock(BuildingBlock block)
        {
            Debug.Log($"Adding {block.BType} with height {block.BlockUpperBounds} to stack");
            PlaceBlock(block);
        }
        public void DestroyBlock()
        {

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
            _blockStack.Push(block);
        }
        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}