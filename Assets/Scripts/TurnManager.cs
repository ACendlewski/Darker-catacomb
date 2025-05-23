using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

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

    [Header("UI References")]
    public TextMeshProUGUI turnNumberText;
    public TextMeshProUGUI turnOrderText;
    public GameObject actionMenuPanel;
    public GameObject actionButtonPrefab;
    public TextMeshProUGUI skillsText;

    [Header("Game Settings")]
    public int numberOfEnemies = 4;

    [Header("Character Lists")]
    public List<Character> playerTeam = new List<Character>();
    public List<Character> enemyTeam = new List<Character>();
    protected List<Character> allCharacters = new List<Character>();
    public List<Character> allEnemies = new List<Character>();
    protected List<Character> turnOrder = new List<Character>();

    [Header("References")]
    public CharacterLoader characterLoader;
    public CharacterStatsUI characterStatsUI;
    public CombatManager combatManager;

    private int currentTurnIndex = 0;
    private int turnNumber = 1;
    private Skill selectedSkill = null;
    private Character selectedEnemy = null;
    private bool isPlayerTurnActive = false; // Track if player turn is in progress

    [Header("End Game UI")]
    public Button endGameButton;


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
            Debug.LogError("UI elements are not assigned!");
            return;
        }

        endGameButton.gameObject.SetActive(false);
        StartCoroutine(WaitForCharacterLoader());
    }

    IEnumerator WaitForCharacterLoader()
    {
        yield return new WaitUntil(() => characterLoader.characters != null &&
                                     characterLoader.characters.Count > 0 &&
                                     characterLoader.enemies != null &&
                                     characterLoader.enemies.Count > 0);

        allCharacters = characterLoader.characters;
        allEnemies = new List<Character>(characterLoader.enemies);
        playerTeam = CharacterManager.Instance.selectedCharacters;

        SelectRandomEnemies();
        StartCoroutine(WaitForEnemies());
        hasAppliedDifficultyMultiplier = false; // Initialize the flag

    }

    private bool hasAppliedDifficultyMultiplier = false; // Flag to check if multiplier has been applied

    IEnumerator WaitForEnemies()
    {
        yield return new WaitUntil(() => enemyTeam != null && enemyTeam.Count > 0);

        turnOrder.AddRange(playerTeam);
        turnOrder.AddRange(enemyTeam);
        turnOrder.Sort((a, b) => b.speed.CompareTo(a.speed));

        characterStatsUI.SpawnAllCharacters(playerTeam, enemyTeam);
        UpdateTurnOrderText();
        UpdateTurnNumberText();

        Debug.Log("Applying Difficulty Multiplier to enemies..."); // Debug log before applying
        Debug.Log($"Difficulty Multiplier: {GameManager.Instance.DifficultyMultiplier}"); // Log the multiplier
        if (!hasAppliedDifficultyMultiplier) // Check if multiplier has been applied

        {
            foreach (Character enemy in enemyTeam)
            {
                Debug.Log($"Before: {enemy.name} - Health: {enemy.health}, Attack: {enemy.attack}, Defense: {enemy.defense}"); // Log before applying multiplier
                enemy.health = (int)(enemy.health * GameManager.Instance.DifficultyMultiplier); // Apply multiplier to enemy health
                Debug.Log($"After: {enemy.name} - Health: {enemy.health}, Attack: {enemy.attack}, Defense: {enemy.defense}"); // Log after applying multiplier


                enemy.attack = (int)(enemy.attack * GameManager.Instance.DifficultyMultiplier); // Apply multiplier to enemy attack

                enemy.defense = (int)(enemy.defense * GameManager.Instance.DifficultyMultiplier); // Apply multiplier to enemy defense

            }
            hasAppliedDifficultyMultiplier = true; // Set the flag to true after applying
        }
        StartTurn();
    }

    void SelectRandomEnemies()
    {
        List<Character> availableEnemies = new List<Character>(allEnemies);

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
        if (turnOrder.Count == 0) return;

        if (currentTurnIndex >= turnOrder.Count)
        {
            currentTurnIndex = 0;
            turnNumber++;
            UpdateTurnNumberText();
        }

        Character currentCharacter = turnOrder[currentTurnIndex];
        if (!currentCharacter.IsAlive())
        {
            EndTurn();
            return;
        }

        Debug.Log($"It's {currentCharacter.name}'s turn!");
        characterStatsUI.ShowCharacterStats(currentCharacter);

        if (currentCharacter.isEnemy)
        {
            StartCoroutine(EnemyTurn(currentCharacter));
        }
        else
        {
            isPlayerTurnActive = true;
            ShowActionMenu(currentCharacter);
            Debug.Log($"Player {currentCharacter.name}'s turn started - waiting for input");
        }
    }



    IEnumerator EnemyTurn(Character enemy)
    {
        yield return new WaitForSeconds(1.0f);

        if (enemy.skills.Count > 0)
        {
            EnemyAI enemyAI = new EnemyAI(); // Create an instance of EnemyAI
            Character target = enemyAI.FindBestTarget(enemy); // Pass the enemy character to FindBestTarget

            Skill selectedSkill = enemyAI.ChooseBestSkill(target, enemy); // Pass the enemy character to ChooseBestSkill




            if (target != null && target.IsAlive())
            {
                Debug.Log($"{enemy.name} uses {selectedSkill.name} on {target.name}!");
                combatManager.ExecuteAction(enemy, selectedSkill, target);
            }
        }

        yield return new WaitForSeconds(0.5f);
        EndTurn();
    }

    Character SelectRandomTarget(List<Character> targets)
    {
        List<Character> aliveTargets = targets.FindAll(t => t.IsAlive());
        return aliveTargets.Count > 0 ? aliveTargets[Random.Range(0, aliveTargets.Count)] : null;
    }

    public void EndTurn()
    {
        if (isPlayerTurnActive)
        {
            Debug.LogWarning("Cannot end turn - player turn still active");
            return;
        }

        if (IsGameOver())
        {
            ShowGameOverScreen();
            return;
        }

        do
        {
            currentTurnIndex++;
            if (currentTurnIndex >= turnOrder.Count)
            {
                currentTurnIndex = 0;
                turnNumber++;
                UpdateTurnNumberText();
            }
        }
        while (!turnOrder[currentTurnIndex].IsAlive()); // Pominięcie martwych postaci

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
        turnNumberText.text = $"{turnNumberText.text} KONIEC GRY";

        foreach (Character character in playerTeam.Concat(enemyTeam))
        {
            if (character.IsAlive())
            {
                characterStatsUI.PlayAnimation(character, "victory", 999999.9f);
            }
        }

        if (endGameButton != null)
        {
            endGameButton.gameObject.SetActive(true);

        }
        else
        {
            Debug.LogWarning("End Game Button is not assigned!");
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

        selectedEnemy = null;
        selectedSkill = null;
        if (character.skills == null || character.skills.Count == 0)
        {
            Debug.LogError("Character skills are not initialized or empty!");
            return;
        }

        actionMenuPanel.SetActive(true);
        CanvasGroup canvasGroup = actionMenuPanel.GetComponent<CanvasGroup>() ?? actionMenuPanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = character.isEnemy ? 0.5f : 1f;
        canvasGroup.interactable = !character.isEnemy;
        canvasGroup.blocksRaycasts = !character.isEnemy;

        foreach (Transform child in actionMenuPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Skill skill in character.skills)
        {
            GameObject skillButton = Instantiate(actionButtonPrefab, actionMenuPanel.transform);
            skillButton.name = skill.name;

            Button button = skillButton.GetComponent<Button>(); // Ensure button is defined

            button.interactable = !character.isEnemy; // Moved this line to ensure button is defined in the correct scope




            TextMeshProUGUI buttonText = skillButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = skill.name;
            }

            EventTrigger trigger = skillButton.GetComponent<EventTrigger>() ?? skillButton.AddComponent<EventTrigger>();
            trigger.triggers.Clear();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener((data) => ShowSkillDescription(skill));
            trigger.triggers.Add(entryEnter);

            button.onClick.AddListener(() => OnActionButtonClicked(character, skill));
            skillButton.GetComponent<Button>().interactable = !character.isEnemy;
        }
    }

    private void ShowEnemyTargets() // Moved method inside the class

    {
        // Clear previous selections
        foreach (var charUI in characterStatsUI.GetCharacterUIs())
        {
            var highlight = charUI.GetComponent<HighlightEffect>();
            if (highlight != null)
            {
                highlight.SetHighlight(false);
            }
        }

        // Show enemy targets
        foreach (Character enemy in enemyTeam)
        {
            if (enemy.IsAlive())
            {
                var charUI = characterStatsUI.GetCharacterUI(enemy);
                if (charUI != null)
                {
                    var highlight = charUI.GetComponent<HighlightEffect>();
                    if (highlight != null)
                    {
                        highlight.SetHighlight(true);
                    }
                    AssignTargetSelectionTriggers(charUI.gameObject, false);
                }
            }
        }
    }

    private void ShowFriendlyTargets() // Moved method inside the class

    {
        // Clear previous selections
        foreach (var charUI in characterStatsUI.GetCharacterUIs())
        {
            var highlight = charUI.GetComponent<HighlightEffect>();
            if (highlight != null)
            {
                highlight.SetHighlight(false);
            }
        }

        // Show friendly targets
        foreach (Character friendly in playerTeam)
        {
            if (friendly.IsAlive())
            {
                var charUI = characterStatsUI.GetCharacterUI(friendly);
                if (charUI != null)
                {
                    var highlight = charUI.GetComponent<HighlightEffect>();
                    if (highlight != null)
                    {
                        highlight.SetHighlight(true);
                    }
                    AssignTargetSelectionTriggers(charUI.gameObject, true);
                }
            }
        }
    }

    private void PerformAction(Character target) // Moved method inside the class

    {
        if (selectedSkill == null || target == null)
        {
            Debug.LogWarning("Both skill and target must be selected!");
            return;
        }

        StartCoroutine(PerformPlayerAction(turnOrder[currentTurnIndex], selectedSkill, target));
    }

    private void ShowSkillDescription(Skill skill) // Moved method inside the class

    {
        if (skillsText != null)
        {
            skillsText.text = $"{(skill.damage > 0 ? "Dmg: " : "Heal: ")} {skill.damage - skill.damageModifier} - {skill.damage + skill.damageModifier}\n" +
                 $"Hit: {skill.hitChance}%\n" +
                 $"Crit: {skill.critChance}%\n";
        }
    }

    void HideSkillDescription() // Moved method inside the class

    {
        if (selectedSkill == null && skillsText != null)
        {
            skillsText.text = "";
        }
    }

    void OnActionButtonClicked(Character character, Skill skill) // Moved method inside the class

    {
        if (selectedEnemy == null)
        {
            Debug.Log("Please select an enemy first!");
            if (skillsText != null)
            {
                skillsText.text = "Please select an enemy first!";
                skillsText.color = Color.red;
            }
            return;
        }

        Debug.Log($"{character.name} uses {skill.name} on {selectedEnemy.name}!");
        selectedSkill = skill;

        StartCoroutine(PerformPlayerAction(character, skill, selectedEnemy));
        selectedEnemy = null;

        if (skillsText != null)
        {
            skillsText.text = "Select an enemy to attack";
            skillsText.color = Color.white;
        }
    }

    public void SelectTarget(Character target, bool isHealingSkill) // Changed to public
    {
        if (target == null || !target.IsAlive())
        {
            Debug.LogWarning("Selected target is null or dead.");
            return;
        }

        selectedEnemy = target;
        Debug.Log(isHealingSkill ? $"Selected ally: {target.name}" : $"Selected enemy: {target.name}");

        // Update UI with clear enemy selection
        if (skillsText != null)
        {
            skillsText.text = $"Selected: {target.name}\nHP: {target.health}";
            skillsText.color = Color.red;
        }

        // Highlight selected enemy in UI
        var characterUIs = characterStatsUI.GetCharacterUIs();
        foreach (var charUI in characterUIs)
        {
            // Get the character by name from the enemy team
            Character character = enemyTeam.FirstOrDefault(c => c.name == charUI.name);
            if (character != null)
            {
                // Add highlight effect
                var highlight = charUI.GetComponent<HighlightEffect>();
                if (highlight != null)
                {
                    highlight.SetHighlight(character == target);
                }
            }
        }

        UpdateSkillButtonsState();
    }

    public void AssignTargetSelectionTriggers(GameObject targetObject, bool isHealingSkill) // Changed to public
    {
        Button targetButton = targetObject.GetComponent<Button>();
        if (targetButton == null)
        {
            targetButton = targetObject.AddComponent<Button>();
        }

        targetButton.onClick.RemoveAllListeners();
        targetButton.onClick.AddListener(() =>
        {
            Character targetCharacter = targetObject.GetComponent<Character>();
            if (targetCharacter != null)
            {
                SelectTarget(targetCharacter, isHealingSkill);
                if (selectedSkill != null)
                {
                    PerformAction(targetCharacter);
                }
            }
        });

        var image = targetObject.GetComponent<Image>();
        if (image != null)
        {
            // Set different colors for friendly vs enemy targets
            image.color = isHealingSkill ?
                new Color(0.5f, 1, 0.5f, 0.8f) : // Green tint for friendly targets
                new Color(1, 0.5f, 0.5f, 0.8f); // Red tint for enemy targets
        }
    }

    void UpdateSkillButtonsState() // Moved method inside the class

    {
        if (actionMenuPanel != null)
        {
            foreach (Transform child in actionMenuPanel.transform)
            {
                Button button = child.GetComponent<Button>();
                if (button != null)
                {
                    button.interactable = !turnOrder[currentTurnIndex].isEnemy;
                }
            }
        }
    }

    IEnumerator PerformPlayerAction(Character character, Skill skill, Character target) // Moved method inside the class

    {
        HideActionMenu();
        combatManager.ExecuteAction(character, skill, target);
        yield return new WaitForSeconds(1.5f);

        // Only end turn after action is complete
        yield return new WaitForSeconds(1.5f); // Wait for action animation or effects

        // Verify action completion before ending turn
        if (combatManager.IsActionComplete())
        {
            isPlayerTurnActive = false;
            Debug.Log($"Player action complete - ending turn");
            EndTurn();
        }
        else
        {
            Debug.LogWarning("Action not complete - waiting...");
            yield return new WaitUntil(() => combatManager.IsActionComplete());
            isPlayerTurnActive = false;
            Debug.Log($"Player action finally complete - ending turn");
            EndTurn();
        }

        isPlayerTurnActive = false;
    }

    void HideActionMenu() // Moved method inside the class

    {
        actionMenuPanel.SetActive(false);
    }

    void UpdateTurnOrderText() // Moved method inside the class

    {
        string turnOrderDisplay = "Turn Order:\n";
        string currentTurnColor = "<color=green>"; // Define color for current turn

        foreach (Character character in turnOrder)
        {
            if (character.health >= 0)
            {
                turnOrderDisplay += $"SPD: {character.speed}";
                turnOrderDisplay += character.isEnemy ? " (Enemy)" : "";
                turnOrderDisplay += $"{(character == turnOrder[currentTurnIndex] ? currentTurnColor : "")} {character.name}{(character == turnOrder[currentTurnIndex] ? "</color>" : "")}";
                turnOrderDisplay += $" HP: {character.health}\n";
            }
        }
        turnOrderText.text = turnOrderDisplay;
    }

    void UpdateTurnNumberText() // Moved method inside the class

    {
        turnNumberText.text = "Turn: " + turnNumber;
    }

    void LogTeams() // Moved method inside the class

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

    void AddTestCharacters() // Moved method inside the class

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
