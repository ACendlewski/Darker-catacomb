using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public TMP_Text volumeText;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;

    // Metoda wywo³ywana przy zmianie suwaka g³oœnoœci
    public void OnVolumeChanged()
    {
        float volumeValue = volumeSlider.value;
        Debug.Log("Volume changed to: " + volumeValue);
        volumeText.text = "Volume: " + volumeValue.ToString("F2");
    }

    // Metoda wywo³ywana przy zmianie ustawienia pe³nego ekranu
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log(isFullscreen);
        // Umieœæ tutaj kod obs³ugi zmiany ustawienia pe³nego ekranu
    }
}
