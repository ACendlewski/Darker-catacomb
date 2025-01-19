using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // Added for UI components

public class SelectedCharactersDisplay : MonoBehaviour
{
    public TextMeshProUGUI selectedCharactersText;  // Text for displaying selected characters

    void Start()
    {
        UpdateSelectedCharactersText();
    }

    public void UpdateSelectedCharactersText()
    {
        // Check if `CharacterManager` is available
        if (CharacterManager.Instance == null || CharacterManager.Instance.selectedCharacters.Count == 0)
        {
            selectedCharactersText.text = "No characters selected.";
            return;
        }

        // Get selected characters
        List<Character> selectedCharacters = CharacterManager.Instance.selectedCharacters;

        // Create string to display characters
        string charactersToDisplay = "Selected characters:\n";
        foreach (Character character in selectedCharacters)
        {
            charactersToDisplay += character.name + "\n";
        }

        // Update the text on the UI
        selectedCharactersText.text = charactersToDisplay;
    }

    // Add method to refresh text after character selection
    public void OnCharacterSelected()
    {
        UpdateSelectedCharactersText();
    }
}
