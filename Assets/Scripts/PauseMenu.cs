using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts {
    public class PauseMenu : MonoBehaviour {
        public GameObject pauseMenuUI;

        public static bool isPaused = false;

        void Start() {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;

        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
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
        }
        public void Pause() {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f; // Freeze all physics and Update() time
            isPaused = true;
        }

        public void RestartGame() {
            Time.timeScale = 1f;
            isPaused = false;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}