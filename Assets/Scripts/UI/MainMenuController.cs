using UnityEngine;
using TMPro;
using StellarDefense.Managers;

namespace StellarDefense.UI
{
    /// <summary>
    /// Controlador de la pantalla del menú principal.
    /// Muestra el high score persistido y gestiona la navegación
    /// a otras pantallas mediante GameManager.
    /// </summary>
    public sealed class MainMenuController : MonoBehaviour
    {
        [Header("Textos")]
        [Tooltip("Texto del High Score en el menú.")]
        [SerializeField] private TextMeshProUGUI highScoreText;

        [Tooltip("Prefijo del high score.")]
        [SerializeField] private string highScorePrefix = "Mejor puntuación: ";

        private void Start()
        {
            // Mostramos el high score guardado en PlayerPrefs.
            // ScoreManager puede no existir si venimos de un cold start en esta escena.
            if (highScoreText != null)
            {
                int hs = ScoreManager.Instance != null
                    ? ScoreManager.Instance.HighScore
                    : PlayerPrefs.GetInt("StellarDefense.HighScore", 0);

                highScoreText.text = $"{highScorePrefix}{hs}";
            }

            // Aseguramos que timeScale esté en 1 por si venimos de una pausa.
            Time.timeScale = 1f;
        }

        // ── Botones ────────────────────────────────────────────────────

        /// <summary>Llamado por el botón "Jugar".</summary>
        public void OnPlayButton()
        {
            if (AudioManager.Instance != null) AudioManager.Instance.OnUIClick();
            if (GameManager.Instance != null) GameManager.Instance.StartGame();
        }

        /// <summary>Llamado por el botón "Salir".</summary>
        public void OnQuitButton()
        {
            if (AudioManager.Instance != null) AudioManager.Instance.OnUIClick();

            // Application.Quit no funciona en el editor, solo en builds.
            // En editor lo simulamos con EditorApplication.isPlaying = false.
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}