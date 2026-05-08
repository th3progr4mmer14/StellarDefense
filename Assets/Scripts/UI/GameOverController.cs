using UnityEngine;
using TMPro;
using StellarDefense.Managers;

namespace StellarDefense.UI
{
    /// <summary>
    /// Controlador de la pantalla de Game Over.
    /// Muestra el score final, indica si se batió el récord,
    /// y permite reintentar o volver al menú.
    /// Es un overlay que se activa encima de la escena Gameplay.
    /// </summary>
    public sealed class GameOverController : MonoBehaviour
    {
        [Header("Textos")]
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI newRecordText;

        [Header("Prefijos")]
        [SerializeField] private string finalScorePrefix = "Puntuación: ";
        [SerializeField] private string highScorePrefix = "Récord: ";

        [Header("Panel raíz")]
        [Tooltip("El panel que contiene toda la UI de Game Over. " +
                 "Se activa/desactiva según el estado del juego.")]
        [SerializeField] private GameObject gameOverPanel;

        private void Awake()
        {
            // Empezamos ocultos. Se activa cuando el GameManager cambia a GameOver.
            if (gameOverPanel != null) gameOverPanel.SetActive(false);

            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged += HandleStateChanged;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }

        // ── Reacción al estado del juego ───────────────────────────────

        private void HandleStateChanged(GameManager.GameState newState)
        {
            if (newState == GameManager.GameState.GameOver)
            {
                ShowGameOver();
            }
        }

        private void ShowGameOver()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);

            int finalScore = ScoreManager.Instance != null
                ? ScoreManager.Instance.CurrentScore : 0;
            int highScore = ScoreManager.Instance != null
                ? ScoreManager.Instance.HighScore : 0;
            bool isNewRecord = finalScore >= highScore && finalScore > 0;

            if (finalScoreText != null)
                finalScoreText.text = $"{finalScorePrefix}{finalScore}";

            if (highScoreText != null)
                highScoreText.text = $"{highScorePrefix}{highScore}";

            // Mostramos el texto de nuevo récord solo si aplica.
            if (newRecordText != null)
                newRecordText.gameObject.SetActive(isNewRecord);

            // SFX de game over.
            if (AudioManager.Instance != null) AudioManager.Instance.OnGameOver();
        }

        // ── Botones ────────────────────────────────────────────────────

        /// <summary>Llamado por el botón "Reintentar".</summary>
        public void OnRetryButton()
        {
            if (AudioManager.Instance != null) AudioManager.Instance.OnUIClick();
            if (GameManager.Instance != null) GameManager.Instance.RestartGame();
        }

        /// <summary>Llamado por el botón "Menú principal".</summary>
        public void OnMenuButton()
        {
            if (AudioManager.Instance != null) AudioManager.Instance.OnUIClick();
            if (GameManager.Instance != null) GameManager.Instance.ReturnToMenu();
        }
    }
}