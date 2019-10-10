using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInventory : MonoBehaviour
{
    [Serializable]
    public struct Item
    {
        public string name;
        public Texture2D image;
        public int quantity;
        public int maxQuanntity;
    }

    [SerializeField] private Item[] inventory;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void AddItem(string name, int quant)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            Item item = inventory[i];
            int newQuant = item.quantity + quant;

            if (item.name == name)
            {
                inventory[i].quantity = newQuant > item.maxQuanntity ? newQuant : item.maxQuanntity;
                break;
            }
        }
    }

    bool CheckIfCanRemoveItem(string name, int quant)
    {
        bool canRemove = false;

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].name == name)
            {
                canRemove = inventory[i].quantity - quant >= 0;
                break;
            }
        }

        return canRemove;
    }

    void RemoveItem(string name, int quant)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].name == name)
            {
                inventory[i].quantity -= quant;
                break;
            }
        }
    }
}