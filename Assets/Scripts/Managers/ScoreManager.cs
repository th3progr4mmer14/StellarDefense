using System;
using UnityEngine;
using StellarDefense.Data;
using StellarDefense.Enemies;

namespace StellarDefense.Managers
{
    /// <summary>
    /// Gestor de puntuación con persistencia de high score y sistema de combo.
    /// Se suscribe al evento estático Enemy.OnAnyEnemyDestroyed para no
    /// requerir referencias directas a los enemigos.
    /// </summary>
    public sealed class ScoreManager : MonoBehaviour
    {
        // ── Singleton ──────────────────────────────────────────────────
        public static ScoreManager Instance { get; private set; }

        // ── Configuración ──────────────────────────────────────────────
        [Header("Datos")]
        [Tooltip("ScriptableObject con multiplicadores y ventana de combo.")]
        [SerializeField] private GameSettings gameSettings;

        // ── Constantes de persistencia ─────────────────────────────────
        // Centralizamos las claves para evitar typos al guardar/leer.
        private const string HighScoreKey = "StellarDefense.HighScore";

        // ── Eventos ────────────────────────────────────────────────────
        /// <summary>Disparado al cambiar el score actual. Param: nuevo valor.</summary>
        public event Action<int> OnScoreChanged;

        /// <summary>Disparado al batir el high score. Param: nuevo high score.</summary>
        public event Action<int> OnNewHighScore;

        /// <summary>Disparado cuando cambia el multiplicador de combo. Param: nuevo multiplicador.</summary>
        public event Action<int> OnComboChanged;

        // ── Estado ─────────────────────────────────────────────────────
        public int CurrentScore { get; private set; }
        public int HighScore { get; private set; }
        public int ComboMultiplier { get; private set; } = 1;

        private float lastKillTime = -999f;
        private bool highScoreBeatenThisRun;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadHighScore();
        }

        private void OnEnable()
        {
            // Suscripción al evento estático del Enemy. Cualquier muerte de enemigo
            // notifica aquí sin necesidad de referencias directas.
            Enemy.OnAnyEnemyDestroyed += HandleEnemyDestroyed;
        }

        private void OnDisable()
        {
            Enemy.OnAnyEnemyDestroyed -= HandleEnemyDestroyed;
        }

        // ── API pública ────────────────────────────────────────────────

        /// <summary>Resetea el score actual. Llamado al iniciar nueva partida.</summary>
        public void ResetScore()
        {
            CurrentScore = 0;
            ComboMultiplier = 1;
            lastKillTime = -999f;
            highScoreBeatenThisRun = false;
            OnScoreChanged?.Invoke(CurrentScore);
            OnComboChanged?.Invoke(ComboMultiplier);
        }

        /// <summary>Suma puntos manualmente (útil para power-ups o eventos especiales).</summary>
        public void AddPoints(int points)
        {
            ApplyScore(points);
        }

        // ── Lógica interna ─────────────────────────────────────────────

        private void HandleEnemyDestroyed(int basePoints)
        {
            UpdateCombo();
            int points = Mathf.RoundToInt(basePoints * ComboMultiplier);
            ApplyScore(points);
            lastKillTime = Time.time;
        }

        private void UpdateCombo()
        {
            if (gameSettings == null) return;

            float timeSinceLastKill = Time.time - lastKillTime;
            if (timeSinceLastKill <= gameSettings.ComboWindow)
            {
                // Mata seguida: incrementamos combo (cap a x10 para evitar inflación absurda).
                ComboMultiplier = Mathf.Min(ComboMultiplier + 1, 10);
            }
            else
            {
                // Demasiado tiempo desde la última: combo se resetea.
                ComboMultiplier = 1;
            }

            OnComboChanged?.Invoke(ComboMultiplier);
        }

        private void ApplyScore(int points)
        {
            CurrentScore += points;
            OnScoreChanged?.Invoke(CurrentScore);

            Debug.Log($"💰 Score: {CurrentScore} | Combo: x{ComboMultiplier} | HighScore: {HighScore}");

            // Comprobamos si se batió el high score, pero solo notificamos UNA vez por run
            // para evitar spam del evento si sigues sumando puntos.
            if (CurrentScore > HighScore)
            {
                HighScore = CurrentScore;
                SaveHighScore();

                if (!highScoreBeatenThisRun)
                {
                    highScoreBeatenThisRun = true;
                    OnNewHighScore?.Invoke(HighScore);
                }
            }
        }

        // ── Persistencia ───────────────────────────────────────────────

        private void LoadHighScore()
        {
            // GetInt con valor por defecto 0 si la clave no existe (primera vez jugando).
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        }

        private void SaveHighScore()
        {
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            // Save() fuerza la escritura inmediata. Sin esto, PlayerPrefs guarda
            // periódicamente, lo cual es eficiente pero arriesgado si el juego crashea.
            PlayerPrefs.Save();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}