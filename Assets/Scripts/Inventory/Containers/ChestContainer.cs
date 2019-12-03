//--------------------------------------------------------------------------------------
// Purpose: The main logic for the chest container.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------------------------------------------
// ChestContainer object. Inheriting from Container.
//--------------------------------------------------------------------------------------
public class ChestContainer : Container
{
    //--------------------------------------------------------------------------------------
    // Default Constructor.
    //
    // Param:
    //      oInventory: The inventory used for this container.
    //      oPlayerInventory: The inventory used for the player container.
    //      nSlots: the amount of slots in the container/inventory.
    //--------------------------------------------------------------------------------------
    public ChestContainer(Inventory oInventory, Inventory oPlayerInventory, int nSlots) : base(oInventory, oPlayerInventory, nSlots)
    {
        // loop through each slot
        for (int i = 0; i < m_nSlots; i++)
        {
            // build the inventory slots
            AddSlot(oInventory, i, m_gPrefab.GetComponentInChildren<Transform>().Find("Main Inventory").transform);
        }

        // loop through each slot
        for (int i = 0; i < InventoryManager.m_gInstance.m_gPlayer.m_nInventorySize - InventoryManager.m_gInstance.m_gPlayer.m_nHotbarSize; i++)
        {
            // build the inventory slots
            AddSlot(oPlayerInventory, InventoryManager.m_gInstance.m_gPlayer.m_nHotbarSize + i, m_gPrefab.GetComponentInChildren<Transform>().Find("Player Inventory").transform);
        }

        // Does the player have a hot bar
        if (InventoryManager.m_gInstance.m_bHasHotbar)
        {
            // loop through each slot
            for (int i = 0; i < InventoryManager.m_gInstance.m_gPlayer.m_nHotbarSize; i++)
            {
                // build the inventory slots
                AddSlot(oPlayerInventory, i, m_gPrefab.GetComponentInChildren<Transform>().Find("Hotbar Inventory").transform);
            }
        }

        // else if there is no hotbar
        else
        {
            // disabled the background image for the hotbar area
            m_gPrefab.GetComponentInChildren<Transform>().Find("Hotbar Inventory").GetComponent<Image>().enabled = false;
        }
    }

    //--------------------------------------------------------------------------------------
    // GetPrefab: Get the container prefab for this container. 
    //
    // Return:
    //      GameObject: Returns the prefab of this container.
    //--------------------------------------------------------------------------------------
    public override GameObject GetPrefab()
    {
        // return the inventory managers instance container prefab
        return InventoryManager.m_gInstance.GetContainerPrefab("Chest Inventory");
    }
}