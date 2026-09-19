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
    }

    public void DrainRisk()
    {
        if (detectionRiskBar == null)
            return;

        detectionRiskBar.value = Mathf.MoveTowards(detectionRiskBar.value, 0f, decreaseSpeed * Time.deltaTime);
    }
}
