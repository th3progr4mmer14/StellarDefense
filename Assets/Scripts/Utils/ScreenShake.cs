using System.Collections;
using UnityEngine;

namespace StellarDefense.Utils
{
    /// <summary>
    /// Efecto de screen shake en la cámara principal.
    /// Singleton ligero que puede llamarse desde cualquier sitio.
    /// </summary>
    public sealed class ScreenShake : MonoBehaviour
    {
        public static ScreenShake Instance { get; private set; }

        private Vector3 originalPosition;
        private Coroutine shakeCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            originalPosition = transform.localPosition;
        }

        /// <summary>Sacude la cámara con intensidad y duración dadas.</summary>
        public void Shake(float duration = 0.2f, float magnitude = 0.15f)
        {
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
        }

        private IEnumerator ShakeRoutine(float duration, float magnitude)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = originalPosition + new Vector3(x, y, 0f);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPosition;
            shakeCoroutine = null;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}