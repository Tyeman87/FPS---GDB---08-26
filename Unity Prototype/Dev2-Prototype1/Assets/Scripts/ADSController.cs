using Unity.VisualScripting;
using UnityEngine;

public class ADSController : MonoBehaviour
{
    [Header("Gun")]
    public Transform gun;
    public Transform adsPosition;

    [Header("Camera")]
    public Camera playerCamera;

    [Header("ADS Settings")]
    public float normalFOV = 90f;
    public float adsFOV = 60f;
    public float adsSpeed = 10f;

    private Vector3 hipPosition;
    private Quaternion hipRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hipPosition = gun.localPosition;
        hipRotation = gun.localRotation;

        playerCamera.fieldOfView = normalFOV;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
