using UnityEngine;
using UnityEngine.UI;

public class stealthBar : MonoBehaviour
{
    public Slider StealthBar;

    public float increaseSpeed = 0.25f;
    public float decreaseSpeed = 0.4f;

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool isMoving = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;

        if (isMoving)
        {
            StealthBar.value += increaseSpeed * Time.deltaTime;
        }
        else
        {
            StealthBar.value -= decreaseSpeed * Time.deltaTime;
        }

        StealthBar.value = Mathf.Clamp01(StealthBar.value);
    }
}
