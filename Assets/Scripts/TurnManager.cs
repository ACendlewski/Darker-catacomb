using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public List<Character> playerTeam = new List<Character>(); // Grupa bohaterów
    public List<Character> enemyTeam = new List<Character>();  // Grupa przeciwników

    private List<Character> turnOrder = new List<Character>(); // Kolejność tur
    private int currentTurnIndex = 0;
    private int turnNumber = 1; // Numer tury

    public TextMeshProUGUI turnNumberText; // Referencja do TMP do wyświetlania numeru tury

    void Start()
    {
        if (turnNumberText == null)
        {
            Debug.LogError("TurnNumberText is not assigned!");
            return;
        }

        // Łączenie drużyn graczy i przeciwników
        turnOrder.AddRange(playerTeam);
        turnOrder.AddRange(enemyTeam);

        // Sortowanie kolejności tur według szybkości (malejąco)
        turnOrder.Sort((a, b) => b.speed.CompareTo(a.speed));

        UpdateTurnNumberText(); // Aktualizuj wyświetlany numer tury
        StartTurn(); // Rozpoczynamy pierwszą turę
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
                PerformAction(currentCharacter); // Tu wywołujemy akcję (atakowanie, leczenie itd.)
            }
            else
            {
                EndTurn(); // Jeśli postać zginęła, natychmiast kończymy turę
            }
        }
    }

    // Wykonywanie akcji
    void PerformAction(Character actingCharacter)
    {
        // Na przykładzie prostego ataku, postać z drużyny gracza atakuje przeciwnika, i vice versa
        if (playerTeam.Contains(actingCharacter))
        {
            // Bohater gracza atakuje przeciwnika (przyjmujemy na razie, że atakuje pierwszego żywego wroga)
            Character target = GetFirstAliveCharacter(enemyTeam);
            if (target != null)
            {
                actingCharacter.Attack(target);
            }
        }
        else
        {
            // Wróg atakuje gracza (atak pierwszego żywego bohatera)
            Character target = GetFirstAliveCharacter(playerTeam);
            if (target != null)
            {
                actingCharacter.Attack(target);
            }
        }

        EndTurn(); // Po akcji kończymy turę
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
}
