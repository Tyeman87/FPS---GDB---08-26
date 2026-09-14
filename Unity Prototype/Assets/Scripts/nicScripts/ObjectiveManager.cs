using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public TMP_Text objectiveText;
    public TMP_Text progressText;

    public void SetObjective(string objective)
    {
        objectiveText.text = objective;
        progressText.text = "";
    }

    public void SetObjective(string objective, string progress)
    {
        objectiveText.text = objective;
        objectiveText.text = "";
    }

    public void ClearObjective()
    {
        objectiveText.text = "";
        progressText.text = "";
    }
}
