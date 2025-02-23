using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public enum EnemyState
{
    Idle,
    Evaluate,
    Attack,
    Dead
}


public class EnemyAI : MonoBehaviour
{
    public TextMeshProUGUI decisionText; // Public TextMeshProUGUI field to show decisions
    private List<string> decisionHistory = new List<string>(); // List to store last 5 decisions


    private EnemyState currentState;
    private Character target;

    void Start()
    {
        ChangeState(EnemyState.Idle, null); // Pass null for character initially
    }

    public void Initialize(Character character) // Pass enemyCharacter as a parameter
    {
        if (character == null)
        {
            Debug.LogError("Enemy character component is missing!");
            return;
        }
        ChangeState(EnemyState.Idle, character); // Pass the character instance
    }

    void ChangeState(EnemyState newState, Character character)
    {
        currentState = newState;
        switch (currentState)
        {
            case EnemyState.Idle:
                StartCoroutine(IdleRoutine());
                break;
            case EnemyState.Evaluate:
                EvaluateAction(character); // Pass character to EvaluateAction
                break;
            case EnemyState.Attack:
                StartCoroutine(AttackRoutine(character)); // Pass character to AttackRoutine
                break;
            case EnemyState.Dead:
                HandleDeath();
                break;
        }
    }

    IEnumerator IdleRoutine()
    {
        yield return new WaitForSeconds(1f);
        ChangeState(EnemyState.Evaluate, null); // Pass null for character
    }

    void EvaluateAction(Character character)
    {
        target = FindBestTarget(character); // Pass character to FindBestTarget
        if (target != null)
        {
            LogDecision($"Target selected: {target.name}"); // Log decision
            ChangeState(EnemyState.Attack, character); // Pass character to ChangeState
        }
        else
        {
            ChangeState(EnemyState.Idle, character); // Pass character to ChangeState
        }
    }


    IEnumerator AttackRoutine(Character enemyCharacter) // Pass enemyCharacter as a parameter
    {
        if (target == null || !target.IsAlive() || enemyCharacter.skills == null || enemyCharacter.skills.Count == 0)
        {
            EndTurn();
            yield break;
        }

        Skill chosenSkill = ChooseBestSkill(target, enemyCharacter); // Pass enemyCharacter
        if (chosenSkill != null)
        {
            enemyCharacter.Attack(target, chosenSkill);
        }

        yield return new WaitForSeconds(0.5f); // Short pause before changing state
        EndTurn();
    }

    void HandleDeath()
    {
        LogDecision($"{target.name} is dying..."); // Log decision
        Debug.Log($"{target.name} is dying...");
        Destroy(gameObject);
        
    }
    void EndTurn()
        {
            ChangeState(EnemyState.Idle, null); // Pass null for character
        }

    public Character FindBestTarget(Character character)
    {
        if (CharacterManager.Instance == null || CharacterManager.Instance.selectedCharacters == null || character == null)
            return null;

        var sortedTargets = CharacterManager.Instance.selectedCharacters
            .Where(c => c.IsAlive())
            .OrderBy(c => c.health)
            .ToList();

        if (sortedTargets.Count == 0) return null;

        if (Random.value < 0.2f) // 20% chance to choose a random target
        {
            return sortedTargets[Random.Range(0, sortedTargets.Count)];
        }

        return sortedTargets.First();
    }

    public Skill ChooseBestSkill(Character target, Character enemyCharacter)
    {
        if (enemyCharacter.skills == null || enemyCharacter.skills.Count == 0)
            return null;

        if (target.health <= target.maxHealth * 0.45f)
        {
            LogDecision($"Choosing skill based on target health: {target.health}"); // Log decision
                                                                                    // If the target has < 45% HP, choose the skill with the highest hit chance
            return enemyCharacter.skills
                .OrderByDescending(s => s.hitChance)
                .FirstOrDefault();

        }
        else
        {
            // Normally choose the skill with the highest damage
            return enemyCharacter.skills

                    .OrderByDescending(s => s.damage)
                    .FirstOrDefault();
        }
    }

    private void LogDecision(string decision)
    {
        if (decisionHistory.Count >= 5)
        {
            decisionHistory.RemoveAt(0);
        }
        decisionHistory.Add(decision);

        if (decisionText != null)
        {
            decisionText.text = string.Join("\n", decisionHistory);
        }

        Debug.Log(decision);
    }
}
