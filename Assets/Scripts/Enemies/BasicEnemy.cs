namespace StellarDefense.Enemies
{
    /// <summary>
    /// Enemigo estándar. Usa el comportamiento base sin modificaciones.
    /// El "tipo" se diferencia por el EnemyData asignado (stats, sprite, color).
    /// </summary>
    public sealed class BasicEnemy : Enemy
    {
        // Hereda todo de Enemy. Aquí podrían ir comportamientos específicos
        // del Basic en el futuro (p.ej. patrón de movimiento sinusoidal).
    }
}