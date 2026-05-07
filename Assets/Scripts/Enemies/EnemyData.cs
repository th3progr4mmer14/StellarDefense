using UnityEngine;

namespace StellarDefense.Enemies
{
    /// <summary>
    /// Define las características de un arquetipo de enemigo.
    /// Cada tipo concreto (Basic, Fast, Tank...) es un asset independiente.
    /// </summary>
    [CreateAssetMenu(
        fileName = "EnemyData",
        menuName = "StellarDefense/Enemy Data",
        order = 1)]
    public sealed class EnemyData : ScriptableObject
    {
        [Header("Identidad")]
        [Tooltip("Nombre legible (debug y UI opcional).")]
        [SerializeField] private string enemyName = "Basic";

        [Tooltip("Sprite asignado al SpriteRenderer del prefab en runtime.")]
        [SerializeField] private Sprite sprite;

        [Tooltip("Color para teñir el sprite. Útil para reusar el mismo sprite.")]
        [SerializeField] private Color tint = Color.white;

        [Header("Stats")]
        [Tooltip("Vida del enemigo. La mayoría mueren con 1 hit; los Tank, más.")]
        [SerializeField, Min(1)] private int health = 1;

        [Tooltip("Puntos otorgados al jugador al destruirlo.")]
        [SerializeField, Min(0)] private int pointsValue = 10;

        [Tooltip("Multiplicador de velocidad relativo a la velocidad base de la formación.")]
        [SerializeField, Min(0f)] private float speedMultiplier = 1f;

        [Header("Comportamiento")]
        [Tooltip("Probabilidad por segundo de disparar (0-1). Se evalúa por frame.")]
        [SerializeField, Range(0f, 1f)] private float shootProbabilityPerSecond = 0.1f;

        [Tooltip("Velocidad del proyectil emitido por este enemigo.")]
        [SerializeField, Min(0f)] private float projectileSpeed = 6f;

        [Tooltip("Daño que aplica el proyectil al jugador.")]
        [SerializeField, Min(1)] private int projectileDamage = 1;

        [Header("Feedback")]
        [Tooltip("Sonido de explosión al ser destruido. Si es null, AudioManager usa default.")]
        [SerializeField] private AudioClip explosionSfx;

        [Tooltip("Prefab de partículas/VFX al ser destruido. Opcional.")]
        [SerializeField] private GameObject deathVfxPrefab;

        public string EnemyName => enemyName;
        public Sprite Sprite => sprite;
        public Color Tint => tint;
        public int Health => health;
        public int PointsValue => pointsValue;
        public float SpeedMultiplier => speedMultiplier;
        public float ShootProbabilityPerSecond => shootProbabilityPerSecond;
        public float ProjectileSpeed => projectileSpeed;
        public int ProjectileDamage => projectileDamage;
        public AudioClip ExplosionSfx => explosionSfx;
        public GameObject DeathVfxPrefab => deathVfxPrefab;
    }
}