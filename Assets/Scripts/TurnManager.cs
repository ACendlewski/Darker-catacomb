using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // Added namespace for UI components
using UnityEngine.EventSystems;
using System.Linq;


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

    public TextMeshProUGUI skillsText; // Text for displaying skill details
    private Skill selectedSkill = null;


    public int numberOfEnemies = 4;

    public List<Character> playerTeam = new List<Character>();
    public List<Character> enemyTeam = new List<Character>();
    protected List<Character> allCharacters = new List<Character>();
    public List<Character> allEnemies = new List<Character>();

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
        Debug.Log("Waiting for character loader...");
    }
    IEnumerator WaitForCharacterLoader()
    {
        yield return new WaitUntil(() => characterLoader.characters != null && characterLoader.characters.Count > 0);

        allCharacters = characterLoader.characters;
        allEnemies = characterLoader.enemies.Cast<Character>().ToList();

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

        characterStatsUI.SpawnAllCharacters(playerTeam, enemyTeam); // ✅ Spawn na start

        LogTeams();
        UpdateTurnOrderText();
        UpdateTurnNumberText();
        StartTurn();
    }


    void SelectRandomEnemies()
    {
        List<Character> availableEnemies = new List<Character>(allEnemies); // ✅ Wybieramy tylko z wrogów

        while (enemyTeam.Count < numberOfEnemies && availableEnemies.Count > 0)
        {
            int randomIndex = Random.Range(0, availableEnemies.Count);
            Character selectedEnemy = availableEnemies[randomIndex];

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

                currentCharacter.PlayAnimation("idle"); // Postać przechodzi do idle

                if (currentCharacter.isEnemy)
                {
                    StartCoroutine(EnemyTurn(currentCharacter)); // Wróg atakuje automatycznie
                }
                else
                {
                    ShowActionMenu(currentCharacter); // Gracz wybiera atak ręcznie
                }
            }
            else
            {
                EndTurn();
            }
        }
    }


    IEnumerator EnemyTurn(Character enemy)
    {
        yield return new WaitForSeconds(1.0f); // Krótkie opóźnienie dla naturalnego przebiegu walki

        if (enemy.skills.Count > 0)
        {
            Skill randomSkill = enemy.skills[Random.Range(0, enemy.skills.Count)];
            Character target = SelectRandomTarget(playerTeam); // Wybór losowego celu

            if (target != null)
            {
                Debug.Log($"{enemy.name} używa {randomSkill.name} na {target.name}!");
                combatManager.ExecuteAction(enemy, randomSkill);
            }
        }

        yield return new WaitForSeconds(0.5f); // Krótkie opóźnienie przed zakończeniem tury
        EndTurn();
    }

    Character SelectRandomTarget(List<Character> targets)
    {
        List<Character> aliveTargets = targets.FindAll(t => t.IsAlive());

        if (aliveTargets.Count > 0)
        {
            return aliveTargets[Random.Range(0, aliveTargets.Count)];
        }

        return null;
    }

    public void EndTurn()
    {
        if (IsGameOver())
        {
            ShowGameOverScreen();
            return;
        }

        currentTurnIndex++;

        if (currentTurnIndex >= turnOrder.Count)
        {
            currentTurnIndex = 0;
            turnNumber++;
            UpdateTurnNumberText();
        }

        characterStatsUI.HideCharacterStats();
        HideActionMenu();

        UpdateTurnOrderText();
        StartTurn();
    }
    private bool IsGameOver()
    {
        bool allPlayersDead = playerTeam.TrueForAll(character => !character.IsAlive());
        bool allEnemiesDead = enemyTeam.TrueForAll(character => !character.IsAlive());

        return allPlayersDead || allEnemiesDead;
    }
    private void ShowGameOverScreen()
    {
        string winner = playerTeam.Exists(character => character.IsAlive()) ? "Gracze wygrali!" : "Wrogowie wygrali!";
        Debug.Log("KONIEC GRY: " + winner);

        turnNumberText.text = "KONIEC GRY";
        turnOrderText.text = winner;

        foreach (Character character in playerTeam.Concat(enemyTeam))
        {
            if (character.IsAlive())
            {
                character.PlayAnimation("victory");
            }
        }

        StopAllCoroutines();
        enabled = false;
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

        // **Sprawdzamy czy postać jest wrogiem i wyszarzamy panel**
        CanvasGroup canvasGroup = actionMenuPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = actionMenuPanel.AddComponent<CanvasGroup>();
        }

        if (character.isEnemy)
        {
            canvasGroup.alpha = 0.5f;  // Zmniejszenie widoczności
            canvasGroup.interactable = false; // Wyłączenie interakcji
            canvasGroup.blocksRaycasts = false; // Blokowanie kliknięć
        }
        else
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        // Usunięcie istniejących przycisków przed dodaniem nowych
        foreach (Transform child in actionMenuPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Skill skill in character.skills)
        {
            GameObject skillButton = Instantiate(actionButtonPrefab, actionMenuPanel.transform);
            skillButton.name = skill.name;

            Button button = skillButton.GetComponent<Button>();

            TextMeshProUGUI buttonText = skillButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = skill.name;
            }

            EventTrigger trigger = skillButton.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = skillButton.AddComponent<EventTrigger>();
            }
            trigger.triggers.Clear();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener((data) => { ShowSkillDescription(skill); });
            trigger.triggers.Add(entryEnter);

            button.onClick.AddListener(() => OnActionButtonClicked(character, skill));

            // **Dodatkowe wyłączenie przycisków, jeśli to wróg**
            button.interactable = !character.isEnemy;
        }
    }


    void ShowSkillDescription(Skill skill)
    {
        if (skillsText != null)
        {
            skillsText.text = $"DMG: {skill.damage - skill.damageModifier} - {skill.damageModifier + skill.damage}\n" +
                              $"Hit: {skill.hitChance}%\n" +
                              $"Crit: {skill.critChance}%\n";
        }
    }

    void HideSkillDescription()
    {
        if (selectedSkill == null && skillsText != null)
        {
            skillsText.text = "";
        }
    }


    void OnActionButtonClicked(Character character, Skill skill)
    {
        Debug.Log($"{character.name} używa skilla {skill.name}!");

        selectedSkill = skill; // Zapamiętaj wybrany skill

        // Przekazanie akcji do CombatManagera
        combatManager.ExecuteAction(character, skill);

        // Ukrycie menu i zakończenie tury
        HideActionMenu();
        EndTurn();
    }


    void HideActionMenu()
    {
        actionMenuPanel.SetActive(false);
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
