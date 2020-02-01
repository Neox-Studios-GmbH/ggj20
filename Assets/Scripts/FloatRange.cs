using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
	[System.Serializable]
	public class FloatRange
	{
		// --- Enums ------------------------------------------------------------------------------------------------------

		// --- Nested Classes ---------------------------------------------------------------------------------------------

		// --- Fields -----------------------------------------------------------------------------------------------------
		[SerializeField] private float _min;
		[SerializeField] private float _max;

		// --- Properties -------------------------------------------------------------------------------------------------
		public float Min
		{
			get => _min;
			set
			{
				_min = value;
				if(_min > _max)
					_max = value;
			}
		}

		public float Max
		{
			get => _max;
			set
			{
				_max = Mathf.Max(_min, value);
			}
		}

		// --- Constructors -----------------------------------------------------------------------------------------------
		public FloatRange(float min, float max)
		{
			_min = min;
			_max = max;
		}

		// --- Public/Internal Methods ------------------------------------------------------------------------------------
		public float GetRandom()
		{
			return Random.Range(_min, _max);
		}

		public float Lerp(float t)
		{
			return Mathf.Lerp(_min, _max, t);
		}

		public float InverseLerp(float input)
		{			
			return Mathf.InverseLerp(_min, _max, input);
		}

		// --- Protected/Private Methods ----------------------------------------------------------------------------------

		// --------------------------------------------------------------------------------------------
	}

	// **************************************************************************************************************************************************
}