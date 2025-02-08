using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // Added namespace for UI components

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; } // Singleton

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public TextMeshProUGUI turnNumberText;
    public TextMeshProUGUI turnOrderText; // UI element for displaying turn order
    public GameObject actionMenuPanel; // New panel for action buttons
    public GameObject actionButtonPrefab; // Prefab for action buttons
    public int numberOfEnemies = 4;

    protected List<Character> playerTeam = new List<Character>();
    public List<Character> enemyTeam = new List<Character>();
    protected List<Character> allCharacters = new List<Character>();

    protected List<Character> turnOrder = new List<Character>();
    private int currentTurnIndex = 0;
    private int turnNumber = 1;

    public CharacterLoader characterLoader;
    public CharacterStatsUI characterStatsUI;

    public CombatManager combatManager;


    void Start()
    {
        if (combatManager == null)
        {
            combatManager = FindObjectOfType<CombatManager>();
            if (combatManager == null)
            {
                Debug.LogError("CombatManager not found in the scene!");
            }
        }
        if (turnNumberText == null || turnOrderText == null || actionMenuPanel == null || actionButtonPrefab == null)
        {
            Debug.LogError("TurnNumberText, TurnOrderText, ActionMenuPanel, or ActionButtonPrefab is not assigned!");
            return;
        }
        StartCoroutine(WaitForCharacterLoader());
    }
    IEnumerator WaitForCharacterLoader()
    {
        yield return new WaitUntil(() => characterLoader.characters != null && characterLoader.characters.Count > 0);

        allCharacters = characterLoader.characters;
        Debug.Log("All characters:");
        foreach (Character character in allCharacters)
        {
            Debug.Log($"Character: {character.name} (Health: {character.health}, Speed: {character.speed})");
        }

        playerTeam = CharacterManager.Instance.selectedCharacters;

        SelectRandomEnemies();
        StartCoroutine(WaitForEnemies());
    }
    IEnumerator WaitForEnemies()
    {
        yield return new WaitUntil(() => enemyTeam != null && enemyTeam.Count > 0);

        turnOrder.AddRange(playerTeam);
        turnOrder.AddRange(enemyTeam);

        turnOrder.Sort((a, b) => b.speed.CompareTo(a.speed));

        LogTeams();
        UpdateTurnOrderText(); // Update the turn order display

        UpdateTurnNumberText();
        StartTurn();
    }

    void SelectRandomEnemies()
    {
        List<Character> availableEnemies = new List<Character>(allCharacters);

        foreach (Character selected in playerTeam)
        {
            availableEnemies.Remove(selected);
        }

        while (enemyTeam.Count < numberOfEnemies && availableEnemies.Count > 0)
        {
            int randomIndex = Random.Range(0, availableEnemies.Count);
            Character selectedEnemy = availableEnemies[randomIndex];

            selectedEnemy.isEnemy = true; // Oznacz jako wroga
            selectedEnemy.name = "(Enemy) " + selectedEnemy.name;

            enemyTeam.Add(selectedEnemy);
            availableEnemies.RemoveAt(randomIndex);
        }
    }


    void StartTurn()
    {
        if (turnOrder.Count > 0 && currentTurnIndex < turnOrder.Count)
        {
            Character currentCharacter = turnOrder[currentTurnIndex];

            if (currentCharacter.IsAlive())
            {
                Debug.Log("It's " + currentCharacter.name + "'s turn!");
                characterStatsUI.ShowCharacterStats(currentCharacter);
                ShowActionMenu(currentCharacter); // Show action menu for the current character
            }
            else
            {
                EndTurn(); // If the character is dead, immediately end the turn
            }
        }
    }

    public void EndTurn()
    {
        currentTurnIndex++;

        if (currentTurnIndex >= turnOrder.Count)
        {
            currentTurnIndex = 0;
            turnNumber++;
            UpdateTurnNumberText();
        }
        characterStatsUI.HideCharacterStats();
        HideActionMenu(); // Hide action menu when the turn ends

        UpdateTurnOrderText(); // Update the turn order display
        StartTurn();
    }

    void ShowActionMenu(Character character)
    {
        if (actionMenuPanel == null || actionButtonPrefab == null)
        {
            Debug.LogError("ActionMenuPanel or ActionButtonPrefab is not assigned!");
            return;
        }

        if (character.skills == null || character.skills.Count == 0)
        {
            Debug.LogError("Character skills are not initialized or empty!");
            return;
        }

        actionMenuPanel.SetActive(true);
        // Clear existing buttons
        foreach (Transform child in actionMenuPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Skill skill in character.skills)
        {
            GameObject button = Instantiate(actionButtonPrefab, actionMenuPanel.transform);
            if (button == null)
            {
                Debug.LogError("Failed to instantiate action button prefab!");
                continue;
            }

            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText == null)
            {
                // Add the TextMeshProUGUI component programmatically
                buttonText = button.AddComponent<TextMeshProUGUI>();
            }

            Button buttonComponent = button.GetComponent<Button>();
            if (buttonComponent == null)
            {
                Debug.LogError("Button component is missing on the prefab!");
                continue;
            }

            buttonText.text = skill.name;
            buttonComponent.onClick.AddListener(() => OnActionButtonClicked(character, skill));
        }
    }

    void HideActionMenu()
    {
        actionMenuPanel.SetActive(false);
    }

    void OnActionButtonClicked(Character character, Skill skill)
    {
        Debug.Log($"{character.name} chose to use {skill.name}!");

        // Przekazanie do CombatManagera
        combatManager.ExecuteAction(character, skill);

        // Ukryj menu i zakończ turę
        HideActionMenu();
        EndTurn();
    }


    void UpdateTurnOrderText()
    {
        string turnOrderDisplay = "Turn Order:\n";
        foreach (Character character in turnOrder)
        {
            turnOrderDisplay += $"SPD: {character.speed} {character.name} (HP: {character.health})\n";
        }
        turnOrderText.text = turnOrderDisplay; // Update the UI text
    }

    Character GetFirstAliveCharacter(List<Character> team)
    {
        foreach (Character member in team)
        {
            if (member.IsAlive())
            {
                return member;
            }
        }
        return null;
    }

    void UpdateTurnNumberText()
    {
        turnNumberText.text = "Turn: " + turnNumber;
    }

    void LogTeams()
    {
        Debug.Log("Player Team:");
        foreach (Character player in playerTeam)
        {
            Debug.Log($"- {player.name} (Health: {player.health}, Speed: {player.speed})");
        }

        Debug.Log("Enemy Team:");
        foreach (Character enemy in enemyTeam)
        {
            Debug.Log($"- {enemy.name} (Health: {enemy.health}, Speed: {enemy.speed})");
        }

        Debug.Log("Turn Order:");
        foreach (Character character in turnOrder)
        {
            Debug.Log($"- {character.name} (Speed: {character.speed})");
        }
    }

    void AddTestCharacters()
    {
        playerTeam.Add(new Character()
        {
            name = "Test Warrior",
            health = 1000,
            attack = 25,
            defense = 10,
            speed = 50,
            characterPrefab = null
        });
    }

}
