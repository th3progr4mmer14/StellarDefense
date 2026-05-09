using UnityEngine;

namespace StellarDefense.Projectiles
{
    /// <summary>
    /// Clase base abstracta para cualquier proyectil del juego.
    /// Maneja movimiento constante en una dirección y aplicación de daño.
    /// Las subclases definen contra qué objetivos puede impactar mediante
    /// configuración de Layers (no chequeo en código).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public abstract class Projectile : MonoBehaviour, IPoolable
    {
        [Header("Movimiento")]
        [Tooltip("Velocidad en unidades/segundo. Se establece desde quien dispara.")]
        [SerializeField] protected float speed = 10f;

        [Header("Daño")]
        [Tooltip("Daño aplicado al impactar un objetivo válido.")]
        [SerializeField, Min(1)] protected int damage = 1;

        protected Rigidbody2D rb;
        protected Vector2 direction = Vector2.up;
        private ProjectilePool ownerPool;

        /// <summary>
        /// Daño que aplica este proyectil. Lo lee el receptor del impacto.
        /// </summary>
        public int Damage => damage;

        protected virtual void Awake()
        {
            // Cacheamos componentes en Awake para evitar GetComponent en hot paths.
            rb = GetComponent<Rigidbody2D>();

            // Kinematic + velocity manual = movimiento predecible sin física simulada.
            // Es lo correcto para proyectiles arcade: no queremos que choquen con paredes
            // físicamente ni que sean afectados por gravedad.
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }

        /// <summary>
        /// Configura el proyectil al sacarlo del pool. Llamar desde quien dispara.
        /// </summary>
        public void Launch(Vector2 origin, Vector2 direction, float speed)
        {
            transform.position = origin;
            this.direction = direction.normalized;
            this.speed = speed;

            // Orientamos el sprite hacia la dirección de vuelo (asumiendo sprite "mira" arriba).
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        /// <summary>
        /// Asigna el pool dueño para que el proyectil se sepa devolver al desactivarse.
        /// </summary>
        public void SetPool(ProjectilePool pool) => ownerPool = pool;

        protected virtual void FixedUpdate()
        {
            // Usamos MovePosition (kinematic) en FixedUpdate para que la física detecte
            // bien las colisiones aunque el proyectil sea rápido.
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }

        /// <summary>
        /// Devuelve el proyectil al pool. Si no tiene pool asignado, se destruye
        /// como fallback (caso edge: proyectil instanciado sin pool, p.ej. en debug).
        /// </summary>
        protected void ReturnToPool()
        {
            if (ownerPool != null)
            {
                ownerPool.Return(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ── IPoolable ──────────────────────────────────────────────────
        public virtual void OnSpawnFromPool()
        {
            // Hook para que las subclases reseteen estado si lo necesitan
            // (timers, partículas, etc.). Por defecto no hace nada extra.
        }

        public virtual void OnReturnToPool()
        {
            // Reseteamos transformaciones para evitar arrastrar valores de la vida anterior.
            direction = Vector2.up;
        }
    }
}