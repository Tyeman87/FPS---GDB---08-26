using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [Range(10, 200)][SerializeField] int Sense;
    [SerializeField] int minVert, maxVert;

    float camRotX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Sense;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Sense;

            camRotX -= mouseY;
            camRotX = Mathf.Clamp(camRotX, minVert, maxVert);
            transform.localRotation = Quaternion.Euler(camRotX, 0, 0);

            transform.parent.Rotate(Vector3.up * mouseX);
        }
    }
}
