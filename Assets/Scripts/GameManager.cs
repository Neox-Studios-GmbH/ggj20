using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public enum Players
    {
        PlayerOne = 0,
        PlayerTwo = 1
    }
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private PlayerStack _stackPlayerOne;
        [SerializeField] private PlayerStack _stackPlayerTwo;
        [SerializeField] private Transform[] _blockSpawn;
        [SerializeField] private Transform _playerOneLavaSpawn;
        [SerializeField] private Transform _playerTwoLavaSpawn;
        [SerializeField] private float _minSpawnTime, _maxSpawnTime;
        [SerializeField] private float _intervall;
        // --- Properties -------------------------------------------------------------------------------------------------
        public static PlayerStack StackPlayerOne => Instance._stackPlayerOne;
        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this);
            }
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        private void Update()
        {
            _intervall -= Time.deltaTime;
            if(_intervall <= 0)
            {
                SpawnBlock();
                SpawnLava();
                _intervall = 3f;
            }
        }
        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public static PlayerStack PlayerStackForPlayer(Players player)
        {
            return player == Players.PlayerOne ? Instance._stackPlayerOne : Instance._stackPlayerTwo;
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        private void SpawnBlock()
        {
            Grabable block = GetRandomBlockOrBoulder();

            Vector3 pos = _blockSpawn[UnityEngine.Random.Range(0, 1)].position;
            block.transform.position = pos;

        }
        private void SpawnLava()
        {
            Grabable grab = GetChunkOrBoulder();

            //System.Random r = new System.Random();

            grab.transform.position = UnityEngine.Random.Range(0, 2) == 1 ? _playerOneLavaSpawn.position : _playerTwoLavaSpawn.position;
        }
        private LavaChunk GetChunkOrBoulder()
        {
            return MegaFactory.Instance.GetFactoryItem<LavaChunk>(MegaFactory.FactoryType.LavaChunk);

        }
        private Grabable GetRandomBlockOrBoulder()
        {
            int randomStuff = UnityEngine.Random.Range(0, 6);
            if(randomStuff > 3)
                return MegaFactory.Instance.GetFactoryItem<Boulder>(MegaFactory.FactoryType.Boulder);

            BuildingBlock.BuildingBlockHeight blockType = (BuildingBlock.BuildingBlockHeight)randomStuff;

            Debug.Log($"{blockType}");
            switch(blockType)
            {
                case BuildingBlock.BuildingBlockHeight.Small:
                default:
                    return MegaFactory.Instance.GetFactoryItem<BuildingBlock>(MegaFactory.FactoryType.SmallBlock);
                case BuildingBlock.BuildingBlockHeight.Medium:
                    return MegaFactory.Instance.GetFactoryItem<BuildingBlock>(MegaFactory.FactoryType.MediumBlock);
                case BuildingBlock.BuildingBlockHeight.Large:
                    return MegaFactory.Instance.GetFactoryItem<BuildingBlock>(MegaFactory.FactoryType.LargeBlock);
            }
        }
        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}