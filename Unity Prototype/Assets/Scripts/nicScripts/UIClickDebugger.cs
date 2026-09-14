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

        if (eventSystem == null)
        {
            Debug.LogError("UIClickDebugger: No EventSystem found!");
        }
        else
        {
            Debug.Log("UIClickDebugger found EventSystem: " + eventSystem.name);
        }
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (eventSystem == null)
        {
            Debug.LogError("UIClickDebugger: EventSystem is null.");
            return;
        }

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        eventSystem.RaycastAll(pointerData, results);

        Debug.Log("===== UI CLICK =====");

        if (results.Count == 0)
        {
            Debug.Log("Nothing was clicked.");
        }
        else
        {
            foreach (RaycastResult result in results)
            {
                Debug.Log("Hit: " + result.gameObject.name);
            }
        }
    }
}