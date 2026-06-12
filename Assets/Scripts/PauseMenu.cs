using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace Assets.Scripts {
    public class PauseMenu : MonoBehaviour {
        public GameObject pauseMenuUI;

        public GameObject firstSelectedButton;

        public static bool isPaused = false;

        void Start() {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;

        }

        void Update() {
            bool pauseButtonPressed = Input.GetKeyDown(KeyCode.Escape);

            if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame) {
                pauseButtonPressed = true;
            }

            if (pauseButtonPressed) {
                if (isPaused) {
                    Resume();
                }
                else {
                    Pause();
                }
            }
        }

        public void Resume() {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f; // Unfreeze time
            isPaused = false;

            EventSystem.current.SetSelectedGameObject(null);
        }
        public void Pause() {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f; // Freeze all physics and Update() time
            isPaused = true;

            EventSystem.current.SetSelectedGameObject(null);

            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }

        public void RestartGame() {
            Time.timeScale = 1f;
            isPaused = false;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}