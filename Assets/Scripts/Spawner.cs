using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class Spawner : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------
        enum SpawnType
        {
            BoulderAndBlocks = 0,
            FireChunks = 1
        }
        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private FloatRange _spawnIntervall;
        [SerializeField] private SpawnType _spawnType;
        private Delay _spawnDelay;
        // --- Properties -------------------------------------------------------------------------------------------------

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            _spawnDelay = new Delay(_spawnIntervall.GetRandom());

        }
        private void Update()
        {
            if(_spawnDelay.HasElapsed)
            {
                if(_spawnType == SpawnType.BoulderAndBlocks)
                    SpawnSomething();
                else
                    SpawnFire();

                _spawnDelay.ChangeDuration(_spawnIntervall.GetRandom());
            }
        }
        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        private void SpawnSomething()
        {
            Grabable grab = GameManager.GetRandomBlockOrBoulder();
            if(grab is BuildingBlock block)
            {
                Debug.Log($"is block!");
                block.LoadData(transform);
                //block.transform.position = transform.position;
            }
            else
                grab.transform.position = transform.position;
        }

        private void SpawnFire()
        {
            LavaChunk chunk = GameManager.GetLavaChunk();
            chunk.transform.position = transform.position;
        }
        // --- Protected/Private Methods ----------------------------------------------------------------------------------

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}