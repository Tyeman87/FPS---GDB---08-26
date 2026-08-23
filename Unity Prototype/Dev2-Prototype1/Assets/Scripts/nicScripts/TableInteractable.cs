using System.Collections;
using UnityEngine;

public class TableInteractable : MonoBehaviour, IInteractable
{
    [Header("Table Settings")]
    [SerializeField] float flipAngle = 90f;
    [SerializeField] float flipSpeed = 3f;

    bool flipped;
    bool flipping;

    public void Interact()
    {
        if (flipped || flipping)
            return;

        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("TableInteractable could not find Player.");
            return;
        }

        // Get direction from player toward table
        Vector3 direction = transform.position - player.transform.position;

        // Ignore vertical difference
        direction.y = 0f;
        direction.Normalize();

        StartCoroutine(FlipTable(direction));
    }

    IEnumerator FlipTable(Vector3 direction)
    {
        flipping = true;

        Quaternion startRotation = transform.rotation;

        // Determine which side of the table the player is on
        float forwardDot = Vector3.Dot(transform.forward, direction);
        float rightDot = Vector3.Dot(transform.right, direction);

        Quaternion targetRotation;

        if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
        {
            // Flip forward/backward
            float angle = forwardDot > 0 ? flipAngle : -flipAngle;

            targetRotation = startRotation *
                             Quaternion.Euler(angle, 0f, 0f);
        }
        else
        {
            // Flip left/right
            float angle = rightDot > 0 ? -flipAngle : flipAngle;

            targetRotation = startRotation *
                             Quaternion.Euler(0f, 0f, angle);
        }

        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * flipSpeed;

            transform.rotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                elapsed
            );

            yield return null;
        }

        transform.rotation = targetRotation;

        flipped = true;
        flipping = false;
    }
}