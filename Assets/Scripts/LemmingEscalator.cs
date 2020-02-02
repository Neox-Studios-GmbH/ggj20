using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class LemmingEscalator : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private Collider2D _entrance;
        [SerializeField] private Transform _exit;
        [SerializeField] private Animator _animator;

        int _counter = 00;

        private readonly int ANIMATOR_HAS_PASSENGERS = Animator.StringToHash("HasPassengers");

        // --- Properties -------------------------------------------------------------------------------------------------
        public Vector3 TargetPosition => _exit.position;

        // --- Unity Functions --------------------------------------------------------------------------------------------        

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void Enter()
        {
            _counter++;
            _animator.SetBool(ANIMATOR_HAS_PASSENGERS, true);
        }

        public void Exit()
        {
            _counter--;

            if(_counter == 0)
            {
                _animator.SetBool(ANIMATOR_HAS_PASSENGERS, false);
            }
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}