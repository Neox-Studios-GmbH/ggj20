﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
	public class DeathZone : MonoBehaviour
	{
		// --- Enums ------------------------------------------------------------------------------------------------------

		// --- Nested Classes ---------------------------------------------------------------------------------------------

		// --- Fields -----------------------------------------------------------------------------------------------------
		[SerializeField] private BoxCollider2D _collider;

		// --- Properties -------------------------------------------------------------------------------------------------

		// --- Unity Functions --------------------------------------------------------------------------------------------
		private void OnCollisionEnter2D(Collision2D collision)
		{
			
		}

		// --- Public/Internal Methods ------------------------------------------------------------------------------------

		// --- Protected/Private Methods ----------------------------------------------------------------------------------

		// --------------------------------------------------------------------------------------------
	}

	// **************************************************************************************************************************************************
}