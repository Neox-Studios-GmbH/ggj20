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
        [SerializeField] private TechnoLizard _lizard;
        [SerializeField] private FloatRange _receiveDuration = new FloatRange(.5f, 1);

        [Header("Lemmings")]
        [SerializeField] private int _lemmingTargetAmount;
        [SerializeField] private int _lemmingSpawnAmount;
        [SerializeField] private FloatRange _lemmingSpawnDelay;

        private List<Lemming> _lemmings = new List<Lemming>();

        // --- Properties -------------------------------------------------------------------------------------------------
        public Stack<BuildingBlock> Blocks { get; private set; }

        public Players Player => _player;
        public TechnoLizard Lizard => _lizard;
        public float Height => CalculateHeight();
        public int Score => CalculateScore();

        public Vector3 NextBlockPosition
        {
            get
            {
                BuildingBlock top = GetTopBlock();
                return top != null
                    ? top.transform.position + Vector3.up * top.BlockHeight
                    : transform.position;
            }
        }

        public float TopBlockWidth => GetTopBlock()?.Bounds.extents.x ?? 1f;

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            Blocks = new Stack<BuildingBlock>();
        }

        private void Start()
        {
            SpawnLemmings();
        }

        private void SpawnLemmings()
        {
            for(int i = 0; i < _lemmings.Count; i++)
            {
                if(_lemmings[i] == null || !_lemmings[i].Active)
                    _lemmings.RemoveAt(i--);
            }

            int maxSpawnAmount = Mathf.Min(_lemmingSpawnAmount, _lemmingTargetAmount - _lemmings.Count);
            for(int i = 0; i < maxSpawnAmount; i++)
            {
                Lemming l = MegaFactory.Instance.GetFactoryItem<Lemming>(MegaFactory.FactoryType.Lemming);
                float xExtents = TopBlockWidth - .25f;
                l.transform.position = NextBlockPosition + Vector3.right * Random.Range(-xExtents, xExtents)
                    + .5f * Vector3.up;
            }

            CoroutineRunner.ExecuteDelayed(_lemmingSpawnDelay.GetRandom(), SpawnLemmings);
        }

        private BuildingBlock GetTopBlock()
        {
            while(Blocks.Count > 0)
            {
                BuildingBlock b = Blocks.Peek();
                if(b != null)
                    return b;

                Blocks.Pop();
            }

            return null;
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void ReceiveBlock(BuildingBlock block)
        {
            //Debug.Log($"Adding {block.BType} with height {block.BlockUpperBounds} to stack");            
            block.transform.SetParent(null);
            float duration = _receiveDuration.GetRandom();

            // NOTE: Block can no longer be grabbed
            block.Stack = this;
            StartCoroutine(PlaceBlockRoutine(block, NextBlockPosition, duration));
        }

        public void HitByChunk()
        {
            if(Blocks.Count == 0)
            {
                Debug.LogWarning($"{Logger.GetPre(this)} Chunk {name} was hit, but has no (more) blocks.");
                return;
            }

            if(Blocks.Peek().BType == BuildingBlock.BlockType.Unobtainium)
                return;

            BuildingBlock block = Blocks.Pop();
            MegaFactory.Instance.ReturnFactoryItem(block);

            if(block.BType == BuildingBlock.BlockType.Wood)
                HitByChunk();
        }

        public void PerishLemmings()
        {
            foreach(Lemming l in _lemmings)
                l.SwitchState(Lemming.State.Suicide);
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        private IEnumerator PlaceBlockRoutine(BuildingBlock bb, Vector2 targetPos, float duration)
        {
            Transform block = bb.transform;
            Vector2 startPos = block.position;
            Quaternion startRot = block.rotation;

            float elapsed = 0f;
            while(elapsed < duration)
            {
                yield return null;

                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                block.position = Vector2.Lerp(startPos, targetPos, t);
                if(t < .5f)
                {
                    block.rotation = Quaternion.Lerp(startRot, Quaternion.identity, 2f * t);
                }
            }

            block.position = targetPos;
            block.rotation = Quaternion.identity;

            SoundManager.Play(bb.BType == BuildingBlock.BlockType.Wood ? SFX.Place_Block_Light
                : bb.BType == BuildingBlock.BlockType.Rock ? SFX.Place_Block_Normal
                : SFX.Place_Block_Hard, block.position);

            ParticleSystem pSystem = Instantiate(Resources.Load<ParticleSystem>("Particles/BoxPlace"), block.transform, false);

            block.SetParent(this.transform);
            Blocks.Push(bb);
            GameManager.Instance.onStackChanged?.Invoke(this);
            //GameManager.AddPlayerScore(_player, block.Score);
        }

        private float CalculateHeight()
        {
            float height = 0;
            foreach(BuildingBlock block in Blocks)
            {
                height += block.BlockHeight;
            }

            return height;
        }

        private int CalculateScore()
        {
            int score = 0;
            foreach(BuildingBlock block in Blocks)
            {
                score += block.Score;
            }

            return score;
        }

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}