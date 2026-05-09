using System;
using System.Collections;
using UnityEngine;
using StellarDefense.Data;
using StellarDefense.InputSystem;
using StellarDefense.Projectiles;
using StellarDefense.Managers;

namespace StellarDefense.Player
{
    /// <summary>
    /// Controlador principal del jugador: movimiento, disparo, vidas y daño.
    /// Expone eventos para que UI y managers reaccionen sin necesitar referencias directas.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PlayerController : MonoBehaviour, IDamageable
    {
        // ── Configuración inyectada ────────────────────────────────────
        [Header("Datos")]
        [Tooltip("ScriptableObject con la configuración global (vidas, velocidad, cooldown).")]
        [SerializeField] private GameSettings gameSettings;

        [Header("Disparo")]
        [Tooltip("Pool del que saca proyectiles al disparar.")]
        [SerializeField] private ProjectilePool playerProjectilePool;

        [Tooltip("Punto desde el que sale el proyectil (un Empty hijo del Player).")]
        [SerializeField] private Transform firePoint;

        [Header("Límites de movimiento")]
        [Tooltip("Coordenada X mínima permitida (borde izquierdo).")]
        [SerializeField] private float minX = -8f;

        [Tooltip("Coordenada X máxima permitida (borde derecho).")]
        [SerializeField] private float maxX = 8f;

        // ── Eventos públicos ───────────────────────────────────────────
        /// <summary>Disparado al recibir daño (no necesariamente morir).</summary>
        public event Action OnPlayerHit;

        /// <summary>Disparado al llegar a 0 vidas. El GameObject sigue activo para animaciones de muerte.</summary>
        public event Action OnPlayerDeath;

        /// <summary>Disparado cada vez que cambia el contador de vidas, con el valor nuevo.</summary>
        public event Action<int> OnLivesChanged;

        // ── Estado interno ─────────────────────────────────────────────
        private PlayerControls controls;
        private SpriteRenderer spriteRenderer;
        private int currentLives;
        private float lastShotTime = -999f;
        private bool isInvulnerable;
        private bool isDead;
        private Vector2 moveInput;

        public int CurrentLives => currentLives;
        public bool IsInvulnerable => isInvulnerable;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            controls = new PlayerControls();

            if (gameSettings == null)
            {
                Debug.LogError($"[{nameof(PlayerController)}] GameSettings no asignado.", this);
                enabled = false;
                return;
            }

            currentLives = gameSettings.InitialLives;
        }

        private void OnEnable()
        {
            // Null guard: si OnEnable se llama antes de Awake (caso edge tras recompilación
            // en runtime), creamos controls aquí. Awake también lo crea, pero esto evita el crash.
            controls ??= new PlayerControls();

            controls.Gameplay.Enable();
            controls.Gameplay.Move.performed += OnMovePerformed;
            controls.Gameplay.Move.canceled += OnMoveCanceled;
            controls.Gameplay.Shoot.performed += OnShootPerformed;
        }

        private void OnDisable()
        {
            // Null guard equivalente: si controls nunca se inicializó, no hay que desuscribir nada.
            if (controls == null) return;

            controls.Gameplay.Move.performed -= OnMovePerformed;
            controls.Gameplay.Move.canceled -= OnMoveCanceled;
            controls.Gameplay.Shoot.performed -= OnShootPerformed;
            controls.Gameplay.Disable();
        }

        private void Start()
        {
            // Notificamos las vidas iniciales para que la UI se sincronice al spawnear.
            OnLivesChanged?.Invoke(currentLives);

            // Reset del score al iniciar la partida. Si el ScoreManager no existe
            // (caso de Play directo en Gameplay sin pasar por MainMenu), no pasa nada.
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.ResetScore();

            // Forzar música de gameplay al iniciar.
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMusic(AudioManager.Instance.GetGameplayMusic());
        }

        private void Update()
        {
            if (isDead) return;
            HandleMovement();
        }

        // ── Input handlers ─────────────────────────────────────────────
        private void OnMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
            => moveInput = ctx.ReadValue<Vector2>();

        private void OnMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
            => moveInput = Vector2.zero;

        private void OnShootPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (isDead) return;
            TryShoot();
        }

        // ── Movimiento ─────────────────────────────────────────────────
        private void HandleMovement()
        {
            // Solo eje X para Space Invaders. Si en el futuro queremos vertical,
            // basta con extender este método sin tocar el contrato de input.
            float deltaX = moveInput.x * gameSettings.PlayerSpeed * Time.deltaTime;
            Vector3 newPos = transform.position + new Vector3(deltaX, 0f, 0f);
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            transform.position = newPos;
        }

        // ── Disparo ────────────────────────────────────────────────────
        private void TryShoot()
        {
            // Cooldown: comparamos contra el último tiempo de disparo.
            // Time.time es preferible a un timer manual porque Unity lo gestiona y no acumula error.
            if (Time.time - lastShotTime < gameSettings.ShootCooldown) return;

            if (playerProjectilePool == null) return;

            Projectile projectile = playerProjectilePool.Get();
            if (projectile == null) return; // pool agotado y sin growth permitido

            Vector2 origin = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
            projectile.Launch(origin, Vector2.up, gameSettings.PlayerProjectileSpeed);

            // Notificar al AudioManager. Null check porque puede no estar en algunas escenas de prueba.
            if (AudioManager.Instance != null) AudioManager.Instance.OnPlayerShoot();

            lastShotTime = Time.time;
        }

        // ── Daño y vidas ───────────────────────────────────────────────
        public void TakeDamage(int amount)
        {
            if (isDead || isInvulnerable) return;

            currentLives -= amount;
            if (currentLives < 0) currentLives = 0;

            OnPlayerHit?.Invoke();
            OnLivesChanged?.Invoke(currentLives);

            // SFX de hit
            if (AudioManager.Instance != null) AudioManager.Instance.OnPlayerHit();

            if (currentLives <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(InvulnerabilityRoutine());
            }
        }

        private void Die()
        {
            isDead = true;
            OnPlayerDeath?.Invoke();

            // Notificamos al GameManager para que cambie el estado a GameOver.
            // Validamos null porque en builds de desarrollo el GameManager puede
            // no estar presente (p.ej. si pulsas Play directamente en Gameplay sin pasar por MainMenu).
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndGame();
            }
        }

        /// <summary>
        /// Invulnerabilidad temporal con parpadeo del sprite.
        /// Coroutine para no bloquear Update con un timer manual.
        /// </summary>
        private IEnumerator InvulnerabilityRoutine()
        {
            isInvulnerable = true;
            float elapsed = 0f;
            const float blinkInterval = 0.1f;

            while (elapsed < gameSettings.InvulnerabilityDuration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval;
            }

            spriteRenderer.enabled = true;
            isInvulnerable = false;
        }
    }
}