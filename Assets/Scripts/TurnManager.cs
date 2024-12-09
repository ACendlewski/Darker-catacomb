using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public TextMeshProUGUI turnNumberText; // Referencja do TMP do wyświetlania numeru tury
    public int numberOfEnemies = 4; // Liczba przeciwników w drużynie

    private List<Character> playerTeam = new List<Character>(); // Grupa bohaterów
    private List<Character> enemyTeam = new List<Character>();  // Grupa przeciwników
    private List<Character> allCharacters = new List<Character>(); // Wszystkie dostępne postacie

    private List<Character> turnOrder = new List<Character>(); // Kolejność tur
    private int currentTurnIndex = 0;
    private int turnNumber = 1; // Numer tury

    public CharacterLoader characterLoader;

    void Start()
    {
        if (turnNumberText == null)
        {
            Debug.LogError("TurnNumberText is not assigned!");
            return;
        }
        StartCoroutine(WaitForCharacterLoader());
    }
    IEnumerator WaitForCharacterLoader()
    {
        // Czekamy na załadowanie postaci z CharacterLoader
        yield return new WaitUntil(() => characterLoader.characters != null && characterLoader.characters.Count > 0);

        // Pobierz postacie z CharacterLoader
        allCharacters = characterLoader.characters;
        Debug.Log("All characters:");
        foreach (Character character in allCharacters)
        {
            Debug.Log($"Character: {character.name} (Health: {character.health}, Speed: {character.speed})");
        }

        // Pobierz wybrane postacie gracza
        playerTeam = CharacterManager.Instance.selectedCharacters;

        // Wybierz przeciwników losowo z pozostałych postaci
        SelectRandomEnemies();
        StartCoroutine(WaitForEnemies());
    }
        IEnumerator WaitForEnemies() {

        yield return new WaitUntil(() => enemyTeam != null && enemyTeam.Count > 0);

        // Łączenie drużyn graczy i przeciwników
        turnOrder.AddRange(playerTeam);
        turnOrder.AddRange(enemyTeam);

        // Sortowanie kolejności tur według szybkości (malejąco)
        turnOrder.Sort((a, b) => b.speed.CompareTo(a.speed));

        // Logowanie drużyn
        LogTeams();

        UpdateTurnNumberText(); // Aktualizuj wyświetlany numer tury
        StartTurn(); // Rozpoczynamy pierwszą turę
    
    }

    void SelectRandomEnemies()
    {
        List<Character> availableEnemies = new List<Character>(allCharacters); // Tworzymy kopię listy wszystkich postaci

        // Usuwamy wybrane postacie gracza z listy dostępnych przeciwników
        foreach (Character selected in playerTeam)
        {
            availableEnemies.Remove(selected);
        }

        Debug.Log("Available enemies after removing player team: " + availableEnemies.Count);

        // Losowo wybieramy przeciwników
        while (enemyTeam.Count < numberOfEnemies && availableEnemies.Count > 0)
        {
            int randomIndex = Random.Range(0, availableEnemies.Count);
            enemyTeam.Add(availableEnemies[randomIndex]);
            availableEnemies.RemoveAt(randomIndex);
        }
        
    }

    // Rozpoczęcie tury
    void StartTurn()
    {
        if (turnOrder.Count > 0 && currentTurnIndex < turnOrder.Count)
        {
            Character currentCharacter = turnOrder[currentTurnIndex];

            if (currentCharacter.IsAlive())
            {
                Debug.Log("It's " + currentCharacter.name + "'s turn!");
         
            }
            else
            {
                EndTurn(); // Jeśli postać zginęła, natychmiast kończymy turę
            }
        }
    }



    // Zakończenie tury
    public void EndTurn()
    {
        currentTurnIndex++;

        if (currentTurnIndex >= turnOrder.Count)
        {
            currentTurnIndex = 0; // Restart cyklu tur
            turnNumber++; // Zwiększ numer tury po zakończeniu cyklu
            UpdateTurnNumberText(); // Aktualizuj wyświetlany numer tury
        }

        StartTurn(); // Rozpocznij turę kolejnej postaci
    }

    // Funkcja zwracająca pierwszą żyjącą postać z danej drużyny
    Character GetFirstAliveCharacter(List<Character> team)
    {
        foreach (Character member in team)
        {
            if (member.IsAlive())
            {
                return member;
            }
        }
        return null; // Jeśli żadna postać nie żyje
    }

    // Aktualizacja wyświetlania numeru tury
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



}
