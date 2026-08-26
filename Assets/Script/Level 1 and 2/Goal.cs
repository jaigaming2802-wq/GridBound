using UnityEngine;

public class Goal : MonoBehaviour
{
    private bool completed;

    private void OnEnable()
    {
        completed = false;
    }

    private void Update()
    {
        Debug.Log("CHECKING GOAL: " + gameObject.name);

        if (completed)
            return;

        Collider2D hit = Physics2D.OverlapBox(transform.position, new Vector2(0.8f, 0.8f), 0f);

        if (hit == null)
            return;

        BoxMovement box = hit.GetComponent<BoxMovement>();

        if (box == null)
            return;

        completed = true;

        Debug.Log("GOAL COMPLETED: " + gameObject.name);

        LevelManager levelManager = FindFirstObjectByType<LevelManager>();


        if (levelManager != null)
        {
            Debug.Log("CALLING LEVEL MANAGER FROM: " + gameObject.name);
            levelManager.CheckLevelComplete();
        }
    }

    public bool IsCompleted()
    {
        return completed;
    }

    public void ResetGoal()
    {
        completed = false;
    }
}