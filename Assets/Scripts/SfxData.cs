using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
	[CreateAssetMenu(fileName = "SfxData", menuName ="GGJ/SfxData", order = 1)]
	public class SfxData : ScriptableObject
	{
		// --- Enums ------------------------------------------------------------------------------------------------------

		// --- Nested Classes ---------------------------------------------------------------------------------------------
		
		// --- Fields -----------------------------------------------------------------------------------------------------
		[SerializeField] private AudioClip _clip = null;
		[SerializeField] private float _volume = 1f;
		[SerializeField] private FloatRange _pitch = new FloatRange(1f, 1f);

		// --- Properties -------------------------------------------------------------------------------------------------
		public AudioClip Clip => _clip;
		public float Volume => _volume;
		public float RandomPitch => _pitch.GetRandom();


		// --- Unity Functions --------------------------------------------------------------------------------------------
		private void Awake()
		{
			
		}

		// --- Public/Internal Methods ------------------------------------------------------------------------------------
		

		// --- Protected/Private Methods ----------------------------------------------------------------------------------
		
		// --------------------------------------------------------------------------------------------
	}

	// **************************************************************************************************************************************************
}