//--------------------------------------------------------------------------------------
// Purpose: The main logic for the chest interactable object.
//
// Description: This script is used for opening and closing the chest on interaction.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Chest object. Inheriting from Interactable.
//--------------------------------------------------------------------------------------
public class Chest : Interactable
{
    // INVENTORY //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Inventory Settings:")]

    // public int for the inventory size
    [LabelOverride("Size")] [Tooltip("The size of the Inventory in the chest object.")]
    public int m_nInventorySize = 6;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // SPRITE //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Sprite Settings:")]

    //
    [LabelOverride("Chest Open Image")] [Tooltip("The image to set the chest object sprite when the chest is open.")]
    public Sprite m_sOpenImage;

    //
    [LabelOverride("Chest Closed Image")] [Tooltip("The image to set the chest object sprite when the chest is closed.")]
    public Sprite m_sClosedImage;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PUBLIC HIDDEN VALUES //
    //--------------------------------------------------------------------------------------
    // public inventory object for the chest inventory
    [HideInInspector]
    public Inventory m_oInventory;
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private inventory manager object for getting the inventory manager intstance
    private InventoryManager m_gInventoryManger;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    new void Awake()
    {
        // set the inventory of the chest
        m_oInventory = new Inventory(m_nInventorySize);

        // Run the base awake
        base.Awake();
    }

    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        // get the inventory instance
        m_gInventoryManger = InventoryManager.m_gInstance;
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void Update()
    {
        // If the interaction button or exc is pressed.
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            // turn off interaction and set chest to closed
            m_bInteracted = false;
            m_bInteractable = false;

            // set the closed chest image
            gameObject.GetComponent<SpriteRenderer>().sprite = m_sClosedImage;
        }
    }

    //--------------------------------------------------------------------------------------
    // OpenCloseInventory: Open/Close the Inventory container of the chest.
    //--------------------------------------------------------------------------------------
    private void OpenCloseInventory()
    {
        // Open the chest inventory
        if (!InventoryManager.m_gInstance.IsInventoryOpen())
            InventoryManager.m_gInstance.OpenContainer(new ChestContainer(m_oInventory, m_sPlayerObject.GetInventory(), m_nInventorySize));

        // set the open chest image
        gameObject.GetComponent<SpriteRenderer>().sprite = m_sOpenImage;
    }

    //--------------------------------------------------------------------------------------
    // InteractedWith: override function from base class for what Interactable objects do 
    // once they have been interacted with.
    //--------------------------------------------------------------------------------------
    protected override void InteractedWith()
    {
        // Run the base interactedWith function.
        base.InteractedWith();

        // open the chest
        OpenCloseInventory();
    }
}