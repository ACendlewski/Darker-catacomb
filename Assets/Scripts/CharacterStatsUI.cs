using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterStatsUI : MonoBehaviour
{
    public TextMeshProUGUI characterNameText;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI speedText;

    public GameObject statsPanel;

    public Transform[] playerPositions; // Pozycje startowe dla drużyny gracza
    public Transform[] enemyPositions;  // Pozycje startowe dla przeciwników

    private Dictionary<string, GameObject> spawnedCharacters = new Dictionary<string, GameObject>(); // Przechowuje już stworzone postacie

    public void ShowCharacterStats(Character character)
    {
        if (statsPanel == null || character == null) return;
        statsPanel.SetActive(true);

        characterNameText.text = character.name;

        healthSlider.maxValue = character.maxHealth;
        healthSlider.value = character.health;
        healthText.text = $"{character.health} / {character.maxHealth}";

        attackText.text = "ATT: " + character.attack;
        defenseText.text = "DEF: " + character.defense;
        speedText.text = "SPD: " + character.speed;

        if (!spawnedCharacters.ContainsKey(character.name))
        {
            SpawnCharacter(character);
        }
        else
        {
            Debug.Log($"Character {character.name} already instantiated.");
        }
    }

    private void SpawnCharacter(Character character)
    {
        string prefabName = character.name.Replace("(Enemy) ", "").Trim();
        GameObject characterPrefab = Resources.Load<GameObject>("Prefabs/" + prefabName);

        if (characterPrefab == null)
        {
            Debug.LogError("Character prefab not found: " + prefabName);
            return;
        }

        Transform spawnPoint = GetSpawnPoint(character);
        if (spawnPoint == null)
        {
            Debug.LogError("No spawn position available for " + character.name);
            return;
        }

        // Ustalamy rotację – wrogowie domyślnie stoją przodem (Quaternion.identity)
        Quaternion rotation = character.isEnemy ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

        Debug.Log($"Instantiating character: {character.name}");
        GameObject characterObject = Instantiate(characterPrefab, spawnPoint.position, rotation);
        characterObject.name = character.name;

        spawnedCharacters[character.name] = characterObject; // Dodajemy do słownika
    }


    private Transform GetSpawnPoint(Character character)
    {
        if (character.isEnemy)
        {
            int index = TurnManager.Instance.enemyTeam.IndexOf(character);
            if (index >= 0 && index < enemyPositions.Length)
            {
                return enemyPositions[index];
            }
        }
        else
        {
            int index = TurnManager.Instance.playerTeam.IndexOf(character);
            if (index >= 0 && index < playerPositions.Length)
            {
                return playerPositions[index];
            }
        }
        return null;
    }

    public void HideCharacterStats()
    {
        if (statsPanel != null)
        {
            statsPanel.SetActive(false);
        }
    }

    public void DisplayActionFeedback(string message)
    {
        Debug.Log(message);
    }
}
