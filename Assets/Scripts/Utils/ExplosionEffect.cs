using System.Collections;
using UnityEngine;

namespace StellarDefense.Utils
{
    /// <summary>
    /// Efecto de explosión simple usando partículas procedurales.
    /// Se autodestruye al terminar la animación.
    /// </summary>
    public sealed class ExplosionEffect : MonoBehaviour
    {
        [SerializeField] private int particleCount = 8;
        [SerializeField] private float speed = 3f;
        [SerializeField] private float lifetime = 0.5f;
        [SerializeField] private Color color = Color.yellow;

        private void Start()
        {
            StartCoroutine(Explode());
        }

        private IEnumerator Explode()
        {
            // Creamos partículas en direcciones distribuidas uniformemente.
            for (int i = 0; i < particleCount; i++)
            {
                float angle = i * (360f / particleCount) * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                GameObject particle = new GameObject("Particle");
                particle.transform.position = transform.position;

                SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
                sr.sprite = CreateSquareSprite();
                sr.color = color;
                particle.transform.localScale = Vector3.one * 0.15f;

                StartCoroutine(MoveParticle(particle, direction));
            }

            yield return new WaitForSeconds(lifetime);
            Destroy(gameObject);
        }

        private IEnumerator MoveParticle(GameObject particle, Vector2 direction)
        {
            float elapsed = 0f;
            SpriteRenderer sr = particle.GetComponent<SpriteRenderer>();

            while (elapsed < lifetime && particle != null)
            {
                particle.transform.position += (Vector3)(direction * speed * Time.deltaTime);

                // Fade out progresivo.
                float alpha = Mathf.Lerp(1f, 0f, elapsed / lifetime);
                if (sr != null) sr.color = new Color(color.r, color.g, color.b, alpha);

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (particle != null) Destroy(particle);
        }

        private Sprite CreateSquareSprite()
        {
            Texture2D tex = new Texture2D(4, 4);
            Color[] pixels = new Color[16];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 4, 4), Vector2.one * 0.5f);
        }
    }
}