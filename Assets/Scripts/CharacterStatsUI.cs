using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class CharacterStatsUI : MonoBehaviour
{
    public TextMeshProUGUI characterNameText;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI speedText;

    public GameObject statsPanel;
    public GameObject healthBarPrefab;
    public Transform[] playerPositions;
    public Transform[] enemyPositions;

    public GameObject enemySelectionPanel;
    public GameObject enemyButtonPrefab;

    private Dictionary<Character, GameObject> spawnedCharacters = new Dictionary<Character, GameObject>();


    private void Start()
    {
        // Initialize any necessary components
    }

    public void ShowCharacterStats(Character character)
    {
        if (statsPanel == null || character == null) return;
        statsPanel.SetActive(true);

        characterNameText.text = !character.isEnemy ? character.name : "(Enemy) " + character.name;
        healthSlider.maxValue = character.maxHealth;
        healthSlider.value = character.health;
        healthText.text = $"{character.health} / {character.maxHealth}";
        attackText.text = "ATT: " + character.attack;
        defenseText.text = "DEF: " + character.defense;
        speedText.text = "SPD: " + character.speed;

        if (!spawnedCharacters.ContainsKey(character))
        {
            SpawnCharacter(character);
        }
        UpdateEnemySelectionPanel();
    }

    private void SpawnCharacter(Character character)
    {
        if (character == null)
        {
            Debug.LogError("Attempted to spawn null character");
            return;
        }

        if (spawnedCharacters.ContainsKey(character))
        {
            Debug.LogWarning($"Character {character.name} is already spawned.");
            return;
        }

        string prefabName = character.name.Replace("(Enemy) ", "").Trim();
        string path = character.isEnemy ? "Prefabs/Enemies/" + prefabName : "Prefabs/" + prefabName;

        GameObject characterPrefab = Resources.Load<GameObject>(path);
        if (characterPrefab == null)
        {
            Debug.LogError("Character prefab not found at: " + path);
            return;
        }

        Transform spawnPoint = GetSpawnPoint(character);
        if (spawnPoint == null)
        {
            Debug.LogError("No spawn position available for " + character.name);
            return;
        }

        Quaternion rotation = character.isEnemy ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
        GameObject characterObject = Instantiate(characterPrefab, spawnPoint.position, rotation);
        characterObject.name = character.name;

        Animator animator = characterObject.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Character prefab " + character.name + " is missing an Animator component");
        }

        // Add health bar for both players and enemies
        if (healthBarPrefab != null)
        {
            GameObject healthBarObject = Instantiate(healthBarPrefab);

            // Set up canvas components
            Canvas canvas = healthBarObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = healthBarObject.AddComponent<Canvas>();
            }
            canvas.renderMode = RenderMode.WorldSpace;

            // Add CanvasScaler if not present
            CanvasScaler scaler = healthBarObject.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = healthBarObject.AddComponent<CanvasScaler>();
            }
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // Set parent and position
            healthBarObject.transform.SetParent(characterObject.transform);
            // Adjust position based on character type
            float yOffset = character.isEnemy ? 1.8f : 2f;
            healthBarObject.transform.localPosition = new Vector3(0f, yOffset, 0f);
            healthBarObject.transform.localScale = Vector3.one * 0.01f;

            // Add HealthBar component if missing
            HealthBar healthBar = healthBarObject.GetComponent<HealthBar>();
            if (healthBar == null)
            {
                healthBar = healthBarObject.AddComponent<HealthBar>();
            }

            // Add Slider component if missing
            Slider slider = healthBarObject.GetComponentInChildren<Slider>();
            if (slider == null)
            {
                Debug.LogError("Slider component not found in health bar prefab");
                return;
            }

            // Initialize health bar
            healthBar.slider = slider;
            healthBar.SetMaxHealth(character.maxHealth);
            healthBar.UpdateHealth(character.health);

            // Register health change callback with debug logging
            character.OnHealthChanged += (newHealth) =>
            {
                healthBar.UpdateHealth(newHealth);
                if (character.isEnemy)
                {
                    Debug.Log($"Enemy {character.name} health updated to {newHealth}");
                }
                else
                {
                    Debug.Log($"Player {character.name} health updated to {newHealth}");
                }
            };

            // Debug log for health bar creation
            if (character.isEnemy)
            {
                Debug.Log($"Created health bar for enemy: {character.name} with {character.health}/{character.maxHealth} HP");
            }

            // Force immediate update
            Canvas.ForceUpdateCanvases();

            healthBarObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Health bar prefab is not assigned");
        }

        // Add button click handler for enemies
        if (character.isEnemy)
        {
            Button enemyButton = characterObject.GetComponent<Button>();
            if (enemyButton == null)
            {
                enemyButton = characterObject.AddComponent<Button>();
            }

            enemyButton.onClick.AddListener(() =>
            {
                TurnManager.Instance.SelectTarget(character, false);
            });
        }

        spawnedCharacters[character] = characterObject;
    }



    public void SpawnAllCharacters(List<Character> playerTeam, List<Character> enemyTeam)
    {
        Debug.Log("Starting SpawnAllCharacters");

        if (playerTeam == null)
        {
            Debug.LogError("Player team list is null");
            return;
        }

        if (enemyTeam == null)
        {
            Debug.LogError("Enemy team list is null");
            return;
        }

        if (playerPositions == null)
        {
            Debug.LogError("Player positions array is not assigned!");
            return;
        }

        if (enemyPositions == null)
        {
            Debug.LogError("Enemy positions array is not assigned!");
            return;
        }

        Debug.Log($"Spawning {playerTeam.Count} player characters");
        for (int i = 0; i < playerTeam.Count; i++)
        {
            if (i < playerPositions.Length)
            {
                SpawnCharacter(playerTeam[i]);
                ShowCharacterStats(playerTeam[i]);
            }
            else
            {
                Debug.LogWarning($"Not enough player positions for character {playerTeam[i].name}");
            }
        }

        Debug.Log($"Spawning {enemyTeam.Count} enemy characters");
        for (int i = 0; i < enemyTeam.Count; i++)
        {
            if (i < enemyPositions.Length)
            {
                SpawnCharacter(enemyTeam[i]);
                ShowCharacterStats(enemyTeam[i]);
                TurnManager.Instance.AssignTargetSelectionTriggers(spawnedCharacters[enemyTeam[i]], false);
            }
            else
            {
                Debug.LogWarning($"Not enough enemy positions for character {enemyTeam[i].name}");
            }
        }
    }

    public void PlayAnimation(Character character, string animationName, float duration)
    {
        if (spawnedCharacters.TryGetValue(character, out GameObject characterObject))
        {
            Animator animator = characterObject.GetComponent<Animator>();
            if (animator != null)
            {
                StartCoroutine(ChangeAnimation(animator, animationName, duration));
            }
            else
            {
                Debug.LogError($"Character {character.name} does not have an Animator component");
            }
        }
        else
        {
            Debug.LogError($"Character {character.name} not found in spawned characters");
        }
    }

    public void RemoveCharacter(Character character)
    {
        if (spawnedCharacters.TryGetValue(character, out GameObject characterObject))
        {
            Destroy(characterObject);
            spawnedCharacters.Remove(character);
        }
        else
        {
            Debug.LogError($"Character {character.name} not found in spawned characters");
        }
    }


    private IEnumerator ChangeAnimation(Animator animator, string animationName, float duration)
    {
        animator.Play(animationName);
        yield return new WaitForSeconds(duration);
        animator.Play("idle");
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

    private void UpdateEnemySelectionPanel()
    {
        if (enemySelectionPanel == null || enemyButtonPrefab == null) return;

        foreach (Transform child in enemySelectionPanel.transform)
        {
            Destroy(child.gameObject);
        }

        enemySelectionPanel.SetActive(true);

        foreach (Character enemy in TurnManager.Instance.enemyTeam)
        {
            GameObject buttonObject = Instantiate(enemyButtonPrefab, enemySelectionPanel.transform);
            buttonObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            TextMeshProUGUI buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = enemy.name;
            }

            Button button = buttonObject.GetComponent<Button>();
            if (button != null)
            {
                Character selectedEnemy = enemy;
                button.onClick.AddListener(() => TurnManager.Instance.SelectTarget(selectedEnemy, false));
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(enemySelectionPanel.GetComponent<RectTransform>());
    }

    public void HideCharacterStats()
    {
        if (statsPanel != null)
        {
            statsPanel.SetActive(false);
        }
        if (enemySelectionPanel != null)
        {
            enemySelectionPanel.SetActive(false);
        }
    }

    public void DisplayActionFeedback(string message)
    {
        Debug.Log(message);
    }

    public List<GameObject> GetCharacterUIs()
    {
        return spawnedCharacters.Values.ToList();
    }

    public GameObject GetCharacterUI(Character character)
    {
        if (spawnedCharacters.ContainsKey(character))
        {
            return spawnedCharacters[character];
        }
        return null;
    }

}
