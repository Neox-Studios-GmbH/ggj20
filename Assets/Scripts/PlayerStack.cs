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
        [SerializeField] private FloatRange _receiveDuration = new FloatRange(.5f, 1);
        private Stack<BuildingBlock> _blockStack;
        private float _lizardBoiDistance;
        private Delay _delay;

        // --- Properties -------------------------------------------------------------------------------------------------
        public Stack<BuildingBlock> BlockStack => _blockStack;
        public int _CombinedStackScore => CombinedStackScore();
        public float CombinedStackHeight => CombinedHeight();
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
            float duration = _receiveDuration.GetRandom();
            //Debug.Log($"{duration}");
            if(_blockStack.Count == 0)
            {
                // StartCoroutine(PlaceBlockRoutine(block.transform, transform.position, duration));
                StartCoroutine(PlaceBlockRoutine(block.transform, transform.position, duration));
            }
            else
            {
                BuildingBlock lastblock = _blockStack.Peek();
                Vector3 newPos = lastblock.transform.position;
                newPos.y = lastblock.BlockUpperBounds;
                StartCoroutine(PlaceBlockRoutine(block.transform, newPos, duration));
            }

            CoroutineRunner.ExecuteDelayed(duration, () =>
            {
                ParticleSystem pSystem = Instantiate(Resources.Load<ParticleSystem>("Particles/BoxPlace"), block.transform);
                pSystem.transform.SetParent(block.transform);
                _blockStack.Push(block);
                Vector2 newLizardBoiPos = new Vector2(_lizardBoi.transform.position.x, block.BlockUpperBounds + _lizardBoiDistance);
                StartCoroutine(PlaceBlockRoutine(_lizardBoi.transform, newLizardBoiPos, _receiveDuration.GetRandom()));

            });

            GameManager.Instance.OnStackChanged?.Invoke(this);

            GameManager.AddPlayerScore(_player, block.Score);
        }
        private IEnumerator PlaceBlockRoutine(Transform from, Vector2 to, float duration)
        {
            float t = 0f;
            Vector2 start = from.position;
            while(t <= duration)
            {
                t = (t + Time.deltaTime);
                Vector2 newpos = Vector2.Lerp(start, to, t / duration);
                from.position = newpos;
                yield return null;
            }

        }
        private float CombinedHeight()
        {
            float height = 0;
            foreach(BuildingBlock block in _blockStack)
            {
                height += block.BlockHeight;
            }
            return height;
        }
        private int CombinedStackScore()
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