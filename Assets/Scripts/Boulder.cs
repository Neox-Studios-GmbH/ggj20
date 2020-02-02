using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class Boulder : Grabable
    {

        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------

        // --- Properties -------------------------------------------------------------------------------------------------

        // --- Unity Functions --------------------------------------------------------------------------------------------        
        private void OnEnable()
        {
            transform.rotation = Randomizer.ZRotation();
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public override void OnGrab(GrapplingHook hook)
        {
            SoundManager.Play(SFX.Boulder_Crumble, transform.position);

            ParticleSystem psSystem = Instantiate(Resources.Load<ParticleSystem>("Particles/LavaChunkExplosion"));
            psSystem.transform.position = this.transform.position;

            Return();
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}