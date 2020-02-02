using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public static class Randomizer
    {
        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------

        // --- Properties -------------------------------------------------------------------------------------------------

        // --- Constructors -----------------------------------------------------------------------------------------------


        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public static bool CoinToss()
        {
            return Random.Range(0, 1) == 0;
        }

        public static int PlusMinusOne()
        {
            return Random.Range(0, 2) * 2 - 1;
        }

        public static Quaternion ZRotation()
        {
            return Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        }

        public static bool Chance(float chance)
        {
            return Random.Range(0f, 1f) < chance;
        }

        public static T Pick<T>(params T[] elements)
        {
            return elements[Random.Range(0, elements.Length)];
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}