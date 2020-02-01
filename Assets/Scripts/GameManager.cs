﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private PlayerStack _stackPlayerOne;
        [SerializeField] private Transform[] _blockSpawn;
        [SerializeField] private float _minSpawnTime, _maxSpawnTime;
        [SerializeField] private BuildingBlock _blockPrefab;
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

        }
        // --- Public/Internal Methods ------------------------------------------------------------------------------------

        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        private void SpawnBlock()
        {
            BuildingBlock block = Instantiate(_blockPrefab, _blockSpawn[0]);

        }

        private void SelectRandomBlock()
        {
            System.Random r = new System.Random();
            BuildingBlock.BlockType blockType = (BuildingBlock.BlockType)r.Next(0, 2);
        }
        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}