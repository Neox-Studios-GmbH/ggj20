using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class GameManager : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        private GameManager _instance;


        [SerializeField] private float _lavaSpeed;

        // --- Properties -------------------------------------------------------------------------------------------------
        public GameManager Instance
        {
            get
            {
                if(_instance != this)
                {
                    Destroy(_instance);
                }
                if(_instance == null)
                {
                    _instance = Instantiate(this);
                }
                return _instance;
            }
        }

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------

        // --- Protected/Private Methods ----------------------------------------------------------------------------------

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}