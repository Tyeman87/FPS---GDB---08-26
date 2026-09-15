using UnityEngine;
using UnityEngine.UI;

public class stealthBar : MonoBehaviour
{
    [Header("Stealth Bar")]
    public Slider StealthBar;

    [Header("Noise Speeds")]
    public float walkingNoise = 0.15f;
    public float runningNoise = 0.4f;
    public float shootingNoise = 1.0f;
    public float decreaseSpeed = 0.5f;

    private void Update()
    {

        if (StealthGameMode.Instance == null || !StealthGameMode.Instance.stealthModeActive)
            return;

        if (StealthBar == null)
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool isMoving = Mathf.Abs(horizontal) > 1.0f || Mathf.Abs(vertical) > 1.0f;

        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isShooting = Input.GetButton("Fire1");

        if (isCrouching)
        {
            StealthBar.value -= decreaseSpeed * Time.deltaTime;
        }
        else if (isShooting)
        {
            StealthBar.value += shootingNoise * Time.deltaTime;
        }
        else if (isMoving && isRunning)
        {
            StealthBar.value += runningNoise * Time.deltaTime;
        }
        else if (isMoving)
        {
            StealthBar.value += walkingNoise * Time.deltaTime;
        }
        else
        {
            StealthBar.value -= decreaseSpeed * Time.deltaTime;
        }

        StealthBar.value = Mathf.Clamp01(StealthBar.value);
    }
}
