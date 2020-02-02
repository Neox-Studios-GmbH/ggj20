using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
	public class RandomRotation : MonoBehaviour
	{
		// --- Enums ------------------------------------------------------------------------------------------------------

		// --- Nested Classes ---------------------------------------------------------------------------------------------

		// --- Fields -----------------------------------------------------------------------------------------------------
		[SerializeField] private FloatRange _rotationSpeed = new FloatRange(90f, 270f);
		[SerializeField] private FloatRange _switchDuration = new FloatRange(1f, 5f);
		[SerializeField, Min(0f)] private float _switchPauseDuration;

		private bool _clockwise, isPaused;
		private float _currentSpeed;
		private Delay _switchCooldown;

		// --- Properties -------------------------------------------------------------------------------------------------
		private int Direction => _clockwise ? 1 : -1;

		// --- Unity Functions --------------------------------------------------------------------------------------------
		private void Awake()
		{
			_clockwise = Randomizer.CoinToss();
			_switchCooldown = new Delay(0f);
		}

		private void FixedUpdate()
		{
			if(_switchCooldown.HasElapsed)
			{
				if(!isPaused && _switchPauseDuration > 0f)
				{
					Pause();	
				}
				else
				{					
					Switch();
				}
			}

			if(!isPaused)
			{
				transform.Rotate(Vector3.forward, Direction * _currentSpeed * Time.fixedDeltaTime, Space.Self);
			}			
		}

		// --- Public/Internal Methods ------------------------------------------------------------------------------------

		// --- Protected/Private Methods ----------------------------------------------------------------------------------
		private void Switch()
		{
			isPaused = false;
			_switchCooldown.ChangeDuration(_switchDuration.GetRandom());
			_currentSpeed = _rotationSpeed.GetRandom();
			_clockwise = !_clockwise;
		}

		private void Pause()
		{
			isPaused = true;
			_switchCooldown.ChangeDuration(_switchPauseDuration);
		}

		// --------------------------------------------------------------------------------------------
	}

	// **************************************************************************************************************************************************
}