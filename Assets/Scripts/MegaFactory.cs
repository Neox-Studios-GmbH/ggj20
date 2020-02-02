using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class MegaFactory : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------
        public enum FactoryType
        {
            Lemming = 0,
            SmallBlock = 1,
            MediumBlock = 2,
            LargeBlock = 3,
            LavaChunk = 4,
            Boulder = 5
        }

        // --- Nested Classes ---------------------------------------------------------------------------------------------
        [Serializable]
        public struct FactoryPreset
        {
            public FactoryItem Preset;
            public int Amount;
        }

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] private List<FactoryPreset> _factoryPresets;

        [SerializeField] private Dictionary<FactoryType, List<FactoryItem>> _factoryItems = new Dictionary<FactoryType, List<FactoryItem>>();

        // --- Properties -------------------------------------------------------------------------------------------------
        public static MegaFactory Instance { get; private set; }

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this);
            }
            Instance = this;

            //DontDestroyOnLoad(Instance);
            InitFactoyItems();
        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public FactoryItem GetFactoryItem(FactoryType type)
        {
            if(!_factoryItems.ContainsKey(type))
            {
                Debug.LogWarning($"{Logger.GetPre(this)} Warning: FactoryType {type.ToString()} is not known to the factory. Returning null.");
                return null;
            }

            FactoryItem item = _factoryItems[type].FirstOrDefault(i => !i.Active);
            if(item == null)
            {
                item = AddItem(_factoryPresets.FirstOrDefault(p => p.Preset.Type == type));
            }

            item.gameObject.SetActive(true);
            return item;
        }

        public T GetFactoryItem<T>(FactoryType type) where T : FactoryItem
        {
            return GetFactoryItem(type) as T;
        }

        public void ReturnFactoryItem(FactoryItem item)
        {
            item.gameObject.SetActive(false);
            item.gameObject.transform.position = Vector3.zero;
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        private void InitFactoyItems()
        {
            foreach(FactoryPreset preset in _factoryPresets)
            {
                for(int i = 0; i < preset.Amount; i++)
                {
                    AddItem(preset);
                }
            }
        }

        private FactoryItem AddItem(FactoryPreset preset)
        {
            if(!_factoryItems.ContainsKey(preset.Preset.Type))
            {
                _factoryItems.Add(preset.Preset.Type, new List<FactoryItem>());
            }

            FactoryItem fi = Instantiate(preset.Preset);
            _factoryItems[preset.Preset.Type].Add(fi);
            fi.gameObject.SetActive(false);
            fi.gameObject.transform.position = Vector3.zero;
            fi.transform.SetParent(transform);
            return fi;
        }
        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}