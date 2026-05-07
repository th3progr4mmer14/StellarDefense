using UnityEngine;

namespace StellarDefense.Projectiles
{
    /// <summary>
    /// Proyectil disparado por el jugador. Daña enemigos al impactar y se
    /// devuelve al pool al chocar con cualquier collider válido (la matriz
    /// de Physics2D garantiza que solo recibe Enemy o Boundary).
    /// </summary>
    public sealed class PlayerProjectile : Projectile
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            // No comprobamos tags ni layers aquí: la matriz de Physics2D ya
            // filtra qué puede colisionar con qué. Confiamos en esa configuración.

            // Si el otro tiene un componente que recibe daño, se lo aplicamos.
            // Usamos TryGetComponent para evitar GetComponent + null check.
            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }

            // Sea Enemy o Boundary, el proyectil se va al pool.
            ReturnToPool();
        }
    }
}