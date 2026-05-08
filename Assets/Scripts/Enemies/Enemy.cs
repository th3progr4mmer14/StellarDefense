using System;
using UnityEngine;
using StellarDefense.Managers;
using StellarDefense.Projectiles;

namespace StellarDefense.Enemies
{
    /// <summary>
    /// Componente base para cualquier enemigo. Maneja vida, daño, disparo y muerte.
    /// La configuración (stats, probabilidades, sprites) viene inyectada desde
    /// EnemyData (ScriptableObject) para iterar valores sin recompilar.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class Enemy : MonoBehaviour, IDamageable
    {
        [Header("Datos")]
        [Tooltip("ScriptableObject con stats y comportamiento de este enemigo.")]
        [SerializeField] private EnemyData data;

        [Tooltip("Punto desde el que dispara (Empty hijo). Si es null, dispara desde el centro.")]
        [SerializeField] private Transform firePoint;

        // ── Eventos ────────────────────────────────────────────────────
        /// <summary>Evento estático global para que cualquier sistema reaccione a muertes.
        /// El parámetro es el valor en puntos del enemigo. Se usa estático para no
        /// requerir referencias directas desde ScoreManager (Fase 3).</summary>
        public static event Action<int> OnAnyEnemyDestroyed;

        /// <summary>Disparado cuando ESTA instancia muere. Lo usa la formación para llevar la cuenta.</summary>
        public event Action<Enemy> OnDestroyed;

        // ── Estado ─────────────────────────────────────────────────────
        private SpriteRenderer spriteRenderer;
        private int currentHealth;
        private ProjectilePool enemyProjectilePool;
        private bool canShoot;

        public EnemyData Data => data;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Inicializa el enemigo cuando lo spawnea la formación. Reset completo de estado.
        /// </summary>
        public virtual void Initialize(EnemyData data, ProjectilePool projectilePool)
        {
            this.data = data;
            enemyProjectilePool = projectilePool;
            currentHealth = data.Health;

            // Aplicamos sprite y color desde el data. Permite tener un solo prefab
            // genérico y muchos arquetipos visuales sin duplicar GameObjects.
            if (data.Sprite != null) spriteRenderer.sprite = data.Sprite;
            spriteRenderer.color = data.Tint;
            spriteRenderer.enabled = true;

            canShoot = true;
        }

        protected virtual void Update()
        {
            if (!canShoot || enemyProjectilePool == null) return;
            TryShoot();
        }

        // ── Disparo aleatorio ──────────────────────────────────────────
        private void TryShoot()
        {
            // Probabilidad por segundo evaluada por frame. Multiplicamos por deltaTime
            // para que sea independiente del framerate.
            float chance = data.ShootProbabilityPerSecond * Time.deltaTime;
            if (UnityEngine.Random.value > chance) return;

            Projectile projectile = enemyProjectilePool.Get();
            if (projectile == null) return;

            Vector2 origin = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
            // Los enemigos disparan hacia abajo (hacia el jugador).
            projectile.Launch(origin, Vector2.down, data.ProjectileSpeed);
            if (AudioManager.Instance != null) AudioManager.Instance.OnEnemyShoot();
        }

        // ── Daño ───────────────────────────────────────────────────────
        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            canShoot = false;

            // Notificamos a quienes nos escuchen ANTES de desactivar el GameObject.
            OnDestroyed?.Invoke(this);
            OnAnyEnemyDestroyed?.Invoke(data.PointsValue);

            // VFX y SFX se gestionarán desde AudioManager/effects en fases siguientes.
            // Por ahora, simplemente desactivamos.
            gameObject.SetActive(false);
        }
    }
}