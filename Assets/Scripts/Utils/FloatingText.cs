using System.Collections;
using UnityEngine;
using TMPro;

namespace StellarDefense.Utils
{
    /// <summary>
    /// Texto flotante que sube y se desvanece. Se usa para mostrar
    /// los puntos ganados al matar un enemigo (+10, +20 x3, etc.).
    /// </summary>
    public sealed class FloatingText : MonoBehaviour
    {
        [SerializeField] private float riseSpeed = 1.5f;
        [SerializeField] private float lifetime = 0.8f;

        private TextMeshPro tmp;

        private void Awake()
        {
            tmp = GetComponent<TextMeshPro>();
            if (tmp == null)
            {
                tmp = gameObject.AddComponent<TextMeshPro>();
                tmp.fontSize = 3f;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.fontStyle = FontStyles.Bold;
            }
        }

        public void Initialize(string text, Color color)
        {
            if (tmp != null)
            {
                tmp.text = text;
                tmp.color = color;
            }
            StartCoroutine(AnimateRoutine());
        }

        private IEnumerator AnimateRoutine()
        {
            float elapsed = 0f;
            Color originalColor = tmp != null ? tmp.color : Color.white;

            while (elapsed < lifetime)
            {
                transform.position += Vector3.up * riseSpeed * Time.deltaTime;

                float alpha = Mathf.Lerp(1f, 0f, elapsed / lifetime);
                if (tmp != null)
                    tmp.color = new Color(originalColor.r, originalColor.g,
                                         originalColor.b, alpha);

                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}