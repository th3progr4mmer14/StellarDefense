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
            // Cuando el panel se activa, inicializamos los sliders con los
            // valores actuales del AudioManager. Así el usuario siempre ve
            // el estado real del audio, no un valor hardcodeado.
            if (AudioManager.Instance == null) return;

            if (masterSlider != null) masterSlider.value = AudioManager.Instance.GetMasterVolume();
            if (musicSlider != null) musicSlider.value = AudioManager.Instance.GetMusicVolume();
            if (sfxSlider != null) sfxSlider.value = AudioManager.Instance.GetSFXVolume();
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