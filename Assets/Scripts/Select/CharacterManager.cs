using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

public List<Character> selectedCharacters = new List<Character>();
public Dictionary<Character, int> characterCounts = new Dictionary<Character, int>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCharacter(Character character)
    {
        if (characterCounts.ContainsKey(character))
        {
            characterCounts[character]++;
        }
        else
        {
            characterCounts[character] = 1;
        }

    
        selectedCharacters.Add(character);
    }
}
