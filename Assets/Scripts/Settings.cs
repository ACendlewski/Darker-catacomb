using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public TMP_Text volumeText;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;

    // Metoda wywo�ywana przy zmianie suwaka g�o�no�ci
    public void OnVolumeChanged()
    {
        float volumeValue = volumeSlider.value;
        Debug.Log("Volume changed to: " + volumeValue);
        volumeText.text = "Volume: " + volumeValue.ToString("F2");
    }

    // Metoda wywo�ywana przy zmianie ustawienia pe�nego ekranu
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log(isFullscreen);
        // Umie�� tutaj kod obs�ugi zmiany ustawienia pe�nego ekranu
    }
}
