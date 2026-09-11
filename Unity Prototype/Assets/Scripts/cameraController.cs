using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [Range(10, 200)][SerializeField] int Sense;
    [SerializeField] int lockVertMin, lockVertMax;

    float camRotX;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Sense;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Sense;

            camRotX -= mouseY;
            camRotX = Mathf.Clamp(camRotX, lockVertMin, lockVertMax);
            transform.localRotation = Quaternion.Euler(camRotX, 0, 0);

            transform.parent.Rotate(Vector3.up * mouseX);
        }
    }
}
