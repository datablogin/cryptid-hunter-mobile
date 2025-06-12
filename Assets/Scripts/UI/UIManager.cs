using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CryptidHunter.Core;

namespace CryptidHunter.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI cryptidsEncounteredText;
        [SerializeField] private TextMeshProUGUI cryptidsCapturedText;
        [SerializeField] private TextMeshProUGUI cryptidInfoText;
        
        [Header("Mobile Controls")]
        [SerializeField] private GameObject mobileControlsPanel;
        [SerializeField] private Joystick movementJoystick;
        [SerializeField] private Joystick lookJoystick;
        [SerializeField] private Button jumpButton;
        [SerializeField] private Button runButton;
        
        [Header("Menus")]
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject settingsMenu;
        
        private GameManager gameManager;
        private bool isPaused = false;
        
        private void Start()
        {
            gameManager = GameManager.Instance;
            
#if UNITY_ANDROID || UNITY_IOS
            if (mobileControlsPanel != null)
                mobileControlsPanel.SetActive(true);
#else
            if (mobileControlsPanel != null)
                mobileControlsPanel.SetActive(false);
#endif
            
            UpdateUI();
        }
        
        private void Update()
        {
            UpdateUI();
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
        
        private void UpdateUI()
        {
            if (gameManager == null) return;
            
            if (scoreText != null)
                scoreText.text = $"Score: {gameManager.GetScore()}";
                
            if (cryptidsEncounteredText != null)
                cryptidsEncounteredText.text = $"Encountered: {gameManager.GetCryptidsEncountered()}";
                
            if (cryptidsCapturedText != null)
                cryptidsCapturedText.text = $"Captured: {gameManager.GetCryptidsCaptured()}";
        }
        
        public void ShowCryptidInfo(string cryptidName, int dangerLevel)
        {
            if (cryptidInfoText != null)
            {
                cryptidInfoText.text = $"Spotted: {cryptidName} (Danger: {dangerLevel}/5)";
                cryptidInfoText.gameObject.SetActive(true);
                
                CancelInvoke(nameof(HideCryptidInfo));
                Invoke(nameof(HideCryptidInfo), 5f);
            }
        }
        
        private void HideCryptidInfo()
        {
            if (cryptidInfoText != null)
                cryptidInfoText.gameObject.SetActive(false);
        }
        
        public void TogglePause()
        {
            isPaused = !isPaused;
            
            if (pauseMenu != null)
                pauseMenu.SetActive(isPaused);
                
            Time.timeScale = isPaused ? 0f : 1f;
            
#if !UNITY_ANDROID && !UNITY_IOS
            Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isPaused;
#endif
        }
        
        public void OpenSettings()
        {
            if (settingsMenu != null)
                settingsMenu.SetActive(true);
                
            if (pauseMenu != null)
                pauseMenu.SetActive(false);
        }
        
        public void CloseSettings()
        {
            if (settingsMenu != null)
                settingsMenu.SetActive(false);
                
            if (pauseMenu != null)
                pauseMenu.SetActive(true);
        }
        
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        
        public Vector2 GetMovementInput()
        {
            if (movementJoystick != null && movementJoystick.gameObject.activeSelf)
            {
                return new Vector2(movementJoystick.Horizontal, movementJoystick.Vertical);
            }
            return Vector2.zero;
        }
        
        public Vector2 GetLookInput()
        {
            if (lookJoystick != null && lookJoystick.gameObject.activeSelf)
            {
                return new Vector2(lookJoystick.Horizontal, lookJoystick.Vertical);
            }
            return Vector2.zero;
        }
    }
}