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
        [SerializeField] private TechnoLizard _lizardBoi;
        private Stack<BuildingBlock> _blockStack;
        private float _lizardBoiDistance;
        // --- Properties -------------------------------------------------------------------------------------------------
        public Stack<BuildingBlock> BlockStack => _blockStack;
        public int CombinedStackHeight => GetCombinedStackHeight();
        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            _blockStack = new Stack<BuildingBlock>();
            //_lizardBoiDistance = transform.position - _lizardBoi.transform.position;
            _lizardBoiDistance = Vector2.Distance(transform.position, _lizardBoi.transform.position);
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void AddBlock(BuildingBlock block)
        {
            //Debug.Log($"Adding {block.BType} with height {block.BlockUpperBounds} to stack");
            PlaceBlock(block);
        }
        public void DestroyBlock(BuildingBlock block)
        {
            if(_blockStack.Peek() == block)
            {
                Debug.Log($"Boom");
                MegaFactory.Instance.ReturnFactoryItem(_blockStack.Pop());
            }
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

            //Todo: Move LizardBoi up
            Vector2 newLizardBoiPos = new Vector2(_lizardBoi.transform.position.x, block.BlockUpperBounds + _lizardBoiDistance);
            float t = 0;

            //while(t <= 1)
            //{
            //    t = t + (2 * Time.deltaTime);
            //    Debug.Log($"{Logger.GetPre(this)} {t}");
            //    _lizardBoi.transform.position = Vector2.Lerp(_lizardBoi.transform.position, newLizardBoiPos, t * Time.deltaTime);
            //}
            _lizardBoi.transform.position = newLizardBoiPos;
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