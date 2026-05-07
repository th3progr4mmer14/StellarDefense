using System;
using System.Collections.Generic;
using UnityEngine;
using StellarDefense.Projectiles;

namespace StellarDefense.Enemies
{
    /// <summary>
    /// Gestiona el spawn y movimiento de un grupo de enemigos en formación.
    /// Implementa el patrón clásico de Space Invaders: movimiento lateral,
    /// inversión de dirección al borde y descenso progresivo.
    /// </summary>
    public sealed class EnemyFormation : MonoBehaviour
    {
        [Header("Prefabs por tipo")]
        [Tooltip("Prefab del enemigo Basic. Debe tener componente BasicEnemy.")]
        [SerializeField] private BasicEnemy basicEnemyPrefab;

        [Tooltip("Prefab del enemigo Fast. Debe tener componente FastEnemy.")]
        [SerializeField] private FastEnemy fastEnemyPrefab;

        [Tooltip("Prefab del enemigo Tank. Debe tener componente TankEnemy.")]
        [SerializeField] private TankEnemy tankEnemyPrefab;

        [Header("Disparo")]
        [Tooltip("Pool del que sacan proyectiles los enemigos.")]
        [SerializeField] private ProjectilePool enemyProjectilePool;

        [Header("Límites")]
        [Tooltip("Coordenada X mínima a la que la formación puede llegar antes de invertir.")]
        [SerializeField] private float minX = -7.5f;

        [Tooltip("Coordenada X máxima.")]
        [SerializeField] private float maxX = 7.5f;

        // ── Eventos ────────────────────────────────────────────────────
        /// <summary>Disparado cuando todos los enemigos de la wave están eliminados.</summary>
        public event Action OnAllEnemiesDefeated;

        // ── Estado ─────────────────────────────────────────────────────
        private readonly List<Enemy> activeEnemies = new List<Enemy>();
        private float currentSpeed;
        private float dropDistance;
        private float speedupOnDrop;
        private int direction = 1; // 1 = derecha, -1 = izquierda
        private bool isMoving;

        /// <summary>
        /// Spawnea la formación según un WaveData. Resetea el estado por completo.
        /// </summary>
        public void SpawnWave(WaveData wave)
        {
            ClearExistingEnemies();

            currentSpeed = wave.InitialSpeed;
            dropDistance = wave.DropDistance;
            speedupOnDrop = wave.SpeedupOnDrop;
            direction = 1;

            // Calculamos la fila más ancha para centrar la formación horizontalmente.
            int maxColumns = 0;
            foreach (var row in wave.Rows)
            {
                if (row.count > maxColumns) maxColumns = row.count;
            }

            float formationWidth = (maxColumns - 1) * wave.HorizontalSpacing;
            float startX = -formationWidth * 0.5f;
            float currentY = wave.SpawnYOffset;

            // Spawneamos fila por fila desde arriba hacia abajo.
            for (int rowIndex = 0; rowIndex < wave.Rows.Length; rowIndex++)
            {
                WaveData.EnemyRow row = wave.Rows[rowIndex];
                if (row.enemyData == null) continue;

                Enemy prefab = SelectPrefabFor(row.enemyData);
                if (prefab == null)
                {
                    Debug.LogWarning($"[{nameof(EnemyFormation)}] No hay prefab para EnemyData '{row.enemyData.EnemyName}'.");
                    continue;
                }

                // Centramos la fila respecto al ancho máximo.
                float rowWidth = (row.count - 1) * wave.HorizontalSpacing;
                float rowStartX = -rowWidth * 0.5f;

                for (int col = 0; col < row.count; col++)
                {
                    Vector3 spawnPos = new Vector3(
                        rowStartX + col * wave.HorizontalSpacing,
                        currentY,
                        0f);

                    Enemy enemy = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
                    enemy.Initialize(row.enemyData, enemyProjectilePool);
                    enemy.OnDestroyed += HandleEnemyDestroyed;
                    activeEnemies.Add(enemy);
                }

                currentY -= wave.VerticalSpacing;
            }

            isMoving = activeEnemies.Count > 0;
        }

        private Enemy SelectPrefabFor(EnemyData data)
        {
            // Selección simple por nombre. En el futuro podríamos usar un dictionary
            // o un campo "prefabReference" dentro del propio EnemyData para escalar mejor.
            return data.EnemyName switch
            {
                "Basic" => basicEnemyPrefab,
                "Fast"  => fastEnemyPrefab,
                "Tank"  => tankEnemyPrefab,
                _       => basicEnemyPrefab // fallback
            };
        }

        private void Update()
        {
            if (!isMoving || activeEnemies.Count == 0) return;
            MoveFormation();
        }

        private void MoveFormation()
        {
            float deltaX = currentSpeed * direction * Time.deltaTime;

            // Buscamos el enemigo más a la izquierda y más a la derecha en cada frame
            // para detectar si la formación toca algún borde.
            float minEnemyX = float.MaxValue;
            float maxEnemyX = float.MinValue;

            for (int i = 0; i < activeEnemies.Count; i++)
            {
                Enemy e = activeEnemies[i];
                if (e == null) continue;
                float x = e.transform.position.x;
                if (x < minEnemyX) minEnemyX = x;
                if (x > maxEnemyX) maxEnemyX = x;
            }

            // ¿Algún enemigo se saldría tras este movimiento? Si sí, bajamos y revertimos.
            bool willHitBorder =
                (direction == 1 && maxEnemyX + deltaX > maxX) ||
                (direction == -1 && minEnemyX + deltaX < minX);

            if (willHitBorder)
            {
                DropAndReverse();
                return;
            }

            // Movimiento lateral normal: trasladamos a todos.
            Vector3 offset = new Vector3(deltaX, 0f, 0f);
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                Enemy e = activeEnemies[i];
                if (e == null) continue;
                e.transform.position += offset;
            }
        }

        private void DropAndReverse()
        {
            direction *= -1;
            currentSpeed *= speedupOnDrop;

            Vector3 dropOffset = new Vector3(0f, -dropDistance, 0f);
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                Enemy e = activeEnemies[i];
                if (e == null) continue;
                e.transform.position += dropOffset;
            }
        }

        private void HandleEnemyDestroyed(Enemy enemy)
        {
            enemy.OnDestroyed -= HandleEnemyDestroyed;
            activeEnemies.Remove(enemy);

            if (activeEnemies.Count == 0)
            {
                isMoving = false;
                OnAllEnemiesDefeated?.Invoke();
            }
        }

        private void ClearExistingEnemies()
        {
            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                Enemy e = activeEnemies[i];
                if (e == null) continue;
                e.OnDestroyed -= HandleEnemyDestroyed;
                Destroy(e.gameObject);
            }
            activeEnemies.Clear();
        }
    }
}