//--------------------------------------------------------------------------------------
// Purpose: The main logic for the Player object.
//
// Description: This script will handled most of the typically work that a player has to
// do like movement, interaction, opening menus, etc.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Player object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class Player : MonoBehaviour
{
    // MOVEMENT //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Movement:")]

    // public float value for the walking speed.
    [LabelOverride("Walking Speed")] [Tooltip("The speed of which the player will walk in float value.")]
    public float m_fWalkSpeed = 5.0f;

    // public float value for the walking speed.
    [LabelOverride("Running Speed")] [Tooltip("The speed of which the player will run in float value.")]
    public float m_fRunSpeed = 7.0f;

    // public float value for max exhaust level.
    [LabelOverride("Running Exhaust")] [Tooltip("The max level of exhaustion the player can handle before running is false.")]
    public float m_fRunExhaust = 3.0f;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // INVENTORY //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Inventory Settings:")]

    // public int for the inventory size
    [LabelOverride("Inventory Size")] [Tooltip("The size of the Inventory for the player object.")]
    public int m_nInventorySize = 9;

    // public int for the hotbar inventory size
    [LabelOverride("Hotbar Size")] [Tooltip("The size of the Hotbar for the player object.")]
    public int m_nHotbarSize = 3;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // DEBUG //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Debug:")]

    // public bool for turning the debug info off and on.
    [LabelOverride("Display Debug Info?")] [Tooltip("Turns off and on debug information in the unity console.")]
    public bool m_bDebugMode = true;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private rigidbody.
    private Rigidbody2D m_rbRigidBody;

    // private float for the current speed of the player.
    private float m_fCurrentSpeed;

    // private flkoat for the current exhaust level of the player.
    private float m_fRunCurrentExhaust = 0.0f;

    // private bool for if ther player can run or not
    private bool m_bExhausted = false;

    // private gameobject for the players arm object
    private GameObject m_gArm;

    // private float value for distance between mouse and arm
    private float m_fArmDistanceFromMouse;

    // private inventory for the player object
    private Inventory m_oInventory;

    // private inventory manager for the inventory manager instance
    private InventoryManager m_gInventoryManger;

    // private bool for freezing the player
    private bool m_bFreezePlayer = false;
    //--------------------------------------------------------------------------------------

    // DELEGATES //
    //--------------------------------------------------------------------------------------
    // Create a new Delegate for handling the interaction functions.
    public delegate void InteractionEventHandler();

    // Create an event for the delegate for extra protection. 
    public InteractionEventHandler InteractionCallback;
    //--------------------------------------------------------------------------------------

    // REMOVE // TEMP // REMOVE // POSSIBLTY //
    // Weapon prefab.
    public GameObject m_gWeaponPrefab;

    // The Pistol weapon.
    private GameObject m_gPistol;

    // an array to add some items to the inventory
    public Item[] itemsToAdd;

    //
    public bool m_bSimpleDemo = false;

    //
    public GameObject m_gGunDisplay;

    //
    public GameObject m_gFistDisplay;

    //
    public Inventory m_oArmourInventory;
    // REMOVE // TEMP // REMOVE // POSSIBLTY //

    //--------------------------------------------------------------------------------------
    // Initialization
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // set the inventory of the player
        m_oInventory = new Inventory(m_nInventorySize);

        // Get the Rigidbody.
        m_rbRigidBody = GetComponent<Rigidbody2D>();

        // get the player arm object.
        m_gArm = transform.Find("Arm").gameObject;

        // set the current speed of the player to walk
        m_fCurrentSpeed = m_fWalkSpeed;

        // REMOVE // TEMP // REMOVE // POSSIBLTY //
        // Set the parenting of pistol prefab.
        m_gPistol = Instantiate(m_gWeaponPrefab);
        m_gPistol.transform.parent = m_gArm.transform;
        m_gPistol.SetActive(false);

        m_oArmourInventory = new Inventory(2);
        // REMOVE // TEMP // REMOVE // POSSIBLTY //
    }

    //--------------------------------------------------------------------------------------
    // Initialization
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

        // get the inventory manager instance
        m_gInventoryManger = InventoryManager.m_gInstance;

        // open player hotbar if there is one
        if (InventoryManager.m_gInstance.m_bHasHotbar)
            m_gInventoryManger.OpenContainer(new PlayerHotbarContainer(null, m_oInventory, m_nHotbarSize));

        // make sure the inventory is not open
        m_gInventoryManger.ResetInventoryStatus();
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void Update()
    {
        // Run the interaction function
        Interaction();

        // Open and close the inventory system
        OpenCloseInventory();

        //
        if (!m_bSimpleDemo)
        {
            // make sure the hotbar selector isnt null and the current selected item isnt null
            if (FindObjectOfType<HotbarSelector>() != null && FindObjectOfType<HotbarSelector>().GetCurrentlySelectedItemStack().GetItem() != null)
            {
                // if the hotbar selector has the gun pistol currently selected turn the object on and off
                if (FindObjectOfType<HotbarSelector>().GetCurrentlySelectedItemStack().GetItem().m_strTitle == "Pistol")
                {
                    m_gPistol.SetActive(true);
                    CustomCursor.m_gInstance.SetCrosshair();
                }
                else
                {
                    m_gPistol.SetActive(false);
                    CustomCursor.m_gInstance.SetDefaultCursor();
                }
            }

            // else if the currently selected item is null or the hotbar selector is null 
            else
            {
                // set the pistol to unactive
                m_gPistol.SetActive(false);
                CustomCursor.m_gInstance.SetDefaultCursor();
            }
        }

        //
        else
        {
            //
            for (int i = 0; i < m_oInventory.GetInventory().Count; i++)
            {
                //
                if (m_oInventory.GetStackInSlot(i).GetItem() != null)
                {
                    //
                    if (m_oInventory.GetStackInSlot(i).GetItem().m_strTitle == "Pistol")
                    { 
                        //
                        if (Input.GetKeyDown(KeyCode.Alpha1))
                        {
                            //
                            m_gPistol.SetActive(true);

                            //
                            m_gGunDisplay.SetActive(true);
                            m_gFistDisplay.SetActive(false);

                            //
                            CustomCursor.m_gInstance.SetCrosshair();
                        }

                        //
                        if (Input.GetKeyDown(KeyCode.Alpha2))
                        {
                            //
                            m_gPistol.SetActive(false);

                            //
                            m_gGunDisplay.SetActive(false);
                            m_gFistDisplay.SetActive(true);

                            //
                            CustomCursor.m_gInstance.SetDefaultCursor();
                        }
                    }
                }
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // FixedUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void FixedUpdate()
    {
        // is player allowed to move
        if (!m_bFreezePlayer)
        {
            // rotate player based on mouse postion.
            Rotate();

            // run the movement function to move player.
            Movement();
        } 
    }

    //--------------------------------------------------------------------------------------
    // LateUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void LateUpdate()
    {
        // move arm with mouse movement if the player is allowed to move
        if (!m_bFreezePlayer)
            MoveArm();
    }

    //--------------------------------------------------------------------------------------
    // Movement: Move the player rigidbody, for both walking and sprinting.
    //--------------------------------------------------------------------------------------
    private void Movement()
    {
        // Get the Horizontal and Vertical axis.
        Vector3 v2MovementDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f).normalized;

        // Move the player
        m_rbRigidBody.MovePosition(transform.position + v2MovementDirection * m_fCurrentSpeed * Time.fixedDeltaTime);

        // if the players holds down left shift
        if (Input.GetKey(KeyCode.LeftShift) && !m_bExhausted)
        {
            // current player speed equals run speed
            m_fCurrentSpeed = m_fRunSpeed;

            // tick the current exhaust level up 
            m_fRunCurrentExhaust += Time.deltaTime;

            // if the current exhaust is above the max
            if (m_fRunCurrentExhaust > m_fRunExhaust)
            {
                // player is exhausted and speed is now walking.
                m_bExhausted = true;
                m_fCurrentSpeed = m_fWalkSpeed;
            }
        }

        // else if shift isnt down
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            // current speed is walking speed.
            m_fCurrentSpeed = m_fWalkSpeed;

            // tick the current exhaust level down 
            m_fRunCurrentExhaust -= Time.deltaTime;

            // if the current exhaust is below 0 keep at 0
            if (m_fRunCurrentExhaust < 0.0f)
                m_fRunCurrentExhaust = 0.0f;

            // if the current exhaust is below the max then exhausted false
            if (m_fRunCurrentExhaust < m_fRunExhaust)
                m_bExhausted = false;
        }
    }

    //--------------------------------------------------------------------------------------
    // Roatate: Rotate the player from the mouse movement.
    //--------------------------------------------------------------------------------------
    private void Rotate()
    {
        // Get mouse inside camera 
        Vector3 v3Position = Camera.main.WorldToScreenPoint(transform.position);

        // Get the  mouse direction.
        Vector3 v3Direction = Input.mousePosition - v3Position;

        // Work out the angle.
        float fAngle = Mathf.Atan2(v3Direction.y, v3Direction.x) * Mathf.Rad2Deg;

        // Update the rotation.
        transform.rotation = Quaternion.AngleAxis(fAngle, Vector3.forward);
    }

    //--------------------------------------------------------------------------------------
    // MoveArm: Move the player objects arm towards the camera.
    //--------------------------------------------------------------------------------------
    private void MoveArm()
    {
        // Get mouse pos inside camera
        Vector3 v3Position = Camera.main.WorldToScreenPoint(m_gArm.transform.position);

        // update the distance arm has from mouse.
        m_fArmDistanceFromMouse = Vector3.Distance(v3Position, Input.mousePosition);

        // Check the distance between the mouse and arm.
        // if far enough away turn the mouse towards mouse.
        // else stop arm rotation.
        if (m_fArmDistanceFromMouse > 100)
        {
            // Get the  mouse direction.
            Vector3 v3Dir = Input.mousePosition - v3Position;

            // Work out the angle.
            float fAngle = Mathf.Atan2(v3Dir.y, v3Dir.x) * Mathf.Rad2Deg;

            // Update the rotation of the arm.
            m_gArm.transform.rotation = Quaternion.AngleAxis(fAngle, Vector3.forward);
        }
    }

    //--------------------------------------------------------------------------------------
    // OpenCloseInventory: Open/Close the Inventory container of the player object.
    //--------------------------------------------------------------------------------------
    private void OpenCloseInventory()
    {
        // if the i key is down
        if (Input.GetKeyDown(KeyCode.I))
        {
            // if the inventory bool is false
            if (!m_gInventoryManger.IsInventoryOpen())
            {
                // Open the player inventory
                m_gInventoryManger.OpenContainer(new PlayerContainer(null, m_oInventory, m_nInventorySize - m_nHotbarSize));
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // GetFreezePlayer: Get the current freeze status of the player. 
    //
    // Return:
    //      bool: the current freeze staus of the player.
    //--------------------------------------------------------------------------------------
    public bool GetFreezePlayer()
    {
        // get the player freeze bool
        return m_bFreezePlayer;
    }

    //--------------------------------------------------------------------------------------
    // GetInventory: Get the inventory of the player object.
    //
    // Return:
    //      Inventory: the inventory of the player object.
    //--------------------------------------------------------------------------------------
    public Inventory GetInventory()
    {
        // return the player inventory
        return m_oInventory;
    }

    //--------------------------------------------------------------------------------------
    // SetFreezePlayer: Set the freeze status of the player object. Used for ensuring the
    // player stays still, good for open menus or possibly cut scenes, etc.
    //
    // Param:
    //      bFreeze: bool for setting the freeze status.
    //--------------------------------------------------------------------------------------
    public void SetFreezePlayer(bool bFreeze)
    {
        // set the player freeze bool
        m_bFreezePlayer = bFreeze;

        // make sure the player is forzen further by constricting ridgidbody
        if (bFreeze)
            m_rbRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        else if (!bFreeze)
            m_rbRigidBody.constraints = RigidbodyConstraints2D.None;

    }

    //--------------------------------------------------------------------------------------
    // Interaction: Function interacts on button press with interactables objects.
    //--------------------------------------------------------------------------------------
    public void Interaction()
    {
        // If the interaction button is pressed.
        if (Input.GetKeyUp(KeyCode.E) && InteractionCallback != null)
        {
            // Run interaction delegate.
            InteractionCallback();
        }
    }
}