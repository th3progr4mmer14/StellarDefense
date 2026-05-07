using UnityEngine;

namespace StellarDefense.Projectiles
{
    /// <summary>
    /// Proyectil disparado por enemigos. Daña al jugador y se devuelve al pool
    /// al chocar con cualquier collider válido. La matriz de Physics2D filtra
    /// para que solo reciba Player o Boundary.
    /// </summary>
    public sealed class EnemyProjectile : Projectile
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }

            ReturnToPool();
        }
    }
}