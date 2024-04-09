using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Movement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private bool isMoving = false;
    private Vector3 targetPosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetTargetPosition();
        }

        if (isMoving)
        {
            MoveCharacter();
        }
    }

    void SetTargetPosition()
    {
        Plane plane = new Plane(Vector3.forward, 0f);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float distance))
        {
            targetPosition = ray.GetPoint(distance);
            isMoving = true;
        }
    }

    void MoveCharacter()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
        }
    }
}
