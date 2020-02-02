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

        private PlayerScores _score;
        public Action<PlayerStack> onStackChanged;

        // --- Properties -------------------------------------------------------------------------------------------------
        public bool GameOver { get; private set; }

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

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public static PlayerStack GetStack(Players player)
        {
            return player == Players.PlayerOne ? Instance._stackPlayerOne : Instance._stackPlayerTwo;
        }

        public static PlayerStack GetEnemyStack(PlayerStack stack)
        {
            return stack.Player == Players.PlayerOne ? Instance._stackPlayerTwo : Instance._stackPlayerOne;
        }

        // --------------------------------------------------------------------------------------------
        public static LavaChunk GetLavaChunk()
        {
            return Instance._GetLavaChunk();
        }

        public static Grabable GetRandomBlockOrBoulder()
        {
            return Instance._GetRandomBlockOrBoulder();
        }

        //public static void AddPlayerScore(Players player, int score)
        //{
        //    Instance._AddPlayerScore(player, score);
        //}

        // --------------------------------------------------------------------------------------------
        public static void OnPlayerWon(Players player)
        {
            Instance._OnPlayerWon(player);            
        }

        public static void OnDraw()
        {
            Instance._OnPlayerWon(Players.PlayerOne);
            Instance._OnPlayerWon(Players.PlayerTwo);
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        //private void _AddPlayerScore(Players player, int score)
        //{
        //    switch(player)
        //    {
        //        case Players.PlayerOne:
        //            _score.playerOne += score;
        //            break;

        //        case Players.PlayerTwo:
        //            _score.playerTwo += score;
        //            break;
        //    }

        //    Debug.Log($"{Logger.GetPre(this)}  ScorePlayerOne {_score.playerOne} ScorePlayerTwo {_score.playerTwo}");
        //}

        // --------------------------------------------------------------------------------------------
        private LavaChunk _GetLavaChunk()
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

        // --------------------------------------------------------------------------------------------
        private void _OnPlayerWon(Players player)
        {
            if(GameOver)
                return;

            Debug.Log($"{Logger.GetPre(this)} '{player}' has won!");

            PlayerStack loserStack = player == Players.PlayerOne
                ? _stackPlayerTwo : _stackPlayerOne;

            Rigidbody2D stackRb = loserStack.gameObject.AddComponent<Rigidbody2D>();
            stackRb.AddTorque(10f, ForceMode2D.Impulse);

            Rigidbody2D lizardRb = loserStack.Lizard.gameObject.AddComponent<Rigidbody2D>();
            lizardRb.AddTorque(10f, ForceMode2D.Impulse);

            GameOver = true;
        }


        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}