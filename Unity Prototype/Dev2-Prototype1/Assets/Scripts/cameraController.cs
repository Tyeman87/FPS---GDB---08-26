using System.Runtime.CompilerServices;
using UnityEngine;

public class cameraController : MonoBehaviour
{

    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;

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
        float mouseX = Input.GetAxisRaw("Mouse X") * sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens;

        camRotX -= mouseY;
        camRotX = Mathf.Clamp(camRotX, lockVertMin, lockVertMax);
        transform.localRotation = Quaternion.Euler(camRotX, 0, 0);

        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
