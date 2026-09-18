using UnityEngine;
using UnityEngine.UI;

public class DetectionRisk : MonoBehaviour
{
    [Header("Detection Risk Bar")]
    public Slider detectionRiskBar;

    [Header("Risk Settings")]
    public float increaseSpeed = 0.5f;
    public float decreaseSpeed = 0.25f;

    private void Update()
    {
        if (detectionRiskBar == null)
            return;

        detectionRiskBar.value -= decreaseSpeed * Time.deltaTime;

        detectionRiskBar.value = Mathf.Clamp01(detectionRiskBar.value);
    }
}
