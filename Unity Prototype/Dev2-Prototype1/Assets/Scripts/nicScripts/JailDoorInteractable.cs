using System.Collections;
using UnityEngine;

public class JailDoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Door")]
    [SerializeField] private Transform hingePivot;
    [SerializeField] private float openAngle = 95f;
    [SerializeField] private float openDuration = 0.35f;
    [SerializeField] private bool invertDirection = false;

    [Header("Hostage")]
    [SerializeField] private HostageSpawner hostageSpawner;

    [Header("State")]
    [SerializeField] private bool startOpen = false;

    private Quaternion closedRotation;
    private Vector3 closedPosition;
    private Vector3 pivotPosition;
    private Vector3 rotationAxis;

    private bool isOpen;
    private Coroutine doorRoutine;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        if (hingePivot == null && transform.parent != null)
        {
            hingePivot = transform.parent.Find("Hinges");
        }

        closedPosition = transform.position;
        closedRotation = transform.rotation;

        pivotPosition = hingePivot != null
            ? hingePivot.position
            : transform.position;

        rotationAxis = hingePivot != null
            ? hingePivot.up
            : transform.up;

        SetOpen(startOpen, true);
    }

    public void Interact()
    {
        Debug.Log("Jail door interacted with!");

        SetOpen(!isOpen);
    }

    public void SetOpen(bool open, bool instant = false)
    {
        isOpen = open;

        // Release hostage when the door opens
        if (open && hostageSpawner != null)
        {
            hostageAI npc = hostageSpawner.GetHostage();

            if (npc != null)
            {
                npc.OpenCell();
            }
            else
            {
                Debug.LogError(
                    "JailDoorInteractable: No hostage found in spawner!"
                );
            }
        }

        if (doorRoutine != null)
        {
            StopCoroutine(doorRoutine);
            doorRoutine = null;
        }

        float targetAngle = open
            ? (invertDirection ? -openAngle : openAngle)
            : 0f;

        if (instant || openDuration <= 0f)
        {
            ApplyDoorRotation(targetAngle);
            return;
        }

        doorRoutine = StartCoroutine(
            AnimateDoor(targetAngle)
        );
    }

    private IEnumerator AnimateDoor(float targetAngle)
    {
        float startAngle = GetCurrentAngle();
        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / openDuration
            );

            float currentAngle = Mathf.Lerp(
                startAngle,
                targetAngle,
                t
            );

            ApplyDoorRotation(currentAngle);

            yield return null;
        }

        ApplyDoorRotation(targetAngle);

        doorRoutine = null;
    }

    private float GetCurrentAngle()
    {
        Quaternion relativeRotation =
            Quaternion.Inverse(closedRotation)
            * transform.rotation;

        relativeRotation.ToAngleAxis(
            out float angle,
            out Vector3 axis
        );

        if (Vector3.Dot(axis, rotationAxis) < 0)
        {
            angle = -angle;
        }

        if (angle > 180f)
        {
            angle -= 360f;
        }

        return angle;
    }

    private void ApplyDoorRotation(float angle)
    {
        Quaternion rotation =
            Quaternion.AngleAxis(
                angle,
                rotationAxis
            );

        Vector3 position =
            rotation * (closedPosition - pivotPosition)
            + pivotPosition;

        Quaternion newRotation =
            rotation * closedRotation;

        transform.SetPositionAndRotation(
            position,
            newRotation
        );
    }
}