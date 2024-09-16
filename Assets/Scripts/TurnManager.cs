using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public List<Character> playerTeam = new List<Character>(); // Grupa bohater�w
    public List<Character> enemyTeam = new List<Character>();  // Grupa przeciwnik�w

    private List<Character> turnOrder = new List<Character>(); // Kolejno�� tur
    private int currentTurnIndex = 0;

    void Start()
    {
        // ��czenie dru�yn graczy i przeciwnik�w
        turnOrder.AddRange(playerTeam);
        turnOrder.AddRange(enemyTeam);

        // Sortowanie kolejno�ci tur wed�ug szybko�ci (malej�co)
        turnOrder.Sort((a, b) => b.speed.CompareTo(a.speed));

        StartTurn(); // Rozpoczynamy pierwsz� tur�
    }

    // Rozpocz�cie tury
    void StartTurn()
    {
        if (turnOrder.Count > 0 && currentTurnIndex < turnOrder.Count)
        {
            Character currentCharacter = turnOrder[currentTurnIndex];

            if (currentCharacter.IsAlive())
            {
                Debug.Log("It's " + currentCharacter.name + "'s turn!");
                PerformAction(currentCharacter); // Tu wywo�ujemy akcj� (atakowanie, leczenie itd.)
            }
            else
            {
                EndTurn(); // Je�li posta� zgin�a, natychmiast ko�czymy tur�
            }
        }
    }

    // Wykonywanie akcji
    void PerformAction(Character actingCharacter)
    {
        // Na przyk�adzie prostego ataku, posta� z dru�yny gracza atakuje przeciwnika, i vice versa
        if (playerTeam.Contains(actingCharacter))
        {
            // Bohater gracza atakuje przeciwnika (przyjmujemy na razie, �e atakuje pierwszego �ywego wroga)
            Character target = GetFirstAliveCharacter(enemyTeam);
            if (target != null)
            {
                actingCharacter.Attack(target);
            }
        }
        else
        {
            // Wr�g atakuje gracza (atak pierwszego �ywego bohatera)
            Character target = GetFirstAliveCharacter(playerTeam);
            if (target != null)
            {
                actingCharacter.Attack(target);
            }
        }

        EndTurn(); // Po akcji ko�czymy tur�
    }

    // Zako�czenie tury
    public void EndTurn()
    {
        currentTurnIndex++;

        if (currentTurnIndex >= turnOrder.Count)
        {
            currentTurnIndex = 0; // Restart cyklu tur
        }

        StartTurn(); // Rozpocznij tur� kolejnej postaci
    }

    // Funkcja zwracaj�ca pierwsz� �yj�c� posta� z danej dru�yny
    Character GetFirstAliveCharacter(List<Character> team)
    {
        foreach (Character member in team)
        {
            if (member.IsAlive())
            {
                return member;
            }
        }
        return null; // Je�li �adna posta� nie �yje
    }
}
