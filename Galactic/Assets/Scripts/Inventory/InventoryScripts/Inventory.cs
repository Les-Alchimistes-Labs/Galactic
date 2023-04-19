using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "New Inventory",menuName = "Inventory/New Inventory")]
public class Inventory : ScriptableObject
{
    public List<(string, int, Sprite)> items = new List<(string, int, Sprite)>();
    public List<InventoryItem> itemList = new List<InventoryItem>();
}
