using System.Collections;
using UnityEngine;
using StellarDefense.Player;

namespace StellarDefense.PowerUps
{
    /// <summary>
    /// Base abstracta para todos los power-ups.
    /// Cae hacia abajo al spawnearse y se autodestruye si el jugador
    /// no lo recoge en un tiempo límite.
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public abstract class PowerUp : MonoBehaviour
    {
        [Header("Movimiento")]
        [SerializeField] private float fallSpeed = 2f;

        [Header("Vida útil")]
        [SerializeField] private float lifetime = 8f;

        private void Start()
        {
            // Auto-destrucción si no lo recoge nadie.
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            // Cae hacia abajo constantemente.
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out PlayerController player)) return;
            Apply(player);
            Destroy(gameObject);
        }

        /// <summary>Efecto concreto del power-up. Lo implementa cada subclase.</summary>
        protected abstract void Apply(PlayerController player);
    }
}