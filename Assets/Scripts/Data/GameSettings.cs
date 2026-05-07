using UnityEngine;

namespace StellarDefense.Data
{
    /// <summary>
    /// Configuración global del juego: balance, vidas, velocidades y duración
    /// de power-ups. Centralizamos aquí los parámetros para iterar sin
    /// recompilar y evitar magic numbers dispersos por el código.
    /// </summary>
    [CreateAssetMenu(
        fileName = "GameSettings",
        menuName = "StellarDefense/Game Settings",
        order = 0)]
    public sealed class GameSettings : ScriptableObject
    {
        [Header("Jugador")]
        [Tooltip("Vidas con las que arranca cada partida.")]
        [SerializeField, Min(1)] private int initialLives = 3;

        [Tooltip("Vidas máximas que puede acumular el jugador.")]
        [SerializeField, Min(1)] private int maxLives = 5;

        [Tooltip("Velocidad horizontal del jugador en unidades/segundo.")]
        [SerializeField, Min(0f)] private float playerSpeed = 8f;

        [Tooltip("Tiempo mínimo entre disparos del jugador (segundos).")]
        [SerializeField, Min(0f)] private float shootCooldown = 0.25f;

        [Tooltip("Duración de la invulnerabilidad post-hit (segundos).")]
        [SerializeField, Min(0f)] private float invulnerabilityDuration = 1.5f;

        [Tooltip("Velocidad de los proyectiles del jugador.")]
        [SerializeField, Min(0f)] private float playerProjectileSpeed = 14f;

        [Header("Puntuación")]
        [Tooltip("Puntos base por enemigo. Se multiplican por el valor del EnemyData.")]
        [SerializeField, Min(0)] private int basePointsPerEnemy = 10;

        [Tooltip("Multiplicador de puntos aplicado por cada wave superada.")]
        [SerializeField, Min(1f)] private float scoreMultiplierPerWave = 1.1f;

        [Tooltip("Tiempo máximo (s) entre kills para mantener combo activo.")]
        [SerializeField, Min(0f)] private float comboWindow = 1.5f;

        [Header("Dificultad")]
        [Tooltip("Multiplicador de velocidad de la formación por wave.")]
        [SerializeField, Min(1f)] private float enemySpeedMultiplierPerWave = 1.15f;

        [Tooltip("Pausa (s) entre el fin de una wave y el inicio de la siguiente.")]
        [SerializeField, Min(0f)] private float timeBetweenWaves = 3f;

        [Header("Power-Ups")]
        [Tooltip("Probabilidad (0-1) de que un enemigo destruido suelte un power-up.")]
        [SerializeField, Range(0f, 1f)] private float powerUpDropChance = 0.05f;

        [Tooltip("Duración del escudo (segundos).")]
        [SerializeField, Min(0f)] private float shieldDuration = 5f;

        [Tooltip("Duración del disparo triple (segundos).")]
        [SerializeField, Min(0f)] private float tripleShotDuration = 8f;

        [Tooltip("Velocidad de caída de los power-ups recogibles.")]
        [SerializeField, Min(0f)] private float powerUpFallSpeed = 2.5f;

        [Header("Audio")]
        [Tooltip("Volumen master por defecto (0-1) si no hay valor guardado.")]
        [SerializeField, Range(0f, 1f)] private float defaultMasterVolume = 0.8f;

        [Tooltip("Volumen música por defecto (0-1).")]
        [SerializeField, Range(0f, 1f)] private float defaultMusicVolume = 0.6f;

        [Tooltip("Volumen SFX por defecto (0-1).")]
        [SerializeField, Range(0f, 1f)] private float defaultSfxVolume = 0.8f;

        // ── Properties expuestas read-only ──────────────────────────────
        public int InitialLives => initialLives;
        public int MaxLives => maxLives;
        public float PlayerSpeed => playerSpeed;
        public float ShootCooldown => shootCooldown;
        public float InvulnerabilityDuration => invulnerabilityDuration;
        public float PlayerProjectileSpeed => playerProjectileSpeed;
        public int BasePointsPerEnemy => basePointsPerEnemy;
        public float ScoreMultiplierPerWave => scoreMultiplierPerWave;
        public float ComboWindow => comboWindow;
        public float EnemySpeedMultiplierPerWave => enemySpeedMultiplierPerWave;
        public float TimeBetweenWaves => timeBetweenWaves;
        public float PowerUpDropChance => powerUpDropChance;
        public float ShieldDuration => shieldDuration;
        public float TripleShotDuration => tripleShotDuration;
        public float PowerUpFallSpeed => powerUpFallSpeed;
        public float DefaultMasterVolume => defaultMasterVolume;
        public float DefaultMusicVolume => defaultMusicVolume;
        public float DefaultSfxVolume => defaultSfxVolume;
    }
}