using System.Collections.Generic;
using UnityEngine;

namespace StellarDefense.Projectiles
{
    /// <summary>
    /// Object Pool genérico para proyectiles. Evita Instantiate/Destroy en runtime
    /// y por tanto las pausas del Garbage Collector causadas por el spam de balas.
    /// Hay un pool independiente por tipo: uno para PlayerProjectile, otro para EnemyProjectile.
    /// </summary>
    public sealed class ProjectilePool : MonoBehaviour
    {
        [Header("Configuración")]
        [Tooltip("Prefab del proyectil. Debe tener un componente que herede de Projectile.")]
        [SerializeField] private Projectile prefab;

        [Tooltip("Cantidad pre-instanciada al inicio. Evita instanciar durante gameplay.")]
        [SerializeField, Min(0)] private int prewarmCount = 20;

        [Tooltip("Si está marcado, el pool puede crear más allá del prewarm cuando se necesite. " +
                 "Si está desmarcado, devuelve null si se queda sin proyectiles disponibles.")]
        [SerializeField] private bool allowGrowth = true;

        private readonly Stack<Projectile> available = new Stack<Projectile>();

        private void Awake()
        {
            if (prefab == null)
            {
                Debug.LogError($"[{nameof(ProjectilePool)}] Prefab no asignado en {name}.", this);
                return;
            }

            for (int i = 0; i < prewarmCount; i++)
            {
                Projectile instance = CreateInstance();
                instance.gameObject.SetActive(false);
                available.Push(instance);
            }
        }

        /// <summary>
        /// Saca un proyectil del pool. Devuelve null si no hay disponibles
        /// y allowGrowth está desactivado.
        /// </summary>
        public Projectile Get()
        {
            Projectile projectile;

            if (available.Count > 0)
            {
                projectile = available.Pop();
            }
            else if (allowGrowth)
            {
                projectile = CreateInstance();
            }
            else
            {
                return null;
            }

            projectile.gameObject.SetActive(true);
            projectile.OnSpawnFromPool();
            return projectile;
        }

        /// <summary>
        /// Devuelve un proyectil al pool. El proyectil se desactiva pero no se destruye.
        /// </summary>
        public void Return(Projectile projectile)
        {
            if (projectile == null) return;

            projectile.OnReturnToPool();
            projectile.gameObject.SetActive(false);
            available.Push(projectile);
        }

        private Projectile CreateInstance()
        {
            // Instanciamos como hijo del pool para que la jerarquía no se llene de objetos sueltos.
            Projectile instance = Instantiate(prefab, transform);
            instance.SetPool(this);
            return instance;
        }
    }
}