//--------------------------------------------------------------------------------------
// Purpose: Move the currently selected hotbar index.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------------------------------------------
// HotbarSelector Object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class HotbarSelector : MonoBehaviour
{
    // SELECTOR SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Selector Settings:")]

    // public list of gameboject for setting the selectors
    [LabelOverride("Selector Gameobjects")] [Tooltip("The gameobjects to use for making a selection on the hotbar.")]
    public List<GameObject> m_agSelectors = new List<GameObject>();

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private inventory manager instance.
    private InventoryManager m_gInventoryManger;

    // private player object for getting acces to player settings.
    private Player m_gPlayer;

    // private array of initial keycodes for selecting hotbar index.
    private KeyCode[] m_akInitHotbarControls = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, 
        KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };

    // private array of needed keycodes for selecting hotbar index. 
    private KeyCode[] m_akHotbarControls;

    // private int for the currently selected hotbar index.
    private int m_nCurrentlySelected = 0;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // Initialization.
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // get the player game object
        m_gPlayer = FindObjectOfType<Player>();

        // loop through all selectors
        for (int i = 0; i < m_agSelectors.Count; i++)
        {
            // disable all of the selector images
            m_agSelectors[i].GetComponent<Image>().enabled = false;

            // init the keycode array
            m_akHotbarControls = new KeyCode[m_agSelectors.Count];
        }

        // loop through all selectors
        for (int i = 0; i < m_agSelectors.Count; i++)
        {
            // set the keycode array to the amount of slots in the hotbar
            m_akHotbarControls[i] = m_akInitHotbarControls[i];
        }
    }

    //--------------------------------------------------------------------------------------
    // Initialization.
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        // get the inventory manager instance
        m_gInventoryManger = InventoryManager.m_gInstance;
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void Update()
    {
        // Update the currently selected item slot
        UpdateCurrentlySelected(Input.GetAxis("Mouse ScrollWheel"));
    }

    //--------------------------------------------------------------------------------------
    // UpdateCurrentlySelected: Move the currently selected hotbar index.
    //
    // Param:
    //      fDirection: float for what direction the scrollwheel should move.
    //--------------------------------------------------------------------------------------
    private void UpdateCurrentlySelected (float fDirection)
    {
        // loop through each button in the hotbar
        for (int i = 0; i < m_akHotbarControls.Length; i++)
        {
            // check if a key has been pressed on the hotbar
            if (Input.GetKeyDown(m_akHotbarControls[i]) && !m_gInventoryManger.IsInventoryOpen())
            {
                // assign currently selected to the key pressed
                m_nCurrentlySelected = i;
            }
        }

        // check if an inventory is currently open
        if (!m_gInventoryManger.IsInventoryOpen())
        {
            // check direction of the scrool wheel
            if (fDirection > 0)
                fDirection = 1;
            if (fDirection < 0)
                fDirection = -1;

            // move the selector
            for (m_nCurrentlySelected -= (int)fDirection; m_nCurrentlySelected < 0; m_nCurrentlySelected += m_agSelectors.Count) ;

            // make sure the index stay within horbar slots
            while (m_nCurrentlySelected >= m_agSelectors.Count)
            {
                // keep the currently selected at or under the current amount of itemslots
                m_nCurrentlySelected -= m_agSelectors.Count;
            }
        }

        // loop through all the selectors
        for (int i = 0; i < m_agSelectors.Count; i++)
        {
            // if the slot is currently selected
            if (i == m_nCurrentlySelected && !m_gInventoryManger.IsInventoryOpen())
            {
                // set the image of the selector to true
                m_agSelectors[m_nCurrentlySelected].GetComponent<Image>().enabled = true;

                // set the curently selected slot
                m_nCurrentlySelected = i;
            }

            // else if the slot is not currently selected
            else if (i != m_nCurrentlySelected)
            {
                // set the image of the selector to false
                m_agSelectors[i].GetComponent<Image>().enabled = false;
            }

            // else for whatever reason
            else
            {
                // set the image of the selector to false
                m_agSelectors[i].GetComponent<Image>().enabled = false;
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // GetCurrentlySelected: Get the currently selected slot on the hotbar.
    //
    // Return:
    //      int: an int for what the currently selected hotbar index is.
    //--------------------------------------------------------------------------------------
    public int GetCurrentlySelected()
    {
        // return the currently selected index
        return m_nCurrentlySelected;
    }

    //--------------------------------------------------------------------------------------
    // SetCurrentlySelected: Set what is currently selected on the hotbar.
    //
    // Param:
    //      nIndex: and int for the index to set the currently selected hotbar.
    //--------------------------------------------------------------------------------------
    public void SetCurrentlySelected(int nIndex)
    {
        // set the currently selected index
        m_nCurrentlySelected = nIndex;
    }

    //--------------------------------------------------------------------------------------
    // GetCurrentlySelectedItemStack: Get the currently selected item stack in the hotbar.
    //
    // Return:
    //      ItemStack: returns an item stack.
    //--------------------------------------------------------------------------------------
    public ItemStack GetCurrentlySelectedItemStack()
    {
        // return the currently selected item
        return m_gPlayer.GetInventory().GetStackInSlot(m_nCurrentlySelected);
    }
}