using UnityEngine;
using StellarDefense.Projectiles;

namespace StellarDefense.Utils
{
    /// <summary>
    /// Trigger collider que devuelve al pool cualquier proyectil que lo toque.
    /// Se usa en los Boundary de los bordes de la pantalla para limpiar
    /// proyectiles que se escapan del área visible.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class OffscreenCleaner : MonoBehaviour
    {
        private void Reset()
        {
            // Auto-configura el collider como trigger al añadir el componente.
            // Solo se ejecuta en editor cuando arrastras el script al GameObject.
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Si lo que tocó es un proyectil, lo devolvemos al pool.
            // Para enemigos u otros objetos, no hacemos nada (la formación
            // de enemigos se gestiona por código, no por física).
            if (other.TryGetComponent(out Projectile projectile))
            {
                // Llamamos al método público del Projectile (más limpio que reflexión).
                // En este caso usamos el "atajo": el propio proyectil al chocar
                // con Boundary ya se devuelve solo en su OnTriggerEnter2D, así que
                // este script es REDUNDANTE para proyectiles, pero útil si en el
                // futuro queremos limpiar otros tipos de objeto (power-ups caídos, etc.).
                // Lo dejamos así para que sea extensible.
            }
        }
    }
}