using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TMPro; // Używamy TextMeshPro zamiast UnityEngine.UI

public enum EnemyState
{
    Idle,
    Evaluate,
    Attack,
    Dead
}

public class EnemyAI : MonoBehaviour
{
    public TextMeshProUGUI decisionText; // Zmieniono z Text na TextMeshProUGUI
    private List<string> decisionHistory = new List<string>(); // Przechowuje do 5 ostatnich decyzji

    private EnemyState currentState;
    private Character target;

    void Start()
    {
        ChangeState(EnemyState.Idle, null);
    }

    public void Initialize(Character character)
    {
        if (character == null)
        {
            Debug.LogError("Enemy character component is missing!");
            return;
        }
        ChangeState(EnemyState.Idle, character);
    }

    void ChangeState(EnemyState newState, Character character)
    {
        currentState = newState;
        LogDecision($"State changed to: {newState}");
        switch (currentState)
        {
            case EnemyState.Idle:
                StartCoroutine(IdleRoutine());
                break;
            case EnemyState.Evaluate:
                EvaluateAction(character);
                break;
            case EnemyState.Attack:
                StartCoroutine(AttackRoutine(character));
                break;
            case EnemyState.Dead:
                HandleDeath();
                break;
        }
    }

    IEnumerator IdleRoutine()
    {
        yield return new WaitForSeconds(1f);
        ChangeState(EnemyState.Evaluate, null);
    }

    void EvaluateAction(Character character)
    {
        target = FindBestTarget(character);
        if (target != null)
        {
            LogDecision($"Target selected: {target.name}");
            ChangeState(EnemyState.Attack, character);
        }
        else
        {
            ChangeState(EnemyState.Idle, character);
        }
    }

    IEnumerator AttackRoutine(Character enemyCharacter)
    {
        if (target == null || !target.IsAlive() || enemyCharacter.skills == null || enemyCharacter.skills.Count == 0)
        {
            LogDecision("Attack aborted, ending turn");
            EndTurn();
            yield break;
        }

        Skill chosenSkill = ChooseBestSkill(target, enemyCharacter);
        if (chosenSkill != null)
        {
            LogDecision($"Using skill: {chosenSkill.name} on {target.name}");
            enemyCharacter.Attack(target, chosenSkill);
        }

        yield return new WaitForSeconds(0.5f);
        EndTurn();
    }

    void HandleDeath()
    {
        LogDecision("Enemy has died.");
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

        if (Random.value < 0.2f)
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
            LogDecision($"Choosing skill based on low HP target: {target.name}");
            return enemyCharacter.skills
                .OrderByDescending(s => s.hitChance)
                .FirstOrDefault();
        }
        else
        {
            LogDecision("Choosing highest damage skill");
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
