using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ20
{
    public class IntroManager : MonoBehaviour
    {
        // --- Enums ------------------------------------------------------------------------------------------------------

        enum state { Idle, PrepareScene, GirlWalk, Edge, FadeToGame }
        state State;

        // --- Nested Classes ---------------------------------------------------------------------------------------------

        // --- Fields -----------------------------------------------------------------------------------------------------

        [SerializeField] GameObject _gameAsset;

        [Header("Scene")]
        [SerializeField] UnityEngine.UI.RawImage _backgroundRawImage;
        [SerializeField] UnityEngine.UI.RawImage GrassLeft, GrassRight;
        [SerializeField] [Range(0f, 10f)] float SceneScrollingSpeed;

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
        float DistanceWalked;
        float Gravity;
        float FallTimer;
        public float FallTime;

        [Header("Transition")]
        public CanvasGroup FullscreenFade;
        [Range(0f, 5f)] public float ScreenFadeDuration;

        // --- Properties -------------------------------------------------------------------------------------------------

        // --- Unity Functions --------------------------------------------------------------------------------------------
        private void Awake()
        {
            GirlGroundPosition = Girl.position;
            State = state.Idle;
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
                        Scroll(_backgroundRawImage, SceneScrollingSpeed);
                        Scroll(GrassLeft, SceneScrollingSpeed * 1.5f);
                        int After = (int)GrassLeft.uvRect.x;

                        // Process input
                        bool ButtonPressed = Input.GetKeyDown("space");

                        if (ButtonPressed)
                        {
                            State = state.PrepareScene;
                            _startText.gameObject.SetActive(false);

                        }

                    }
                    break;

                case state.PrepareScene:
                    {
                        AnimateGirl();

                        int Before = (int)GrassLeft.uvRect.x;
                        Scroll(_backgroundRawImage, SceneScrollingSpeed);
                        Scroll(GrassLeft, SceneScrollingSpeed * 1.5f);
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
                        Scroll(_backgroundRawImage, SceneScrollingSpeed);

                        float Delta = -PanSpeed * Time.deltaTime;
                        TranslateX(GrassLeft.transform, Delta);
                        TranslateX(GrassRight.transform, Delta);

                        DistanceWalked += -Delta;
                        if (DistanceWalked > 15)
                        {
                            Girl.position = GirlGroundPosition;
                            Girl.eulerAngles = Vector3.zero;
                            State = state.Edge;
                        }

                    }
                    break;

                case state.Edge:
                    {
                        float Delta = PanSpeed * 0.75f * Time.deltaTime;
                        TranslateX(GrassLeft.transform, -Delta);
                        TranslateX(GrassRight.transform, Delta);

                        Gravity += Time.deltaTime / 6f;
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
                            StartCoroutine(FadeToGame());
                        }
                    }
                    break;

                case state.FadeToGame:
                    {

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

        IEnumerator FadeToGame()
        {
            yield return LerpAlpha(1f, ScreenFadeDuration);

            Destroy(this.gameObject);
            Instantiate(_gameAsset);
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

        void Scroll(UnityEngine.UI.RawImage Image, float Speed)
        {
            Rect UVRect;
            UVRect = Image.uvRect;
            UVRect.x += Speed * Time.deltaTime;
            Image.uvRect = UVRect;
        }

        // --------------------------------------------------------------------------------------------
    }

    // **************************************************************************************************************************************************
}