using UnityEngine;
using UnityEngine.Audio;
using StellarDefense.Data;
using StellarDefense.Enemies;
using StellarDefense.Player;

namespace StellarDefense.Managers
{
    /// <summary>
    /// Gestor central de audio. Reproduce música y SFX, controla volúmenes mediante
    /// AudioMixer y persiste preferencias en PlayerPrefs. Se suscribe a eventos
    /// del juego para reproducir SFX automáticamente sin necesidad de referencias directas.
    /// </summary>
    public sealed class AudioManager : MonoBehaviour
    {
        // ── Singleton ──────────────────────────────────────────────────
        public static AudioManager Instance { get; private set; }

        // ── Configuración del Mixer ────────────────────────────────────
        [Header("Mixer")]
        [Tooltip("AudioMixer principal con grupos Music y SFX expuestos.")]
        [SerializeField] private AudioMixer audioMixer;

        [Tooltip("AudioMixerGroup del canal Music. Asignar el grupo, no el mixer.")]
        [SerializeField] private AudioMixerGroup musicGroup;

        [Tooltip("AudioMixerGroup del canal SFX.")]
        [SerializeField] private AudioMixerGroup sfxGroup;

        // ── AudioSources ───────────────────────────────────────────────
        [Header("AudioSources (creados automáticamente)")]
        [Tooltip("Source dedicado a la música de fondo. Se autocrea si está null.")]
        [SerializeField] private AudioSource musicSource;

        [Tooltip("Source dedicado a SFX puntuales. Se autocrea si está null.")]
        [SerializeField] private AudioSource sfxSource;

        // ── Clips ──────────────────────────────────────────────────────
        [Header("Música")]
        [Tooltip("Música del menú principal. Opcional.")]
        [SerializeField] private AudioClip menuMusic;

        [Tooltip("Música del gameplay. Opcional.")]
        [SerializeField] private AudioClip gameplayMusic;

        [Header("SFX del jugador")]
        [SerializeField] private AudioClip playerShootClip;
        [SerializeField] private AudioClip playerHitClip;

        [Header("SFX de enemigos")]
        [SerializeField] private AudioClip enemyShootClip;
        [SerializeField] private AudioClip enemyExplosionClip;

        [Header("SFX de UI / sistema")]
        [SerializeField] private AudioClip uiClickClip;
        [SerializeField] private AudioClip waveStartClip;
        [SerializeField] private AudioClip gameOverClip;

        // ── Configuración inicial ──────────────────────────────────────
        [Header("Volúmenes por defecto (si no hay preferencias guardadas)")]
        [SerializeField] private GameSettings gameSettings;

        // ── Constantes de PlayerPrefs ──────────────────────────────────
        private const string MasterVolumeKey = "StellarDefense.Audio.Master";
        private const string MusicVolumeKey = "StellarDefense.Audio.Music";
        private const string SFXVolumeKey = "StellarDefense.Audio.SFX";

        // ── Nombres de parámetros expuestos en el Mixer ────────────────
        // IMPORTANTE: deben coincidir EXACTAMENTE con los nombres expuestos en el AudioMixer.
        private const string MasterParam = "MasterVolume";
        private const string MusicParam = "MusicVolume";
        private const string SFXParam = "SFXVolume";

        // Mínimo absoluto del slider para evitar Log10(0) = -infinity.
        private const float MinSliderValue = 0.0001f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (transform.parent != null) transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            EnsureAudioSources();
            LoadAndApplyVolumes();

            // Reproducir música según el estado actual al inicializar.
            // Necesario porque OnStateChanged puede haberse disparado antes
            // de que este AudioManager existiera.
            StartCoroutine(PlayMusicOnStart());
        }

        private System.Collections.IEnumerator PlayMusicOnStart()
        {
            yield return null;

            if (GameManager.Instance == null) yield break;

            switch (GameManager.Instance.CurrentState)
            {
                case GameManager.GameState.MainMenu:
                    PlayMusic(menuMusic);
                    break;
                case GameManager.GameState.Playing:
                    PlayMusic(gameplayMusic);
                    break;
            }
        }

        private void OnEnable()
        {
            // Suscripción a eventos del juego. Aquí concentramos TODA la lógica de
            // qué SFX suena en respuesta a qué evento. Si en el futuro queremos
            // añadir o quitar sonidos, este es el único sitio que hay que tocar.
            Enemy.OnAnyEnemyDestroyed += HandleEnemyDestroyed;

            // Suscribirse a cambios de estado del juego para cambiar música.
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            Enemy.OnAnyEnemyDestroyed -= HandleEnemyDestroyed;

            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleGameStateChanged;
        }

        private void HandleGameStateChanged(GameManager.GameState newState)
        {
            switch (newState)
            {
                case GameManager.GameState.MainMenu:
                    PlayMusic(menuMusic);
                    break;

                case GameManager.GameState.Playing:
                    PlayMusic(gameplayMusic);
                    break;

                case GameManager.GameState.Paused:
                    // No cambiamos música al pausar, solo se congela con timeScale=0
                    // pero el AudioSource sigue sonando (comportamiento correcto).
                    break;

                case GameManager.GameState.GameOver:
                    // Paramos la música al morir. Opcional: podrías poner música de game over.
                    StopMusic();
                    break;
            }
        }

        // ── Inicialización ─────────────────────────────────────────────

        private void EnsureAudioSources()
        {
            // Si los AudioSources no se asignaron en el Inspector, los creamos aquí.
            // Patrón defensivo: el manager funciona con o sin pre-configuración.
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            if (musicGroup != null) musicSource.outputAudioMixerGroup = musicGroup;

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
            if (sfxGroup != null) sfxSource.outputAudioMixerGroup = sfxGroup;
        }

        private void LoadAndApplyVolumes()
        {
            float defaultMaster = gameSettings != null ? gameSettings.DefaultMasterVolume : 0.8f;
            float defaultMusic  = gameSettings != null ? gameSettings.DefaultMusicVolume  : 0.6f;
            float defaultSfx    = gameSettings != null ? gameSettings.DefaultSfxVolume    : 0.8f;

            // Si no hay preferencias guardadas, usamos defaults.
            // Si las hay pero son menores que 0.01 (casi mute), ignoramos y usamos defaults.
            float master = PlayerPrefs.GetFloat(MasterVolumeKey, defaultMaster);
            float music  = PlayerPrefs.GetFloat(MusicVolumeKey,  defaultMusic);
            float sfx    = PlayerPrefs.GetFloat(SFXVolumeKey,    defaultSfx);

            // Protección anti-mute accidental.
            if (master < 0.01f) master = defaultMaster;
            if (music  < 0.01f) music  = defaultMusic;
            if (sfx    < 0.01f) sfx    = defaultSfx;

            SetMasterVolume(master);
            SetMusicVolume(music);
            SetSFXVolume(sfx);
        }

        // ── API pública: reproducir audio ──────────────────────────────

        /// <summary>Reproduce un SFX one-shot. Soporta múltiples sonidos simultáneos.</summary>
        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || sfxSource == null) return;
            // PlayOneShot permite reproducir varios SFX al mismo tiempo desde el mismo source.
            // Es la forma correcta para efectos puntuales.
            sfxSource.PlayOneShot(clip);
        }

        /// <summary>Reproduce música. Si ya hay una sonando con el mismo clip, no la corta.</summary>
        public void PlayMusic(AudioClip clip)
        {
            if (clip == null || musicSource == null) return;
            if (musicSource.clip == clip && musicSource.isPlaying) return;

            musicSource.clip = clip;
            musicSource.Play();
        }

        public void StopMusic()
        {
            if (musicSource != null) musicSource.Stop();
        }

        // ── API pública: control de volúmenes ──────────────────────────

        public void SetMasterVolume(float linearValue) => ApplyVolume(MasterParam, MasterVolumeKey, linearValue);
        public void SetMusicVolume(float linearValue) => ApplyVolume(MusicParam, MusicVolumeKey, linearValue);
        public void SetSFXVolume(float linearValue) => ApplyVolume(SFXParam, SFXVolumeKey, linearValue);

        private void ApplyVolume(string mixerParam, string prefsKey, float linearValue)
        {
            if (audioMixer == null)
            {
                Debug.LogError("AudioMixer es NULL");
                return;
            }

            linearValue = Mathf.Clamp(linearValue, MinSliderValue, 1f);
            float volumeDb = Mathf.Log10(linearValue) * 20f;

            audioMixer.SetFloat(mixerParam, volumeDb);

            PlayerPrefs.SetFloat(prefsKey, linearValue);
            PlayerPrefs.Save();
        }

        // ── Helpers para que UI y otros sistemas lean valores actuales ─

        public float GetMasterVolume() => PlayerPrefs.GetFloat(MasterVolumeKey, 0.8f);
        public float GetMusicVolume() => PlayerPrefs.GetFloat(MusicVolumeKey, 0.6f);
        public float GetSFXVolume() => PlayerPrefs.GetFloat(SFXVolumeKey, 0.8f);
        public AudioClip GetMenuMusic() => menuMusic;
        public AudioClip GetGameplayMusic() => gameplayMusic;

        // ── Conexión con eventos del juego ─────────────────────────────

        private void HandleEnemyDestroyed(int pointsValue)
        {
            // No usamos los puntos para nada en audio, pero el evento los pasa.
            PlaySFX(enemyExplosionClip);
        }

        // ── Métodos públicos disparados externamente (PlayerController los llamará) ─

        /// <summary>Llamar desde PlayerController al disparar.</summary>
        public void OnPlayerShoot() => PlaySFX(playerShootClip);

        /// <summary>Llamar desde PlayerController al recibir daño.</summary>
        public void OnPlayerHit() => PlaySFX(playerHitClip);

        /// <summary>Llamar desde Enemy al disparar (si quieres ese SFX).</summary>
        public void OnEnemyShoot() => PlaySFX(enemyShootClip);

        /// <summary>Llamar desde WaveManager al iniciar wave.</summary>
        public void OnWaveStart() => PlaySFX(waveStartClip);

        /// <summary>Llamar al perder.</summary>
        public void OnGameOver() => PlaySFX(gameOverClip);

        /// <summary>Llamar desde botones de UI.</summary>
        public void OnUIClick() => PlaySFX(uiClickClip);

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}