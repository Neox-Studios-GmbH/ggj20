using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class GrapplingHook : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        private int _hookRange = 5;
        private int _cableCount = 5;
        private int _cablePassed = 0;
        [SerializeField] private float _cableSpawnDistance = 1f;
        [SerializeField] private Transform _hookCable;
        [SerializeField] private float _hookSpeed = 4f;

        private float _lerpLimit = 0.0001f;
        private bool _hasGrabed;
        private bool _allCableHasPassed = false;
        Vector3 originPos;
        Vector3 endPosition;
        bool isHooking = false;

        private List<Transform> _cables = new List<Transform>();
        private List<Transform> _cablesOriginTrans = new List<Transform>();

        SpriteRenderer spriteCable;
        float spriteCableLenght;
        // --- Properties -------------------------------------------------------------------------------------------------

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            foreach (Transform t in _hookCable)
            {
                _cables.Add(t);
                _cablesOriginTrans.Add(t);
            }
            _hookRange = _cables.Count;
            _cableCount = _cables.Count;

            spriteCable = _cables[0].GetComponent<SpriteRenderer>();
            spriteCableLenght = spriteCable.bounds.extents.y;

        }
        void Update()
        {
            if (Input.GetKeyDown("space") && !isHooking)
            {
                originPos = this.gameObject.transform.position;
                endPosition = new Vector3(this.gameObject.transform.position.x + _hookRange, this.gameObject.transform.position.y, 0f);
                switchIsHook();
            }
            if (isHooking)
            {
                this.gameObject.transform.position = Vector2.MoveTowards(this.gameObject.transform.position, endPosition, _hookSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, endPosition) < _lerpLimit)
                {
                    endPosition = originPos;
                }
                cableVisible();
                if (Vector3.Distance(transform.position, originPos) < _lerpLimit)
                {
                    switchIsHook();
                }
            }
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public void switchIsHook()
        {
            isHooking = !isHooking;
        }
        private void hooking(Vector3 currentPos, Vector3 targetPos)
        {
            this.gameObject.transform.position = Vector2.MoveTowards(currentPos, targetPos, _hookSpeed * Time.deltaTime);
        }
        private void cableVisible()
        {

            if (Vector3.Distance(transform.position, originPos) > spriteCableLenght + _cablePassed && !_allCableHasPassed)
            {
                _cablePassed++;
                _cables[_cableCount - _cablePassed].gameObject.SetActive(true);

                if (_cableCount == _cablePassed + 1)
                {
                    _allCableHasPassed = true;
                }
                _cableSpawnDistance++;
            }
            if (Vector3.Distance(transform.position, originPos) > spriteCableLenght - _cablePassed && _allCableHasPassed)
            {

                _cables[_cableCount - _cablePassed].gameObject.SetActive(false);
                _cablePassed--;
                if (0 == _cablePassed)
                {
                    _allCableHasPassed = false;
                }
                _cableSpawnDistance--;

            }
        }
    }
    // --- Protected/Private Methods ----------------------------------------------------------------------------------

    // --------------------------------------------------------------------------------------------

    // **************************************************************************************************************************************************
}