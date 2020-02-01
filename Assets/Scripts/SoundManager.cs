using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Audio;

namespace GGJ20
{
    public enum SFX
    {
        TechnoLizard_Rotation = 0,
        TechnoLizard_Movement = 1,
        Grappling_Shot = 2,
        Grappling_Return = 3,

        Grab_Block_Hard = 10,
        Grab_Block_Normal = 11,
        Grab_Block_Light = 12,
        Grab_Boulder = 13,
        Grab_LaveChunk = 14,

        Place_Block_Hard = 20,
        Place_Block_Normal = 22,
        Place_Block_Light = 23,
        Boulder_Crumble = 24,
        LavaChunk_Explosion = 25,
        Block_Destruction = 26,

        Lava_Splash = 30,
        Lava_Plop = 31,
        Lava_Sizzle = 32,

        Ahh_1 = 40,
        Ahh_2 = 41,
        Ahh_3 = 42,
        Ahh_4 = 43,
        Ahh_5 = 44,
        Ahh_6 = 45,
        Ahh_7 = 46,
        Ahh_8 = 47,

        Cheer_1 = 50,
        Cheer_2 = 51,
        Cheer_3 = 52,
        Cheer_4 = 53,
        Cheer_5 = 54,
        Cheer_6 = 55,
        Cheer_7 = 56,
        Cheer_8 = 57,

        Intro_StartButton = 60,
        Intro_GroundCracking = 61,
        Intro_TitleVoiceover = 62,
    }

    public class SoundManager : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------


        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------
        [SerializeField] AudioMixerGroup _mixerGroup;
        [SerializeField, Range(0, 16)] private int _startSources = 5;
        [SerializeField] private FloatRange _stereoRange = new FloatRange(-5, 5);

        private List<AudioSource> _sources = new List<AudioSource>();

        // --- Properties -------------------------------------------------------------------------------------------------
        public static SoundManager Instance { get; private set; }

        private string PRE => Logger.GetPre(this);

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            for(int i = 0; i < _startSources; i++)
            {
                AddSource();
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if(Input.GetKey(KeyCode.F8))
            {
                if(Input.GetKeyDown(KeyCode.A))
                {
                    PlayRandomAhh(Vector3.right * _stereoRange.GetRandom());
                }
                else if(Input.GetKeyDown(KeyCode.C))
                {
                    PlayRandomCheer(Vector3.right * _stereoRange.GetRandom());
                }
            }
        }
#endif

        // --- Public/Internal Methods ------------------------------------------------------------------------------------
        public static void Play(SFX sfx, Vector3 position, float volume = 1f, float delay = 0f)
        {
            Instance._Play(sfx, position, volume, delay);
        }

        public static void PlayRandomAhh(Vector3 position, float volume = 1f, float delay = 0f)
        {
            Instance._PlayRandom(40, 47, position, volume, delay);
        }

        public static void PlayRandomCheer(Vector3 position, float volume = 1f, float delay = 0f)
        {
            Instance._PlayRandom(50, 57, position, volume, delay);
        }

        // --- Protected/Private Methods ----------------------------------------------------------------------------------
        private void _Play(SFX sfx, Vector3 position, float volume, float delay)
        {
            SfxData data = GetData(sfx);
            if(data == null || data.Clip == null)
            {
                Debug.LogWarning($"{PRE} Failed to get AudioClip for SFX '{sfx}'");
                return;
            }

            AudioSource source = GetFreeSource();
            source.clip = data.Clip;
            source.volume = Mathf.Clamp01(data.Volume * volume);
            source.pitch = data.RandomPitch;
            source.panStereo = 2f * _stereoRange.InverseLerp(position.x) - 1f;

            if(delay > 0f)
            {
                source.PlayDelayed(delay);
            }
            else
            {
                source.Play();
            }
        }

        private void _PlayRandom(int minIndex, int maxIndex, Vector3 position, float volume, float delay)
        {
            int index = UnityEngine.Random.Range(minIndex, maxIndex + 1);
            _Play((SFX)index, position, volume, delay);
        }

        // --------------------------------------------------------------------------------------------
        private SfxData GetData(SFX sfx)
        {
            return Resources.Load<SfxData>($"SFX/{sfx}");
        }

        // --------------------------------------------------------------------------------------------
        private AudioSource GetFreeSource()
        {
            return _sources.FirstOrDefault(a => a.isPlaying == false) ?? AddSource();
        }

        private AudioSource AddSource()
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = _mixerGroup;
            _sources.Add(source);
            return source;
        }

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}