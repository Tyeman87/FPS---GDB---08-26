using UnityEngine;

public class LdrCallback : MonoBehaviour
{
    private bool isFirstUpdate = true;

    private void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            MenuLdr.LoaderCallback();
        }
    }
}