using UnityEngine;
using UnityEngine.InputSystem;
using StellarDefense.Managers;
using StellarDefense.InputSystem;

namespace StellarDefense.UI
{
    /// <summary>
    /// Controlador del menú de pausa. Escucha la tecla Pause (Esc) e
    /// intercambia entre estados Playing y Paused mediante GameManager.
    /// El panel se activa/desactiva automáticamente según el estado.
    /// </summary>
    public sealed class PauseMenuController : MonoBehaviour
    {
        [Header("Panel")]
        [Tooltip("Panel raíz del menú de pausa. Se activa al pausar.")]
        [SerializeField] private GameObject pausePanel;

        [Header("Settings (referencia opcional)")]
        [Tooltip("Panel de ajustes que se abre con el botón Ajustes. Opcional.")]
        [SerializeField] private GameObject settingsPanel;

        private PlayerControls controls;

        private void Awake()
        {
            // Empezamos ocultos.
            if (pausePanel != null) pausePanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);

            controls = new PlayerControls();
        }

        private void OnEnable()
        {
            if (controls == null) controls = new PlayerControls();
            controls.Gameplay.Enable();
            controls.Gameplay.Pause.performed += OnPausePressed;

            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            if (controls != null)
            {
                controls.Gameplay.Pause.performed -= OnPausePressed;
                controls.Gameplay.Disable();
            }

            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }

        // ── Input ──────────────────────────────────────────────────────

        private void OnPausePressed(InputAction.CallbackContext ctx)
        {
            if (GameManager.Instance == null) return;

            // Solo permitimos togglear pausa si estamos jugando o pausados.
            // No queremos pausar durante GameOver o MainMenu.
            switch (GameManager.Instance.CurrentState)
            {
                case GameManager.GameState.Playing:
                    GameManager.Instance.PauseGame();
                    break;
                case GameManager.GameState.Paused:
                    GameManager.Instance.ResumeGame();
                    break;
            }
        }

        // ── Estado del juego ───────────────────────────────────────────

        private void HandleStateChanged(GameManager.GameState newState)
        {
            // Mostrar/ocultar el panel según el estado del juego.
            if (pausePanel != null)
                pausePanel.SetActive(newState == GameManager.GameState.Paused);

            // Si volvemos al juego, cerramos también el settings si estaba abierto.
            if (newState == GameManager.GameState.Playing && settingsPanel != null)
                settingsPanel.SetActive(false);
        }

        // ── Botones del Pause Menu ─────────────────────────────────────

        public void OnResumeButton()
        {
            if (AudioManager.Instance != null) AudioManager.Instance.OnUIClick();
            if (GameManager.Instance != null) GameManager.Instance.ResumeGame();
        }

        public void OnSettingsButton()
        {
            if (AudioManager.Instance != null) AudioManager.Instance.OnUIClick();
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        public void OnMainMenuButton()
        {
            if (AudioManager.Instance != null) AudioManager.Instance.OnUIClick();
            if (GameManager.Instance != null) GameManager.Instance.ReturnToMenu();
        }
    }
}