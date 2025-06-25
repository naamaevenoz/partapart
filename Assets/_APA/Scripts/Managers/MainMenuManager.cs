namespace _APA.Scripts.Managers
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;

    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private string gameWorldSceneName = "GameWorld";
        [SerializeField] private RawImage backgroundVideoDisplay;

        private bool isLoading = false;

        void Start()
        {
            Time.timeScale = 1f;

            if (GameManager.Instance != null)
            {
                if (backgroundVideoDisplay != null && GameManager.Instance.mainMenuRenderTexture != null)
                {
                    backgroundVideoDisplay.texture = GameManager.Instance.mainMenuRenderTexture;
                    backgroundVideoDisplay.enabled = true;
                    GameManager.Instance.PlayMainMenuBackgroundVideo();
                }
                else
                {
                    Debug.LogWarning("Missing RawImage or RenderTexture. Can't play background video.");
                }
            }
        }

        void Update()
        {
            if (isLoading) return;

            if (Input.anyKeyDown &&
                !Input.GetKeyDown(KeyCode.Escape) &&
                !Input.GetMouseButtonDown(0))
            {
                if (UnityEngine.EventSystems.EventSystem.current?.currentSelectedGameObject == null)
                    StartGame();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                QuitGame();
        }

        public void StartGame()
        {
            if (isLoading || string.IsNullOrEmpty(gameWorldSceneName)) return;

            isLoading = true;
            backgroundVideoDisplay.enabled = false;
            GameManager.Instance?.StopMainMenuBackgroundVideo();
            GameManager.Instance?.TriggerIntroVideo();
        }

        public void QuitGame()
        {
            GameManager.Instance?.StopMainMenuBackgroundVideo();
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
