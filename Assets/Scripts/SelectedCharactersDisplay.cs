using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectedCharactersDisplay : MonoBehaviour
{
    public TextMeshProUGUI selectedCharactersText;  // Tekst na UI do wyœwietlania postaci

    void Start()
    {
        UpdateSelectedCharactersText();
    }

    public void UpdateSelectedCharactersText()
    {
        // SprawdŸ, czy `CharacterManager` jest dostêpny
        if (CharacterManager.Instance == null || CharacterManager.Instance.selectedCharacters.Count == 0)
        {
            selectedCharactersText.text = "No characters selected.";
            return;
        }

        // Pobierz wybrane postacie
        List<Character> selectedCharacters = CharacterManager.Instance.selectedCharacters;

        // Tworzymy string do wyœwietlenia postaci
        string charactersToDisplay = "Selected characters:\n";
        foreach (Character character in selectedCharacters)
        {
            charactersToDisplay += character.name + "\n";
        }

        // Aktualizacja tekstu na UI
        selectedCharactersText.text = charactersToDisplay;
    }
    // Dodaj funkcjê do odœwie¿enia tekstu po wyborze postaci
    public void OnCharacterSelected()
    {
        UpdateSelectedCharactersText();
    }
}
