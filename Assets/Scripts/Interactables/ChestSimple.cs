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
using UnityEngine.UI;

//--------------------------------------------------------------------------------------
// Chest object. Inheriting from Interactable.
//--------------------------------------------------------------------------------------
public class ChestSimple : Interactable
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

    // REMOVE // TEMP // REMOVE // POSSIBLTY //
    // an array to add some items to the inventory
    public Item[] itemsToAdd;
    public GameObject ItemText1;
    public GameObject ItemText2;
    public float m_fTimer = 0;
    // REMOVE // TEMP // REMOVE // POSSIBLTY //

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
        // REMOVE // TEMP // REMOVE // POSSIBLTY //
        // for each item in items to add
        foreach (Item i in itemsToAdd)
        {
            // add an item to the inventory
            m_oInventory.AddItem(new ItemStack(i, 1));
        }
        // REMOVE // TEMP // REMOVE // POSSIBLTY //

        // get the inventory instance
        m_gInventoryManger = InventoryManager.m_gInstance;
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void Update()
    {
        // tick the timer
        m_fTimer += Time.deltaTime;

        // If the interaction button or exc is pressed.
        if (m_fTimer > 1.8 && m_bInteracted)
        {
            // turn off interaction and set chest to closed
            m_bInteracted = false;
            m_bInteractable = false;

            // set the closed chest image
            gameObject.GetComponent<SpriteRenderer>().sprite = m_sClosedImage;
        }

        // if the timer goes above 5
        if (m_fTimer > 5 && !m_bInteracted)
        {
            // diabled item obtained text
            ItemText1.GetComponent<Text>().enabled = false;
            ItemText2.GetComponent<Text>().enabled = false;
        }
    }

    //--------------------------------------------------------------------------------------
    // OpenCloseInventory: Open/Close the Inventory container of the chest.
    //--------------------------------------------------------------------------------------
    private void GetChestContents()
    {   
        // loop through each item stack in the inventory
        foreach (ItemStack i in m_oInventory.GetInventory())
        {
            // if the stack in the slot isnt empty
            if (!m_oInventory.GetStackInSlot(i).IsStackEmpty())
            {
                // enabled the item obtained text
                ItemText1.GetComponent<Text>().enabled = true;
                ItemText2.GetComponent<Text>().enabled = true;
            }

            // add an item to the inventory
            InventoryManager.m_gInstance.m_gPlayer.GetInventory().AddItem(m_oInventory.GetStackInSlot(i));

            // remove the item from the chest
            m_oInventory.GetStackInSlot(i).SetStack(new ItemStack());
        }

        // if the item text 1 is false
        if (ItemText1.GetComponent<Text>().enabled == false)
        {
            // set the text to the chest being empty
            ItemText1.GetComponent<Text>().text = "The chest is empty.";
            ItemText1.GetComponent<Text>().enabled = true;
        }

        // set the open chest image
        gameObject.GetComponent<SpriteRenderer>().sprite = m_sOpenImage;

        // reset the timer
        m_fTimer = 0;
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
        GetChestContents();
    }
}