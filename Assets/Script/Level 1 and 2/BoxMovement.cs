using UnityEngine;

public class BoxMovement : MonoBehaviour
{
    private Vector2 startPosition;
    private void Awake()
    {
        startPosition=transform.position;
    }

    public void MoveBox(Vector2 direction , float gridSize)
    {
        Vector2 targetposition = (Vector2) transform.position+ direction * gridSize;
        transform.position = targetposition;
    }

    public void ResetBox()
        {
        transform.position = startPosition;
        }


}