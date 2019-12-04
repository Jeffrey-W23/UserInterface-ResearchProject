//--------------------------------------------------------------------------------------
// Purpose: The main logic for the enemy interactable object.
//
// Description: This script is used for the main logic of the enemy as well as opening 
// and closing the enemy inventory on interaction.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------------------------------------------
// EnemyComplex object. Inheriting from Interactable.
//--------------------------------------------------------------------------------------
public class EnemyComplex : Interactable
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
    public float m_fHealth = 0.08f;
    public Transform m_tHealthBar;
    public GameObject m_fHealthBarBack;
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

        // set interactable to true to stop intial interaction
        m_bHoldInteraction = true;
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
        // If the interaction button or exc is pressed.
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            // turn off interaction and set chest to closed
            m_bInteracted = false;
            m_bInteractable = false;
        }

        // move the health bar with the health value
        m_tHealthBar.gameObject.GetComponent<SpriteRenderer>().size = new Vector2(m_fHealth, 0.02f);

        // if the health is lower than 0
        if (m_fHealth <= 0)
        {
            // turn off the health bar
            m_tHealthBar.gameObject.GetComponent<SpriteRenderer>().size = new Vector2(0, 0);
            m_fHealthBarBack.SetActive(false);

            // turn off the enemy box collider
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            // set the open chest image
            gameObject.GetComponent<SpriteRenderer>().sprite = m_sOpenImage;

            // stop interaction from happening
            m_bHoldInteraction = false;
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

    //--------------------------------------------------------------------------------------
    // OnCollisionEnter2D: When this object collides with another object call this function.
    //
    // Param:
    //      cObject: a Collision2D for what object has had a collision.
    //--------------------------------------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D cObject)
    {
        // if a bullet collides with the enemy
        if (cObject.collider.tag == "Pistol Bullet")
            m_fHealth -= 0.01f;
    }
}