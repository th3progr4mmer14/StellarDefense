namespace StellarDefense.Projectiles
{
    /// <summary>
    /// Contrato para entidades que pueden recibir daño.
    /// Lo implementan PlayerController y los enemigos.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>Aplica una cantidad de daño a esta entidad.</summary>
        void TakeDamage(int amount);
    }
}