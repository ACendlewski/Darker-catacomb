using UnityEngine;

public class Test_Animation_prefabs : MonoBehaviour
{
    private Animator animator;
    private string[] animationStates = { "attack", "casting", "die", "hurt", "idle", "victory" };
    private int currentAnimationIndex = 0;
    private float animationChangeInterval = 10f;
    private float timer;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the GameObject!");
            return;
        }
        timer = animationChangeInterval;
        animator.Play(animationStates[currentAnimationIndex]);
    }

    void Update()
    {
        if (animator == null) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ChangeAnimation();
            timer = animationChangeInterval;
        }
    }

    void ChangeAnimation()
    {
        currentAnimationIndex = (currentAnimationIndex + 1) % animationStates.Length;
        animator.Play(animationStates[currentAnimationIndex]);
        Debug.Log("TEST Changed animation to: " + animationStates[currentAnimationIndex]);
    }
}
