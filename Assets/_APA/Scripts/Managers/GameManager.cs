// Updated GameManager.cs with corrected parameter names to match revised VideoPlaybackController

using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using APA.Core;

namespace _APA.Scripts.Managers
{
    public class GameManager : APAMonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Scene Names")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private string gameWorldSceneName = "GameWorld";

        [Header("Video Clips")]
        [SerializeField] private VideoClip mainMenuBackgroundVideo;
        [SerializeField] private VideoClip introVideo;
        [SerializeField] private VideoClip middleVideo;
        [SerializeField] private VideoClip gameEndingVideo;
        [SerializeField] private float gameEndingVideoLoopStartTime = 5f;
        [SerializeField] private VideoClip finalCreditsVideo;

        [Header("UI")]
        [SerializeField] public RenderTexture mainMenuRenderTexture;
        [SerializeField] private Image blackScreenOverlay;

        [Header("Video Prefab")]
        [SerializeField] private GameObject videoPlayerPrefab;

        private GameObject currentEventVideoInstance;
        private GameObject currentMenuBackgroundVideoInstance;
        private Coroutine endingVideoInputCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void TriggerIntroVideo()
        {
            EnableBlackScreen();
            PlayEventVideo(introVideo, LoadGameWorldScene);
        }

        public void PlayMainMenuBackgroundVideo()
        {
            if (!IsValid(mainMenuBackgroundVideo, mainMenuRenderTexture, videoPlayerPrefab)) return;

            StopCurrentVideos();

            currentMenuBackgroundVideoInstance = Instantiate(videoPlayerPrefab);
            var controller = currentMenuBackgroundVideoInstance.GetComponent<VideoPlaybackController>();
            var audio = currentMenuBackgroundVideoInstance.GetComponent<AudioSource>();

            controller.Play(
                mainMenuBackgroundVideo,
                onComplete: null,
                loop: true,
                renderTexture: mainMenuRenderTexture,
                rawImage: null,
                audioOutput: GetAudioMode(mainMenuBackgroundVideo, audio),
                customAudioSource: audio
            );
        }

        public void StopMainMenuBackgroundVideo()
        {
            StopAndDestroy(ref currentMenuBackgroundVideoInstance);
        }

        public void PlayMiddleVideo(Action onComplete = null)
        {
            Time.timeScale = 0f;
            PlayEventVideo(middleVideo, () =>
            {
                Time.timeScale = 1f;
                onComplete?.Invoke();
            });
        }

        public void PlayEndingVideo()
        {
            EnableBlackScreen();
            Time.timeScale = 0f;

            StopCurrentVideos();
            currentEventVideoInstance = Instantiate(videoPlayerPrefab);
            var controller = currentEventVideoInstance.GetComponent<VideoPlaybackController>();
            var audio = currentEventVideoInstance.GetComponent<AudioSource>();

            double loopStart = Math.Max(0, gameEndingVideo.length - gameEndingVideoLoopStartTime);

            controller.PlayWithLoopAtEnd(
                gameEndingVideo,
                loopStart,
                onReachedLoop: () =>
                {
                    endingVideoInputCoroutine = StartCoroutine(WaitForEnterKey(controller));
                },
                onCompleteIfNoLoop: ProceedToCreditsOrMainMenu,
                audioOutput: GetAudioMode(gameEndingVideo, audio),
                customAudioSource: audio
            );
        }

        private void PlayEventVideo(VideoClip clip, Action onComplete)
        {
            if (!IsValid(clip, null, videoPlayerPrefab)) { onComplete?.Invoke(); return; }

            StopCurrentVideos();
            currentEventVideoInstance = Instantiate(videoPlayerPrefab);
            var controller = currentEventVideoInstance.GetComponent<VideoPlaybackController>();
            var audio = currentEventVideoInstance.GetComponent<AudioSource>();

            controller.Play(
                clip,
                onComplete: () =>
                {
                    StopAndDestroy(ref currentEventVideoInstance);
                    onComplete?.Invoke();
                },
                loop: false,
                renderTexture: null,
                rawImage: null,
                audioOutput: GetAudioMode(clip, audio),
                customAudioSource: audio
            );
        }

        private IEnumerator WaitForEnterKey(VideoPlaybackController controller)
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    controller.ForceStop();
                    ProceedToCreditsOrMainMenu();
                    yield break;
                }
                yield return null;
            }
        }

        private void ProceedToCreditsOrMainMenu()
        {
            if (endingVideoInputCoroutine != null)
            {
                StopCoroutine(endingVideoInputCoroutine);
                endingVideoInputCoroutine = null;
            }

            StopCurrentVideos();

            if (finalCreditsVideo != null)
            {
                PlayEventVideo(finalCreditsVideo, () =>
                {
                    Time.timeScale = 1f;
                    LoadMainMenuScene();
                });
            }
            else
            {
                Time.timeScale = 1f;
                LoadMainMenuScene();
            }
        }

        private void StopCurrentVideos()
        {
            StopAndDestroy(ref currentEventVideoInstance);
            StopAndDestroy(ref currentMenuBackgroundVideoInstance);
        }

        private void StopAndDestroy(ref GameObject instance)
        {
            if (instance == null) return;
            var controller = instance.GetComponent<VideoPlaybackController>();
            controller?.ForceStop();
            Destroy(instance);
            instance = null;
        }

        private void EnableBlackScreen()
        {
            if (blackScreenOverlay)
            {
                blackScreenOverlay.gameObject.SetActive(true);
                blackScreenOverlay.CrossFadeAlpha(1f, 0f, true);
            }
        }

        private void LoadGameWorldScene() => SceneManager.LoadScene(gameWorldSceneName);
        private void LoadMainMenuScene() => SceneManager.LoadScene(mainMenuSceneName);

        private bool IsValid(VideoClip clip, RenderTexture texture, GameObject prefab)
        {
            if (clip == null)
            {
                Debug.LogWarning("Missing VideoClip.");
                return false;
            }
            if (prefab == null)
            {
                Debug.LogError("Missing VideoPlayerPrefab.");
                return false;
            }
            if (texture == null && clip == mainMenuBackgroundVideo)
            {
                Debug.LogWarning("Main menu render texture is missing.");
                return false;
            }
            return true;
        }

        private VideoAudioOutputMode GetAudioMode(VideoClip clip, AudioSource source)
        {
            if (clip.audioTrackCount == 0) return VideoAudioOutputMode.None;
            return source ? VideoAudioOutputMode.AudioSource : VideoAudioOutputMode.Direct;
        }
    }
}
