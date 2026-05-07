using UnityEngine;

namespace StellarDefense.Enemies
{
    /// <summary>
    /// Define una oleada concreta: estructura de la formación, tipos de
    /// enemigos por fila, espaciado y parámetros de movimiento. Cada wave
    /// es un asset independiente para diseñar progresión a mano sin código.
    /// </summary>
    [CreateAssetMenu(
        fileName = "WaveData",
        menuName = "StellarDefense/Wave Data",
        order = 2)]
    public sealed class WaveData : ScriptableObject
    {
        /// <summary>
        /// Una fila concreta de la formación. La fila 0 se renderiza arriba
        /// del todo y las siguientes descienden en pantalla.
        /// </summary>
        [System.Serializable]
        public struct EnemyRow
        {
            [Tooltip("Tipo de enemigo de esta fila.")]
            public EnemyData enemyData;

            [Tooltip("Cantidad de enemigos en esta fila.")]
            [Min(1)] public int count;
        }

        [Header("Identidad")]
        [Tooltip("Nombre de la wave (debug y UI opcional).")]
        [SerializeField] private string waveName = "Wave 1";

        [Tooltip("Número de wave (1-indexado). Se muestra en HUD.")]
        [SerializeField, Min(1)] private int waveNumber = 1;

        [Header("Composición")]
        [Tooltip("Filas de la formación, de arriba hacia abajo.")]
        [SerializeField] private EnemyRow[] rows;

        [Header("Espaciado")]
        [Tooltip("Distancia horizontal entre enemigos contiguos.")]
        [SerializeField, Min(0.1f)] private float horizontalSpacing = 1.2f;

        [Tooltip("Distancia vertical entre filas.")]
        [SerializeField, Min(0.1f)] private float verticalSpacing = 1f;

        [Tooltip("Posición Y inicial del centro de la formación (mundo).")]
        [SerializeField] private float spawnYOffset = 3.5f;

        [Header("Movimiento")]
        [Tooltip("Velocidad horizontal inicial de la formación (unidades/s).")]
        [SerializeField, Min(0f)] private float initialSpeed = 1.5f;

        [Tooltip("Cuánto baja la formación cada vez que toca un borde.")]
        [SerializeField, Min(0f)] private float dropDistance = 0.5f;

        [Tooltip("Multiplicador de velocidad aplicado tras cada bajada. " +
                 "Replica el aumento de tensión clásico de Space Invaders.")]
        [SerializeField, Min(1f)] private float speedupOnDrop = 1.05f;

        [Header("Disparo enemigo")]
        [Tooltip("Cantidad máxima de proyectiles enemigos vivos a la vez. " +
                 "Limita la dificultad sin tocar la probabilidad individual.")]
        [SerializeField, Min(1)] private int maxConcurrentEnemyProjectiles = 3;

        public string WaveName => waveName;
        public int WaveNumber => waveNumber;
        public EnemyRow[] Rows => rows;
        public float HorizontalSpacing => horizontalSpacing;
        public float VerticalSpacing => verticalSpacing;
        public float SpawnYOffset => spawnYOffset;
        public float InitialSpeed => initialSpeed;
        public float DropDistance => dropDistance;
        public float SpeedupOnDrop => speedupOnDrop;
        public int MaxConcurrentEnemyProjectiles => maxConcurrentEnemyProjectiles;

        /// <summary>
        /// Total de enemigos en la formación. Lo usa <c>EnemyFormation</c>
        /// para saber cuándo disparar <c>OnAllEnemiesDefeated</c>.
        /// </summary>
        public int GetTotalEnemyCount()
        {
            if (rows == null) return 0;
            int total = 0;
            for (int i = 0; i < rows.Length; i++) total += rows[i].count;
            return total;
        }
    }
}