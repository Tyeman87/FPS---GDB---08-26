using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIClickDebugger : MonoBehaviour
{
    private EventSystem eventSystem;

    private void Start()
    {
        eventSystem = EventSystem.current;

        if (eventSystem == null)
        {
            eventSystem = FindFirstObjectByType<EventSystem>();
        }
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (eventSystem == null)
        {
            return;
        }

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        eventSystem.RaycastAll(pointerData, results);

        if (results.Count == 0)
        {
        }
        else
        {
            foreach (RaycastResult result in results)
            {
            }
        }
    }
}