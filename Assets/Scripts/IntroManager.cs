using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ20
{
    public class IntroManager : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------

        enum state { Idle, PrepareScene, GirlWalk, Edge, FadeToGame, Announcer, MoveMessage, AnnouncerEnd }
        state State;

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------

        [SerializeField] GameObject _gameAsset;

        [Header("Scene")]
        [SerializeField] UnityEngine.UI.RawImage CityImage;
        [SerializeField] UnityEngine.UI.RawImage HillsImage;
        [SerializeField] UnityEngine.UI.RawImage GrassLeft, GrassRight;
        [SerializeField] UnityEngine.UI.RawImage CaveImage;
        [SerializeField] [Range(0f, 10f)] float SceneScrollingSpeed;
        public AudioClip BackgroundMusic;
        public AudioClip ButtonPressSound;

        [Header("Splash")]
        [SerializeField] UnityEngine.UI.Text _startText;
        [SerializeField] [Range(0.01f, 10f)] float SplashDelay = 0.75f;
        float SplashElapsedTime;

        [Header("Girl")]
        [SerializeField] Transform Girl;
        [SerializeField] [Range(0f, 360f)] float GirlRot;
        [SerializeField] [Range(0f, 10f)] float GirlStepDelay;
        Vector2 GirlGroundPosition;
        float GirlStepOffset = 1f;
        float GirlAirY;
        float GirlTimer;
        bool RotDirectionSwitch;

        [Range(1f, 50f)] public float PanSpeed;
        public float GravityStart;
        float DistanceWalked;
        float Gravity;
        float FallTimer;
        public float FallTime;

        [Header("Transition")]
        public CanvasGroup FullscreenFade;
        [Range(0f, 5f)] public float ScreenFadeDuration;

        public AudioClip FloorCrackSound;
        public AudioClip GirlScreamSound;
        public float CaveScrollSpeed;

        [Header("Announcer")]
        public AudioClip MessageSounds;
        public float MessageDelay;
        public float MessageSpeed;
        float MessageTimer;
        int MessageIndex;
        public AudioSource Audio;

        [System.Serializable]
        public class audio_message
        {
            public float TargetX;
            public Image Image;
            public float Delay;
        }

        public audio_message[] Messages;


        // --- Properties -------------------------------------------------------------------------------------------------

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            GirlGroundPosition = Girl.position;
            State = state.Idle;

            Audio.clip = BackgroundMusic;
            Audio.Play();

            MessageDelay = Messages[0].Delay;

            Gravity = GravityStart;

        }
        private void Update()
        {
            switch (State)
            {
                case state.Idle:
                    {
                        // Splash Display
                        SplashElapsedTime += Time.deltaTime;
                        if (SplashElapsedTime >= SplashDelay)
                        {
                            _startText.gameObject.SetActive(!_startText.gameObject.activeSelf);
                            SplashElapsedTime = 0;
                        }

                        // Animation    
                        AnimateGirl();

                        int Before = (int)GrassLeft.uvRect.x;
                        ScrollX(HillsImage, SceneScrollingSpeed);
                        ScrollX(CityImage, SceneScrollingSpeed * 1.5f);
                        ScrollX(GrassLeft, SceneScrollingSpeed * 2f);
                        int After = (int)GrassLeft.uvRect.x;

                        // Process input
                        bool ButtonPressed = Input.GetKeyDown("space");

                        if (ButtonPressed)
                        {
                            State = state.PrepareScene;
                            _startText.gameObject.SetActive(false);
                            Audio.PlayOneShot(ButtonPressSound);
                        }

                    }
                    break;

                case state.PrepareScene:
                    {
                        AnimateGirl();

                        int Before = (int)GrassLeft.uvRect.x;
                        ScrollX(HillsImage, SceneScrollingSpeed);
                        ScrollX(CityImage, SceneScrollingSpeed * 1.5f);
                        ScrollX(GrassLeft, SceneScrollingSpeed * 2f);
                        int After = (int)GrassLeft.uvRect.x;

                        if (After > Before)
                        {
                            Rect UVRect;
                            UVRect = GrassLeft.uvRect;
                            UVRect.x = 0f;
                            GrassLeft.uvRect = UVRect;

                            State = state.GirlWalk;
                        }
                    }
                    break;

                case state.GirlWalk:
                    {
                        AnimateGirl();
                        ScrollX(HillsImage, SceneScrollingSpeed);
                        ScrollX(CityImage, SceneScrollingSpeed * 1.5f);

                        float Delta = -PanSpeed * Time.deltaTime;
                        TranslateX(GrassLeft.transform, Delta);
                        TranslateX(GrassRight.transform, Delta);

                        DistanceWalked += -Delta;
                        if (DistanceWalked > 15)
                        {
                            Girl.position = GirlGroundPosition;
                            Girl.eulerAngles = Vector3.zero;
                            State = state.Edge;
                            Audio.Stop();
                            Audio.PlayOneShot(GirlScreamSound);
                            Audio.PlayOneShot(FloorCrackSound);
                        }

                    }
                    break;

                case state.Edge:
                    {
                        float Delta = PanSpeed * 0.75f * Time.deltaTime;
                        TranslateX(GrassLeft.transform, -Delta);
                        TranslateX(GrassRight.transform, Delta);

                        Gravity += Time.deltaTime / 4f;
                        float RotSpeed = 18f * Time.deltaTime;

                        TranslateY(Girl, -Gravity);
                        RotateZ(Girl, RotSpeed);

                        TranslateY(GrassLeft.transform, -Gravity);
                        RotateZ(GrassLeft.transform, RotSpeed);

                        TranslateY(GrassRight.transform, -Gravity);
                        RotateZ(GrassRight.transform, -RotSpeed);

                        FallTimer += Time.deltaTime;
                        if (FallTimer > FallTime)
                        {
                            State = state.FadeToGame;
                            StartCoroutine(FadeToAnnouncement());
                        }
                    }
                    break;

                case state.FadeToGame:
                    {
                        // Wait On Coroutine to finsh
                    }
                    break;





                case state.Announcer:
                    {
                        if (MessageTimer > MessageDelay)
                        {
                            MessageTimer = 0;


                            State = state.MoveMessage;
                            return;

                        }

                        MessageTimer += Time.deltaTime;

                    }
                    break;

                case state.MoveMessage:
                    {
                        audio_message Message = Messages[MessageIndex];
                        Transform T = Message.Image.transform;
                        Vector2 Pos = T.localPosition;

                        Pos.x = Mathf.MoveTowards(Pos.x, Message.TargetX, MessageSpeed * Time.deltaTime);
                        T.localPosition = Pos;

                        if (Pos.x == Message.TargetX)
                        {
                            ++MessageIndex;

                            if (MessageIndex == Messages.Length)
                            {
                                State = state.AnnouncerEnd;
                                MessageTimer = 0;

                            }
                            else
                            {
                                State = state.Announcer;
                                MessageDelay = Messages[MessageIndex].Delay;
                            }
                        }
                    }
                    break;



                case state.AnnouncerEnd:
                    {
                        if (Audio.isPlaying)
                        {
                            //
                        }
                        else
                        {
                            if (MessageTimer > 1f)
                            {
                                StartCoroutine(FadeToGame());
                            }

                            MessageTimer += Time.deltaTime;
                        }
                    }
                    break;
            }

        }

        // --- Public/Internal Methods ------------------------------------------------------------------------------------

        // --- Protected/Private Methods ----------------------------------------------------------------------------------


        IEnumerator LerpAlpha(float TargetAlpha, float Duration)
        {
            float SourceAlpha = FullscreenFade.alpha;
            float Timer = 0;
            while (Timer < Duration)
            {
                FullscreenFade.alpha = Mathf.Lerp(SourceAlpha, TargetAlpha, Timer / Duration);
                Timer += Time.deltaTime;
                yield return null;
            }

            FullscreenFade.alpha = TargetAlpha;
        }

        IEnumerator FadeToAnnouncement()
        {
            yield return LerpAlpha(1f, ScreenFadeDuration);

            CityImage.gameObject.SetActive(false);
            HillsImage.gameObject.SetActive(false);
            GrassLeft.gameObject.SetActive(false);
            GrassRight.gameObject.SetActive(false);

            CaveImage.transform.localPosition = Vector3.zero;
            StartCoroutine(ScrollCave());

            yield return LerpAlpha(0f, ScreenFadeDuration);

            State = state.Announcer;
            Audio.clip = MessageSounds;
            Audio.Play();


        }


        IEnumerator FadeToGame()
        {
            yield return LerpAlpha(1f, ScreenFadeDuration);

            Destroy(this.gameObject);
            Instantiate(_gameAsset);
        }

        IEnumerator ScrollCave()
        {
            while (true)
            {
                ScrollY(CaveImage, -CaveScrollSpeed);
                yield return null;
            }
        }



        void TranslateX(Transform T, float Value)
        {
            Vector2 P = T.position;
            P.x += Value;
            T.position = P;
        }

        void TranslateY(Transform T, float Value)
        {
            Vector2 P = T.position;
            P.y += Value;
            T.position = P;
        }

        void RotateZ(Transform T, float Value)
        {
            T.eulerAngles += new Vector3(0, 0, Value);
        }

        void AnimateGirl()
        {
            GirlAirY = GirlGroundPosition.y + GirlStepOffset;
            GirlTimer += Time.deltaTime;

            if (GirlTimer > GirlStepDelay)
            {
                GirlTimer -= GirlStepDelay;

                float NewY = 0;
                float NewRot = 0;
                float CurrentY = Girl.position.y;

                if (CurrentY == GirlGroundPosition.y)
                {
                    RotDirectionSwitch = !RotDirectionSwitch;

                    NewY = GirlAirY;
                    NewRot = RotDirectionSwitch ? GirlRot : -GirlRot;
                }
                else if (CurrentY == GirlAirY)
                {
                    NewY = GirlGroundPosition.y;
                }

                Girl.position = new Vector2(Girl.position.x, NewY);
                Girl.eulerAngles = new Vector3(0, 0, NewRot);
            }
        }

        void ScrollX(UnityEngine.UI.RawImage Image, float Speed)
        {
            Rect UVRect;
            UVRect = Image.uvRect;
            UVRect.x += Speed * Time.deltaTime;
            Image.uvRect = UVRect;
        }

        void ScrollY(UnityEngine.UI.RawImage Image, float Speed)
        {
            Rect UVRect;
            UVRect = Image.uvRect;
            UVRect.y += Speed * Time.deltaTime;
            Image.uvRect = UVRect;
        }

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}