using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionUI : MonoBehaviour
{
    public CharacterLoader characterLoader; // Odniesienie do skryptu CharacterLoader
    public int maxSelection = 4; // Maksymalna liczba postaci do wybrania
    public Transform characterPanel; // Panel, w którym bêd¹ generowane przyciski postaci
    public GameObject characterButtonPrefab; // Prefab przycisku dla ka¿dej postaci
    public Button startBattleButton; // Przycisk "Start Battle"

    public Transform selectedCharacterPanel; // Panel, na którym bêd¹ wyœwietlane wybrane postacie
    public GameObject selectedCharacterButtonPrefab; // Prefab przycisku dla wybranej postaci

void Start()
{
    if (characterLoader == null)
    {
        Debug.LogError("CharacterLoader is not assigned!");
        return; // Zatrzymaj dalsze wykonanie
    }

    if (characterLoader.characters == null || characterLoader.characters.Count == 0)
    {
        Debug.LogError("No characters loaded in CharacterLoader!");
        return; // Zatrzymaj dalsze wykonanie
    }

    CreateCharacterButtons();
    UpdateSelectedCharacterButtons(); // Dodaj to wywo³anie
    startBattleButton.interactable = false;
}

    void CreateCharacterButtons()
    {
        if (characterPanel == null)
        {
            Debug.LogError("CharacterPanel is not assigned!");
            return;
        }

        if (characterButtonPrefab == null)
        {
            Debug.LogError("CharacterButtonPrefab is not assigned!");
            return;
        }

        // Dla ka¿dej za³adowanej postaci twórz przycisk
        foreach (Character character in characterLoader.characters)
        {
            GameObject buttonObj = Instantiate(characterButtonPrefab, characterPanel);
            Button button = buttonObj.GetComponent<Button>();

            if (button == null)
            {
                Debug.LogError("Button component is missing on the prefab!");
                continue;
            }

            // SprawdŸ, czy prefab zawiera TextMeshProUGUI
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText == null)
            {
                Debug.LogError("TextMeshProUGUI component is missing on the prefab!");
                continue;
            }

            // Ustaw nazwê postaci na przycisku
            buttonText.text = character.name;

            // Dodaj listener dla przycisku, aby dodaæ postaæ po klikniêciu
            button.onClick.AddListener(() => SelectCharacter(character));
        }
    }


    public void SelectCharacter(Character character)
    {
        if (CharacterManager.Instance == null)
        {
            Debug.LogError("CharacterManager.Instance is null!");
            return;
        }

        if (CharacterManager.Instance.selectedCharacters == null)
        {
            Debug.LogError("selectedCharacters list is null!");
            return;
        }

        if (startBattleButton == null)
        {
            Debug.LogError("StartBattleButton is not assigned!");
            return;
        }

        // SprawdŸ, czy nie wybrano ju¿ maksymalnej liczby postaci
        if (CharacterManager.Instance.selectedCharacters.Count < maxSelection)
        {
            CharacterManager.Instance.AddCharacter(character);
            Debug.Log(character.name + " has been selected!");

            // Zaktualizuj przyciski wybranych postaci
            UpdateSelectedCharacterButtons();

            // SprawdŸ, czy mo¿na ju¿ rozpocz¹æ walkê
            if (CharacterManager.Instance.selectedCharacters.Count == maxSelection)
            {
                startBattleButton.interactable = true;
            }
        }
        else
        {
            Debug.Log("You have already selected the maximum number of characters.");
        }
    }


    void UpdateSelectedCharacterButtons()
    {
        if (selectedCharacterPanel == null)
        {
            Debug.LogError("SelectedCharacterPanel is not assigned!");
            return;
        }

        if (selectedCharacterButtonPrefab == null)
        {
            Debug.LogError("SelectedCharacterButtonPrefab is not assigned!");
            return;
        }

        // Usuñ wszystkie istniej¹ce przyciski z panelu
        foreach (Transform child in selectedCharacterPanel)
        {
            Destroy(child.gameObject);
        }

        // Dla ka¿dej wybranej postaci twórz przycisk
        foreach (Character character in CharacterManager.Instance.selectedCharacters)
        {
            GameObject buttonObj = Instantiate(selectedCharacterButtonPrefab, selectedCharacterPanel);
            Button button = buttonObj.GetComponent<Button>();

            if (button == null)
            {
                Debug.LogError("Button component is missing on the prefab!");
                continue;
            }

            // SprawdŸ, czy prefab zawiera TextMeshProUGUI
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText == null)
            {
                Debug.LogError("TextMeshProUGUI component is missing on the prefab!");
                continue;
            }

            // Ustaw nazwê postaci na przycisku
            buttonText.text = character.name;

            // Dodaj listener dla przycisku (opcjonalnie: mo¿esz dodaæ akcje przycisków dla wybranych postaci)
        }
    }


    public void StartBattle()
    {
        // SprawdŸ, czy wybrano wymagan¹ liczbê postaci
        if (CharacterManager.Instance.selectedCharacters.Count == maxSelection)
        {
            SceneManager.LoadScene("BattleScene"); // PrzejdŸ do sceny walki
        }
        else
        {
            Debug.Log("You must select " + maxSelection + " characters to start the battle.");
        }
    }
}
