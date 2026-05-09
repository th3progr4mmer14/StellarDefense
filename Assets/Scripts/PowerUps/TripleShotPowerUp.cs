using UnityEngine;
using StellarDefense.Player;

namespace StellarDefense.PowerUps
{
    /// <summary>
    /// Power-up de triple disparo. El jugador dispara 3 proyectiles
    /// en abanico durante un tiempo limitado.
    /// </summary>
    public sealed class TripleShotPowerUp : PowerUp
    {
        [SerializeField] private float duration = 8f;

        protected override void Apply(PlayerController player)
        {
            player.ActivateTripleShot(duration);
            if (Managers.AudioManager.Instance != null)
                Managers.AudioManager.Instance.OnUIClick();
        }
    }
}