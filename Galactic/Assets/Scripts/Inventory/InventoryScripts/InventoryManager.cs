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

   void Update()
   {
      //CreateItems();
      
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
   
   public static void CreateItems(Sprite image)
   {
      Slot newItem = Instantiate(instance.slotPrefab, instance.slotGrid.transform.position, Quaternion.identity);
      newItem.gameObject.transform.SetParent(instance.slotGrid.transform);
      newItem.slotItem = ScriptableObject.CreateInstance<InventoryItem>();
      newItem.slotImage.sprite = image;
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
      
      //for (int i = 0; i < instance.playerBag.items.Count; i++)
      //{
         //CreateItems();
      //}
   //}
}
