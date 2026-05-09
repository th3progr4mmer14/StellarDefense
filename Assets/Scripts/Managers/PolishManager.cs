using UnityEngine;
using StellarDefense.Enemies;
using StellarDefense.Player;
using StellarDefense.Utils;

namespace StellarDefense.Managers
{
    /// <summary>
    /// Coordinador de efectos visuales (polish). Se suscribe a eventos
    /// del juego y dispara explosiones, screen shake y texto flotante.
    /// Completamente desacoplado del gameplay.
    /// </summary>
    public sealed class PolishManager : MonoBehaviour
    {
        [Header("Prefabs de efectos")]
        [Tooltip("Prefab de la explosión. Debe tener el componente ExplosionEffect.")]
        [SerializeField] private ExplosionEffect explosionPrefab;

        [Tooltip("Prefab del texto flotante. Debe tener FloatingText.")]
        [SerializeField] private FloatingText floatingTextPrefab;

        [Header("Referencias")]
        [Tooltip("PlayerController para suscribirse al evento de daño.")]
        [SerializeField] private PlayerController playerController;

        [Header("Colores de texto flotante")]
        [SerializeField] private Color normalPointsColor = Color.white;
        [SerializeField] private Color comboPointsColor = Color.yellow;

        // Guardamos la última posición de muerte de un enemigo para
        // spawnear la explosión y el texto en el sitio correcto.
        private Vector3 lastEnemyDeathPosition;

        private void OnEnable()
        {
            Enemy.OnAnyEnemyDestroyed += HandleEnemyDestroyed;

            if (playerController != null)
                playerController.OnPlayerHit += HandlePlayerHit;
        }

        private void OnDisable()
        {
            Enemy.OnAnyEnemyDestroyed -= HandleEnemyDestroyed;

            if (playerController != null)
                playerController.OnPlayerHit -= HandlePlayerHit;
        }

        // ── Handlers ───────────────────────────────────────────────────

        private void HandleEnemyDestroyed(int basePoints)
        {
            // Explosión en la posición del enemigo.
            SpawnExplosion(lastEnemyDeathPosition, Color.yellow);

            // Texto flotante con puntos.
            int combo = ScoreManager.Instance != null
                ? ScoreManager.Instance.ComboMultiplier : 1;

            int totalPoints = basePoints * combo;
            string text = combo > 1
                ? $"+{totalPoints}\nx{combo}"
                : $"+{totalPoints}";

            Color textColor = combo > 1 ? comboPointsColor : normalPointsColor;
            SpawnFloatingText(lastEnemyDeathPosition, text, textColor);
        }

        private void HandlePlayerHit()
        {
            // Screen shake al recibir daño.
            if (ScreenShake.Instance != null)
                ScreenShake.Instance.Shake(0.2f, 0.2f);
        }

        // ── Spawn de efectos ───────────────────────────────────────────

        private void SpawnExplosion(Vector3 position, Color color)
        {
            if (explosionPrefab == null) return;
            ExplosionEffect fx = Instantiate(explosionPrefab, position, Quaternion.identity);
        }

        private void SpawnFloatingText(Vector3 position, string text, Color color)
        {
            if (floatingTextPrefab == null) return;
            FloatingText ft = Instantiate(floatingTextPrefab,
                position + Vector3.up * 0.5f, Quaternion.identity);
            ft.Initialize(text, color);
        }

        /// <summary>
        /// Llamar desde Enemy.Die() para registrar la posición de muerte.
        /// </summary>
        public static void RegisterEnemyDeathPosition(Vector3 position)
        {
            if (instance != null) instance.lastEnemyDeathPosition = position;
        }

        private static PolishManager instance;
        private void Awake() => instance = this;
        private void OnDestroy() { if (instance == this) instance = null; }
    }
}