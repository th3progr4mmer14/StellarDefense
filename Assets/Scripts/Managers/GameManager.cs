using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StellarDefense.Managers
{
    /// <summary>
    /// Coordinador central del juego. Mantiene el estado actual (menú, jugando,
    /// pausado, game over) y expone métodos para transicionar entre ellos.
    /// Singleton con DontDestroyOnLoad para sobrevivir entre escenas.
    /// </summary>
    public sealed class GameManager : MonoBehaviour
    {
        /// <summary>Estados posibles del juego. Cualquier comportamiento global
        /// debería revisar este enum antes de actuar (p.ej. el pause menu).</summary>
        public enum GameState
        {
            MainMenu,
            Playing,
            Paused,
            GameOver
        }

        // ── Singleton ──────────────────────────────────────────────────
        public static GameManager Instance { get; private set; }

        // ── Eventos ────────────────────────────────────────────────────
        /// <summary>Disparado cada vez que cambia el estado. Param: nuevo estado.</summary>
        public event Action<GameState> OnStateChanged;

        // ── Estado ─────────────────────────────────────────────────────
        public GameState CurrentState { get; private set; } = GameState.MainMenu;

        // ── Nombres de escena (centralizamos para evitar magic strings) ─
        [Header("Escenas")]
        [Tooltip("Nombre exacto de la escena del menú principal.")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        [Tooltip("Nombre exacto de la escena de gameplay.")]
        [SerializeField] private string gameplaySceneName = "Gameplay";

        private void Awake()
        {
            // Patrón Singleton clásico: si ya existe otra instancia, autodestrucción.
            // Esto previene duplicados al cargar escenas que también contienen GameManager.
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ── API pública: transiciones de estado ────────────────────────

        /// <summary>Carga la escena de gameplay y entra en estado Playing.</summary>
        public void StartGame()
        {
            Time.timeScale = 1f;
            ChangeState(GameState.Playing);
            SceneManager.LoadScene(gameplaySceneName);
        }

        /// <summary>Pone el juego en pausa. Solo válido si CurrentState == Playing.</summary>
        public void PauseGame()
        {
            if (CurrentState != GameState.Playing) return;
            Time.timeScale = 0f;
            ChangeState(GameState.Paused);
        }

        /// <summary>Reanuda el juego. Solo válido si CurrentState == Paused.</summary>
        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused) return;
            Time.timeScale = 1f;
            ChangeState(GameState.Playing);
        }

        /// <summary>Marca el juego como terminado (game over). No carga ninguna escena;
        /// la UI de game over es un overlay sobre Gameplay.</summary>
        public void EndGame()
        {
            if (CurrentState == GameState.GameOver) return;
            // Mantenemos timeScale = 1 para permitir animaciones del game over,
            // tweens de UI, etc. Si algún sistema quiere parar todo, lo hace
            // localmente en su Update revisando CurrentState.
            ChangeState(GameState.GameOver);
        }

        /// <summary>Reinicia la partida cargando Gameplay desde cero.</summary>
        public void RestartGame()
        {
            Time.timeScale = 1f;
            ChangeState(GameState.Playing);
            SceneManager.LoadScene(gameplaySceneName);
        }

        /// <summary>Vuelve al menú principal.</summary>
        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
            ChangeState(GameState.MainMenu);
        }

        // ── Cambio de estado interno ───────────────────────────────────
        private void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            Debug.Log($"🎮 GameState changed: {newState}");
            OnStateChanged?.Invoke(newState);
        }

        private void OnDestroy()
        {
            // Si la instancia activa se destruye (cierre de juego), limpiamos
            // la referencia estática para evitar punteros muertos.
            if (Instance == this) Instance = null;
        }
    }
}