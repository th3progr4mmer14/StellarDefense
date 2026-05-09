using UnityEngine;
using StellarDefense.Enemies;

namespace StellarDefense.PowerUps
{
    /// <summary>
    /// Spawner de power-ups. Se suscribe al evento de muerte de enemigos
    /// y con cierta probabilidad instancia un power-up aleatorio.
    /// </summary>
    public sealed class PowerUpSpawner : MonoBehaviour
    {
        [Header("Prefabs de power-ups")]
        [SerializeField] private PowerUp shieldPrefab;
        [SerializeField] private PowerUp extraLifePrefab;
        [SerializeField] private PowerUp tripleShotPrefab;

        [Header("Probabilidad")]
        [Tooltip("Probabilidad de que caiga un power-up al matar un enemigo (0-1).")]
        [SerializeField, Range(0f, 1f)] private float dropChance = 0.15f;

        private void OnEnable()
        {
            Enemy.OnAnyEnemyDestroyed += HandleEnemyDestroyed;
        }

        private void OnDisable()
        {
            Enemy.OnAnyEnemyDestroyed -= HandleEnemyDestroyed;
        }

        private void HandleEnemyDestroyed(int pointsValue)
        {
            if (Random.value > dropChance) return;

            // Elegimos un power-up aleatorio de los 3 tipos.
            PowerUp[] options = { shieldPrefab, extraLifePrefab, tripleShotPrefab };
            PowerUp chosen = options[Random.Range(0, options.Length)];
            if (chosen == null) return;

            // Spawneamos en una posición aleatoria horizontal.
            Vector3 spawnPos = new Vector3(
                Random.Range(-7f, 7f),
                6f, // Arriba de la pantalla, cae hacia abajo.
                0f
            );

            Instantiate(chosen, spawnPos, Quaternion.identity);
        }
    }
}