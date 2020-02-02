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
        [SerializeField] private FloatRange _receiveDuration = new FloatRange(1, 4);
        private Stack<BuildingBlock> _blockStack;
        private float _lizardBoiDistance;
        private Delay _delay;

        // --- Properties -------------------------------------------------------------------------------------------------
        public Stack<BuildingBlock> BlockStack => _blockStack;
        public int CombinedStackHeight => GetCombinedStackHeight();
        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            _delay = new Delay(0f);
            _blockStack = new Stack<BuildingBlock>();
            //_lizardBoiDistance = transform.position - _lizardBoi.transform.position;
            if(_lizardBoi == null)
                return;
            _lizardBoiDistance = Vector2.Distance(transform.position, _lizardBoi.transform.position);
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void ReceiveBlock(BuildingBlock block)
        {
            //Debug.Log($"Adding {block.BType} with height {block.BlockUpperBounds} to stack");
            block.transform.SetParent(null);
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

            _lizardBoi.transform.position = newLizardBoiPos;

            GameManager.Instance.OnStackChanged?.Invoke(this);

            GameManager.AddPlayerScore(_player, block.Score);
        }
        private IEnumerator PlaceBlockRoutine()
        {

            yield return null;
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