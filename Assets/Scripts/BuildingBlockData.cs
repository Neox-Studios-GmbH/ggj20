using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    [CreateAssetMenu(fileName = "BuildingBlockData", menuName = "GGJ/BlockData", order = 2)]
    public class BuildingBlockData : ScriptableObject
    {
        // --- Enums ------------------------------------------------------------------------------------------------------
        public enum SpriteOrientation
        {
            Standard = 0,
            Flipped = 1
        }
        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private Color _spriteColor;
        [SerializeField] private SpriteOrientation _spriteOrientation;
        
        // --- Properties -------------------------------------------------------------------------------------------------
        public Color BlockColor => _spriteColor;
        public SpriteOrientation Orientation => _spriteOrientation;

        // --- Constructors -----------------------------------------------------------------------------------------------

        // --- Public/Internal Methods ------------------------------------------------------------------------------------

        // --- Protected/Private Methods ----------------------------------------------------------------------------------

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}