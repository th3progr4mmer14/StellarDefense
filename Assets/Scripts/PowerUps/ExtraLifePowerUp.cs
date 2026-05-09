using UnityEngine;
using StellarDefense.Player;

namespace StellarDefense.PowerUps
{
    /// <summary>
    /// Power-up de vida extra. Añade una vida al jugador.
    /// </summary>
    public sealed class ExtraLifePowerUp : PowerUp
    {
        protected override void Apply(PlayerController player)
        {
            player.AddLife();
            if (Managers.AudioManager.Instance != null)
                Managers.AudioManager.Instance.OnUIClick();
        }
    }
}