using System;
using System.Collections;
using UnityEngine;
using StellarDefense.Enemies;
using StellarDefense.UI;

namespace StellarDefense.Managers
{
    /// <summary>
    /// Coordinador de oleadas. Lanza waves secuencialmente, espera a que la
    /// formación quede vacía y avanza a la siguiente con una pausa intermedia.
    /// </summary>
    public sealed class WaveManager : MonoBehaviour
    {
        [Header("Configuración")]
        [Tooltip("Lista ordenada de waves a lanzar. La primera es la wave 1.")]
        [SerializeField] private WaveData[] waves;

        [Tooltip("Formación de enemigos que recibe los WaveData.")]
        [SerializeField] private EnemyFormation formation;

        [Tooltip("Pausa (segundos) entre waves.")]
        [SerializeField, Min(0f)] private float timeBetweenWaves = 3f;

        [Tooltip("Si está marcado, al terminar la última wave se reinicia el ciclo. " +
                 "Útil para gameplay infinito mientras desarrollamos el sistema de score.")]
        [SerializeField] private bool loopWaves = true;

        [Header("UI")]
        [Tooltip("HUDController para mostrar wave actual y countdown.")]
        [SerializeField] private HUDController hudController;

        // ── Eventos ────────────────────────────────────────────────────
        /// <summary>Disparado al iniciar una wave nueva. Param: número de wave (1-indexado).</summary>
        public event Action<int> OnWaveStarted;

        /// <summary>Disparado al completar todas las waves (solo si loopWaves está desactivado).</summary>
        public event Action OnAllWavesCompleted;

        private int currentWaveIndex;

        private void Start()
        {
            if (formation == null || waves == null || waves.Length == 0)
            {
                Debug.LogError($"[{nameof(WaveManager)}] Falta configuración: formation o waves.", this);
                enabled = false;
                return;
            }

            formation.OnAllEnemiesDefeated += HandleWaveCleared;
            StartCoroutine(StartFirstWaveAfterDelay(0.5f));
        }

        private void OnDestroy()
        {
            if (formation != null)
            {
                formation.OnAllEnemiesDefeated -= HandleWaveCleared;
            }
        }

        private IEnumerator StartFirstWaveAfterDelay(float delay)
        {
            // Pequeño delay inicial para dar tiempo a que el resto de sistemas
            // (UI, audio) se suscriban a OnWaveStarted antes del primer disparo.
            yield return new WaitForSeconds(delay);
            LaunchCurrentWave();
        }

        private void LaunchCurrentWave()
        {
            WaveData wave = waves[currentWaveIndex];
            formation.SpawnWave(wave);
            OnWaveStarted?.Invoke(wave.WaveNumber);

            if (hudController != null)
                hudController.UpdateWave(wave.WaveNumber);
        }

        private void HandleWaveCleared()
        {
            currentWaveIndex++;

            if (currentWaveIndex >= waves.Length)
            {
                if (loopWaves)
                {
                    currentWaveIndex = 0;
                }
                else
                {
                    OnAllWavesCompleted?.Invoke();
                    return;
                }
            }

            StartCoroutine(WaitAndLaunchNext());
        }

        private IEnumerator WaitAndLaunchNext()
        {
            if (hudController != null)
                hudController.ShowWaveCountdown(timeBetweenWaves);

            yield return new WaitForSeconds(timeBetweenWaves);
            LaunchCurrentWave();
        }
    }
}