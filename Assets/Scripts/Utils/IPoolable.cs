namespace StellarDefense.Projectiles
{
    /// <summary>
    /// Contrato para objetos que pueden ser gestionados por un pool.
    /// Mantenerlo en interfaz desacopla la lógica del pool del tipo concreto.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>Llamado al sacar el objeto del pool y activarlo.</summary>
        void OnSpawnFromPool();

        /// <summary>Llamado al devolver el objeto al pool y desactivarlo.</summary>
        void OnReturnToPool();
    }
}