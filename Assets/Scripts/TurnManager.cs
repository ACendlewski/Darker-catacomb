using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public List<Character> playerTeam = new List<Character>(); // Grupa bohaterów
    public List<Character> enemyTeam = new List<Character>();  // Grupa przeciwników

    private List<Character> turnOrder = new List<Character>(); // Kolejnoœæ tur
    private int currentTurnIndex = 0;

    void Start()
    {
        // £¹czenie dru¿yn graczy i przeciwników
        turnOrder.AddRange(playerTeam);
        turnOrder.AddRange(enemyTeam);

        // Sortowanie kolejnoœci tur wed³ug szybkoœci (malej¹co)
        turnOrder.Sort((a, b) => b.speed.CompareTo(a.speed));

        StartTurn(); // Rozpoczynamy pierwsz¹ turê
    }

    // Rozpoczêcie tury
    void StartTurn()
    {
        if (turnOrder.Count > 0 && currentTurnIndex < turnOrder.Count)
        {
            Character currentCharacter = turnOrder[currentTurnIndex];

            if (currentCharacter.IsAlive())
            {
                Debug.Log("It's " + currentCharacter.name + "'s turn!");
                PerformAction(currentCharacter); // Tu wywo³ujemy akcjê (atakowanie, leczenie itd.)
            }
            else
            {
                EndTurn(); // Jeœli postaæ zginê³a, natychmiast koñczymy turê
            }
        }
    }

    // Wykonywanie akcji
    void PerformAction(Character actingCharacter)
    {
        // Na przyk³adzie prostego ataku, postaæ z dru¿yny gracza atakuje przeciwnika, i vice versa
        if (playerTeam.Contains(actingCharacter))
        {
            // Bohater gracza atakuje przeciwnika (przyjmujemy na razie, ¿e atakuje pierwszego ¿ywego wroga)
            Character target = GetFirstAliveCharacter(enemyTeam);
            if (target != null)
            {
                actingCharacter.Attack(target);
            }
        }
        else
        {
            // Wróg atakuje gracza (atak pierwszego ¿ywego bohatera)
            Character target = GetFirstAliveCharacter(playerTeam);
            if (target != null)
            {
                actingCharacter.Attack(target);
            }
        }

        EndTurn(); // Po akcji koñczymy turê
    }

    // Zakoñczenie tury
    public void EndTurn()
    {
        currentTurnIndex++;

        if (currentTurnIndex >= turnOrder.Count)
        {
            currentTurnIndex = 0; // Restart cyklu tur
        }

        StartTurn(); // Rozpocznij turê kolejnej postaci
    }

    // Funkcja zwracaj¹ca pierwsz¹ ¿yj¹c¹ postaæ z danej dru¿yny
    Character GetFirstAliveCharacter(List<Character> team)
    {
        foreach (Character member in team)
        {
            if (member.IsAlive())
            {
                return member;
            }
        }
        return null; // Jeœli ¿adna postaæ nie ¿yje
    }
}
