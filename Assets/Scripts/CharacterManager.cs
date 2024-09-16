using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    public List<Character> selectedCharacters = new List<Character>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Jeœli chcesz, aby obiekt przetrwa³ zmiany sceny
        }
        else
        {
            Destroy(gameObject); // Zniszcz duplikaty
        }
    }

    public void AddCharacter(Character character)
    {
        selectedCharacters.Add(character);
    }
}
