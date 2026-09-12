using UnityEngine;

[CreateAssetMenu]

public class ItemStats : ScriptableObject
{
    [SerializeField] public int itemCost;
    public string itemID;
    public string itemName;
    //add field for visual: icon or model?

}
