 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
	public class JigsawMovement : MonoBehaviour
	{
		// --- Enums ------------------------------------------------------------------------------------------------------
		public enum DirectionType
		{
			Positive = 0,
			Negative = 1,
			Random = 2
		}

		// --- Nested Classes ---------------------------------------------------------------------------------------------

		// --- Fields -----------------------------------------------------------------------------------------------------
		[SerializeField] private FloatRange _xRange;
		[SerializeField] private DirectionType _initialDirectionX;
		[SerializeField, Min(0f)] private float _xSpeed;
		[SerializeField, Min(0f)] private float _xAcceleration;
		[Space]
		[SerializeField] private FloatRange _yRange;
		[SerializeField] private DirectionType _initialDirectionY;
		[SerializeField, Min(0f)] private float _ySpeed;
		[SerializeField, Min(0f)] private float _yAcceleration;

		private int _xDirection, _yDirection;
		private float _xVelocity, _yVelocity;

		// --- Properties -------------------------------------------------------------------------------------------------
		private float XTarget => GetTarget(_xRange, _xDirection);
		private float YTarget => GetTarget(_yRange, _yDirection);

		// --- Unity Functions --------------------------------------------------------------------------------------------
		private void Awake()
		{
			_xDirection = GetDirection(_initialDirectionX);
			_yDirection = GetDirection(_initialDirectionY);
		}

		private void FixedUpdate()
		{
			float x = UpdatePosition(transform.localPosition.x, XTarget, _xAcceleration, _xSpeed, ref _xDirection, ref _xVelocity);
			float y = UpdatePosition(transform.localPosition.y, YTarget, _yAcceleration, _ySpeed, ref _yDirection, ref _yVelocity);

			transform.localPosition = new Vector2(x, y);
		}

		// --- Public/Internal Methods ------------------------------------------------------------------------------------

		// --- Protected/Private Methods ----------------------------------------------------------------------------------
		private int GetDirection(DirectionType type)
		{
			return type == DirectionType.Positive ? 1
				: type == DirectionType.Negative ? -1
				: Randomizer.PlusMinusOne();
		}

		private float GetTarget(FloatRange range, int direction)
		{
			return direction == 1 ? range.Max : range.Min;
		}

		// --------------------------------------------------------------------------------------------
		private float UpdatePosition(float current, float target, float acceleration, float maxSpeed,
			ref int direction, ref float speed)
		{
			speed = Mathf.MoveTowards(speed, direction * maxSpeed, direction * acceleration * Time.fixedDeltaTime);

			float n = Mathf.MoveTowards(current, target, speed * Time.fixedDeltaTime);
			if(n == target)
			{
				direction *= -1;
			}

			return n;
		}

		// --------------------------------------------------------------------------------------------
	}

	// **************************************************************************************************************************************************
}