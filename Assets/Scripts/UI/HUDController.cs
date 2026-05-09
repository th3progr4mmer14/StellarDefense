using System.Collections;
using UnityEngine;
using TMPro;
using StellarDefense.Managers;
using StellarDefense.Player;

namespace StellarDefense.UI
{
    /// <summary>
    /// Controlador del HUD durante el gameplay. Se suscribe a eventos de los
    /// managers y actualiza los elementos visuales sin modificar la lógica del juego.
    /// Nunca llama directamente a GameManager ni ScoreManager para cambiar estado,
    /// solo lee y muestra información.
    /// </summary>
    public sealed class HUDController : MonoBehaviour
    {
        [Header("Referencias de texto (TextMeshPro)")]
        [Tooltip("Texto que muestra el score actual.")]
        [SerializeField] private TextMeshProUGUI scoreText;

        [Tooltip("Texto que muestra el high score.")]
        [SerializeField] private TextMeshProUGUI highScoreText;

        [Tooltip("Texto que muestra las vidas restantes.")]
        [SerializeField] private TextMeshProUGUI livesText;

        [Tooltip("Texto que muestra la wave actual.")]
        [SerializeField] private TextMeshProUGUI waveText;

        [Tooltip("Texto de cuenta regresiva entre waves. Se oculta durante el juego.")]
        [SerializeField] private TextMeshProUGUI countdownText;

        [Header("Referencias de gameplay")]
        [Tooltip("Referencia al PlayerController para suscribirse a sus eventos.")]
        [SerializeField] private PlayerController playerController;

        [Header("Formato")]
        [Tooltip("Prefijo del score. P.ej: 'Score: ' o 'Puntos: '")]
        [SerializeField] private string scorePrefix = "Score: ";

        [Tooltip("Prefijo del high score.")]
        [SerializeField] private string highScorePrefix = "HS: ";

        [Tooltip("Prefijo de wave.")]
        [SerializeField] private string wavePrefix = "Wave ";

        // ── Estado interno ─────────────────────────────────────────────
        private Coroutine countdownCoroutine;

        private void Start()
        {
            SubscribeToEvents();
            InitializeDisplay();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        // ── Suscripciones ──────────────────────────────────────────────

        private void SubscribeToEvents()
        {
            // ScoreManager: actualizamos cuando cambia el score.
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged += UpdateScore;
            }

            // PlayerController: actualizamos cuando cambian las vidas.
            if (playerController != null)
            {
                playerController.OnLivesChanged += UpdateLives;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged -= UpdateScore;
            }

            if (playerController != null)
            {
                playerController.OnLivesChanged -= UpdateLives;
            }
        }

        // ── Inicialización ─────────────────────────────────────────────

        private void InitializeDisplay()
        {
            // Mostramos valores iniciales para que el HUD no aparezca vacío al arrancar.
            if (ScoreManager.Instance != null)
            {
                UpdateScore(ScoreManager.Instance.CurrentScore);
                UpdateHighScore(ScoreManager.Instance.HighScore);
            }

            if (playerController != null)
            {
                UpdateLives(playerController.CurrentLives);
            }

            // Ocultamos el countdown hasta que sea necesario.
            if (countdownText != null)
                countdownText.gameObject.SetActive(false);

            // Wave inicial.
            UpdateWave(1);
        }

        // ── Actualizadores de UI ───────────────────────────────────────

        private void UpdateScore(int newScore)
        {
            if (scoreText == null) return;
            scoreText.text = $"{scorePrefix}{newScore}";

            // Actualizamos el HighScore en tiempo real si se supera.
            UpdateHighScore(ScoreManager.Instance != null
                ? ScoreManager.Instance.HighScore
                : newScore);
        }

        private void UpdateHighScore(int highScore)
        {
            if (highScoreText == null) return;
            highScoreText.text = $"{highScorePrefix}{highScore}";
        }

        private void UpdateLives(int lives)
        {
            if (livesText == null) return;

            // Mostramos corazones ♥ en lugar de número. Un símbolo por vida.
            // Si quieres número, cambia a: livesText.text = $"{livesPrefix}{lives}";
            string hearts = "";
            for (int i = 0; i < lives; i++) hearts += "♥ ";
            livesText.text = hearts.TrimEnd();
        }

        /// <summary>Llamar desde WaveManager al iniciar cada wave.</summary>
        public void UpdateWave(int waveNumber)
        {
            if (waveText == null) return;
            waveText.text = $"{wavePrefix}{waveNumber}";
        }

        /// <summary>
        /// Muestra una cuenta regresiva entre waves.
        /// Llamar desde WaveManager cuando acaba una wave y antes de la siguiente.
        /// </summary>
        public void ShowWaveCountdown(float seconds)
        {
            if (countdownCoroutine != null)
                StopCoroutine(countdownCoroutine);

            countdownCoroutine = StartCoroutine(CountdownRoutine(seconds));
        }

        private IEnumerator CountdownRoutine(float seconds)
        {
            if (countdownText == null) yield break;

            countdownText.gameObject.SetActive(true);
            float remaining = seconds;

            while (remaining > 0f)
            {
                countdownText.text = $"Próxima wave en {Mathf.CeilToInt(remaining)}...";
                yield return new WaitForSeconds(1f);
                remaining -= 1f;
            }

            countdownText.gameObject.SetActive(false);
            countdownCoroutine = null;
        }
    }
}