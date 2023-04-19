using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
   static InventoryManager instance;

   public Inventory playerBag;

   public GameObject slotGrid;
   public Slot slotPrefab;
   public Text itemInformation;

   void Awake()
   {
      if (instance is not null)
      {
         Destroy(this);
      }

      instance = this;
   }
   

   private void OnEnable()
   {
      //RefreshItem();
      instance.itemInformation.text = "";
   }

   public static void UpdateItemInfo(string itemDescription)
   {
      instance.itemInformation.text = itemDescription;
   }
   
   public static void CreateNewItem(InventoryItem item)
   {
      Slot newItem = Instantiate(instance.slotPrefab, instance.slotGrid.transform.position, Quaternion.identity);
      newItem.gameObject.transform.SetParent(instance.slotGrid.transform);
      newItem.slotItem = item;
      newItem.slotImage.sprite = item.itemImage;
   }
   
   
   //public static void RefreshItem()
   //{
      //for (int i = 0; i < instance.slotGrid.transform.childCount; i++)
      //{
         //if (instance.slotGrid.transform.childCount == 0)
         //{
            //break;
         //}
         //Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
      //}
      
      //for (int i = 0; i < instance.playerBag.itemList.Count; i++)
      //{
         //CreateNewItem(instance.playerBag.itemList[i]);
      //}
   //}
}
