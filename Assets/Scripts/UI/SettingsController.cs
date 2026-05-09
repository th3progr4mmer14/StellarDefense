using UnityEngine;
using UnityEngine.UI;
using StellarDefense.Managers;

namespace StellarDefense.UI
{
    /// <summary>
    /// Controlador del panel de Ajustes. Modifica volúmenes en tiempo real
    /// mediante AudioManager y persiste cambios automáticamente.
    /// Se puede reutilizar tanto desde MainMenu como desde el Pause Menu.
    /// </summary>
    public sealed class SettingsController : MonoBehaviour
    {
        [Header("Sliders de volumen")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        [Header("Panel raíz")]
        [Tooltip("Panel completo del Settings. Necesario para el botón Cerrar.")]
        [SerializeField] private GameObject settingsPanel;

        private void OnEnable()
        {
            if (AudioManager.Instance == null) return;

            // Desconectamos temporalmente los callbacks para que al
            // inicializar los sliders NO disparen OnValueChanged
            // (que sobreescribirían el volumen con el valor inicial del slider).
            if (masterSlider != null)
            {
                masterSlider.onValueChanged.RemoveListener(OnMasterChanged);
                masterSlider.value = AudioManager.Instance.GetMasterVolume();
                masterSlider.onValueChanged.AddListener(OnMasterChanged);
            }

            if (musicSlider != null)
            {
                musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
                musicSlider.value = AudioManager.Instance.GetMusicVolume();
                musicSlider.onValueChanged.AddListener(OnMusicChanged);
            }

            if (sfxSlider != null)
            {
                sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
                sfxSlider.value = AudioManager.Instance.GetSFXVolume();
                sfxSlider.onValueChanged.AddListener(OnSFXChanged);
            }
        }

        // ── Callbacks de los sliders ───────────────────────────────────

        public void OnMasterChanged(float value)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetMasterVolume(value);
        }

        public void OnMusicChanged(float value)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetMusicVolume(value);
        }

        public void OnSFXChanged(float value)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetSFXVolume(value);
        }

        // ── Botón cerrar ───────────────────────────────────────────────

        public void OnCloseButton()
        {
            if (AudioManager.Instance != null) AudioManager.Instance.OnUIClick();
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }
    }
}