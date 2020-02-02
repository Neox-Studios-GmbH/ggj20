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
        public struct PlayerScores
        {
            public int playerOne;
            public int playerTwo;
        }
        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private PlayerStack _stackPlayerOne;
        [SerializeField] private PlayerStack _stackPlayerTwo;

        private const int MAX_SCORE = 450;
        private PlayerScores _score;
        public Action<PlayerStack> OnStackChanged;
        // --- Properties -------------------------------------------------------------------------------------------------

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
            //_intervall -= Time.deltaTime;
            //if(_intervall <= 0)
            //{
            //    SpawnBlock();
            //    SpawnLava();
            //    _intervall = 3f;
            //}
        }
        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public static PlayerStack PlayerStackForPlayer(Players player)
        {
            return player == Players.PlayerOne ? Instance._stackPlayerOne : Instance._stackPlayerTwo;
        }

        public static Grabable GetRandomBlockOrBoulder()
        {
            return Instance._GetRandomBlockOrBoulder();
        }
        public static LavaChunk GetLavaChunk()
        {
            return Instance._GetChunk();
        }

        public static void AddPlayerScore(Players player, int score)
        {
            Instance._AddPlayerScore(player, score);
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        private void _AddPlayerScore(Players player, int score)
        {
            switch(player)
            {
                case Players.PlayerOne:
                    _score.playerOne += score;

                    break;
                case Players.PlayerTwo:
                    _score.playerTwo += score;
                    break;
            }
            if(IsGameOver())
            {
                Players _player = _score.playerOne > _score.playerTwo ? Players.PlayerOne : Players.PlayerTwo;
                Debug.Log($"Game over Bro {_player} won!");
            }
            Debug.Log($"{Logger.GetPre(this)}  ScorePlayerOne {_score.playerOne} ScorePlayerTwo {_score.playerTwo}");
        }
        private LavaChunk _GetChunk()
        {
            return MegaFactory.Instance.GetFactoryItem<LavaChunk>(MegaFactory.FactoryType.LavaChunk);
        }
        private Grabable _GetRandomBlockOrBoulder()
        {
            int randomStuff = UnityEngine.Random.Range(0, 6);
            if(randomStuff > 3)
                return MegaFactory.Instance.GetFactoryItem<Boulder>(MegaFactory.FactoryType.Boulder);

            BuildingBlock.BuildingBlockHeight blockType = (BuildingBlock.BuildingBlockHeight)randomStuff;

            //Debug.Log($"{blockType}");
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

        public bool IsGameOver()
        {
            return _score.playerOne >= MAX_SCORE || _score.playerTwo >= MAX_SCORE;
        }
        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}