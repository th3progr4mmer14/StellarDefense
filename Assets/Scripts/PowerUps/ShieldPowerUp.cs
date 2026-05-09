using UnityEngine;
using StellarDefense.Player;

namespace StellarDefense.PowerUps
{
    /// <summary>
    /// Power-up de escudo: activa invulnerabilidad temporal en el jugador.
    /// </summary>
    public sealed class ShieldPowerUp : PowerUp
    {
        [SerializeField] private float shieldDuration = 5f;

        protected override void Apply(PlayerController player)
        {
            player.ActivateShield(shieldDuration);
            if (Managers.AudioManager.Instance != null)
                Managers.AudioManager.Instance.OnUIClick();
        }
    }
}