using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField] private Text worldToolTipUI;
    [SerializeField] private Text inventoryToolTipUI;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivityX;
    [SerializeField] private float mouseSensitivityY;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float gravity;
    [SerializeField] [Range(0.0f, 0.5f)] private float moveSmoothTime;
    [SerializeField] [Range(0.0f, 0.5f)] private float mouseSmoothTime;
    [SerializeField] private AnimationCurve jumpFallOff;
    [SerializeField] private float jumpMultiplier;
    // Range at which the player can interact with InventoryItems
    [SerializeField] private float pickupRange;
    [SerializeField] private float pickupMinDropDistance;
    [SerializeField] private float pickupMaxDropDistance;
    [SerializeField] private bool isInvHandSlotSelectReverse;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode pickUpKey = KeyCode.E;
    [SerializeField] private KeyCode inventoryKey = KeyCode.Tab;
    // Left-click to use items in hotbar
    [SerializeField] private KeyCode leftClickKey = KeyCode.Mouse0;
    // Right-click to use items inventory
    [SerializeField] private KeyCode rightClickKey = KeyCode.Mouse1;
    // Used for navigating inventory hand slots like the mouse scroll wheel
    [SerializeField] private KeyCode numpadOneKey = KeyCode.Keypad1;
    [SerializeField] private KeyCode numpadTwoKey = KeyCode.Keypad2;
    [SerializeField] private KeyCode numpadThreeKey = KeyCode.Keypad3;
    [SerializeField] private KeyCode numpadFourKey = KeyCode.Keypad4;

    private InventoryUI inventoryUI;

    private bool isMovementLocked = false;
    private bool isCameraMovementLocked = false;
    private bool isInventoryActive = false;

    private bool isCursorLocked = true;
    private float cameraPitch = 0.0f;
    private float velocityY = 0.0f;
    private bool isJumping;
    CharacterController playerController = null;

    private Vector2 currentDirection = Vector2.zero;
    private Vector2 currentDirVelocity = Vector2.zero;

    private Vector2 currentMouseDelta = Vector2.zero;
    private Vector2 currentMouseDeltaVelocity = Vector2.zero;

    #endregion Variables

    #region MonoBehaviours
    private void Start()
    {
        // Lets the character controller handle movement and collision, applies the value to this component
        playerController = GetComponent<CharacterController>();

        if (isCursorLocked)
        {
            // Locks the cursor to the center of the screen
            Cursor.lockState = CursorLockMode.Locked;

            // Makes the cursor invisible
            Cursor.visible = false;
        }

        inventoryUI = GetComponent<PlayerInventory>().GetInventoryUI();

        int selectedInvHandSlot = GetComponent<PlayerInventory>().GetSelectedInvHandSlot();
        Color background = inventoryUI.GetInvHandSelectedSlotUI()[selectedInvHandSlot].GetComponent<RawImage>().color;
        inventoryUI.GetInvHandSelectedSlotUI()[selectedInvHandSlot].GetComponent<RawImage>().color =
            new Color(background.r, background.g, background.b, .8f);
    }

    private void Update()
    {
        if (!isCameraMovementLocked)
        {
            UpdateMouseLook();
        }
        if (!isMovementLocked)
        {
            UpdateMovement();
        }
        if (isInventoryActive)
        {
            UpdateInventoryInteractions();
        }
        else
        {
            UpdateHotbarInteractions();
        }
        CheckForPickups();
        CheckOtherInputs();
    }

    #endregion MonoBehaviours

    #region KeyboardAndMouse
    private void UpdateMouseLook()
    {
        // Saves the x and y position of the mouse on the screen
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Smooths camera movement
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        // Saves the camera's vertical rotation as the inverse of mouse movement
        cameraPitch -= currentMouseDelta.y * mouseSensitivityY;

        // Guarentees the camera does not rotate beyond looking straight up or straight down
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        // Rotates the camera vertically
        playerCamera.transform.localEulerAngles = Vector3.right * cameraPitch;

        // Rotates the parent object left and right based on the mouse's x (horizontal) position
        this.gameObject.transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivityX);
    }

    private void UpdateMovement()
    {
        Vector3 playerVelocity = Vector3.zero;

        // Storing the axis
        Vector2 targetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Normalize the vector so every direction is cappped at the same speed
        targetDirection.Normalize();

        // Eases into movement so it's not instantaneous
        currentDirection = Vector2.SmoothDamp(currentDirection, targetDirection, ref currentDirVelocity, moveSmoothTime);

        // Reset the rate the player falls if they are touching the ground
        if (playerController.isGrounded)
        {
            velocityY = 0.0f;
        }

        // The downward acceleration
        velocityY += gravity * Time.deltaTime;

        // Sets the player's speed using the vectors scaled by their axis
        if (Input.GetKey(sprintKey))
        {
            playerVelocity = (transform.forward * currentDirection.y + transform.right * currentDirection.x)
                * sprintSpeed + Vector3.up * velocityY;
        }
        else
        {
            playerVelocity = (transform.forward * currentDirection.y + transform.right * currentDirection.x)
                * walkSpeed + Vector3.up * velocityY;
        }

        // The character controller will do its thing
        playerController.Move(playerVelocity * Time.deltaTime);

        JumpInput();
    }

    private void JumpInput()
    {
        // Player can jump
        if (Input.GetKeyDown(jumpKey) && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }
    }

    private IEnumerator JumpEvent()
    {
        playerController.slopeLimit = 90.0f;
        float timeInAir = 0.0f;

        do
        {
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            playerController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;
            yield return null;
            // Stops the player from going any higher if they hit a ceiling
        } while (!playerController.isGrounded && playerController.collisionFlags != CollisionFlags.Above);

        isJumping = false;
        playerController.slopeLimit = 45.0f;
    }

    public void CheckOtherInputs()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            ToggleInventoryUI();
        }

        // Using scroll wheel to change inventory hand slots
        if (Input.mouseScrollDelta.y < 0)
        {
            if (!isInvHandSlotSelectReverse)
            {
                ChangeSelectedHandSlot(true);
            }
            else
            {
                ChangeSelectedHandSlot(false);
            }
        }
        else if (Input.mouseScrollDelta.y > 0)
        {
            if (!isInvHandSlotSelectReverse)
            {
                ChangeSelectedHandSlot(false);
            }
            else
            {
                ChangeSelectedHandSlot(true);
            }
        }

        // Using number keys to change inventory hand slots
        if (Input.GetKeyDown("1") || Input.GetKeyDown(numpadOneKey))
        {
            ChangeSelectedHandSlot(0);
        }
        else if (Input.GetKeyDown("2") || Input.GetKeyDown(numpadTwoKey))
        {
            ChangeSelectedHandSlot(1);
        }
        else if (Input.GetKeyDown("3") || Input.GetKeyDown(numpadThreeKey))
        {
            ChangeSelectedHandSlot(2);
        }
        else if (Input.GetKeyDown("4") || Input.GetKeyDown(numpadFourKey))
        {
            ChangeSelectedHandSlot(3);
        }
    }

    public void ChangeSelectedHandSlot(int slotIndex)
    {
        // Get currently selected slot
        int selectedInvHandSlot = GetComponent<PlayerInventory>().GetSelectedInvHandSlot();

        // Make currently selected slot lose its highlight
        Color background = inventoryUI.GetInvHandSelectedSlotUI()[selectedInvHandSlot].GetComponent<RawImage>().color;
        inventoryUI.GetInvHandSelectedSlotUI()[selectedInvHandSlot].GetComponent<RawImage>().color =
            new Color(background.r, background.g, background.b, 0f);

        // Change selected slot
        GetComponent<PlayerInventory>().SetSelectedInvHandSlot(slotIndex);

        // Make new selected slot highlighted
        background = inventoryUI.GetInvHandSelectedSlotUI()[slotIndex].GetComponent<RawImage>().color;
        inventoryUI.GetInvHandSelectedSlotUI()[slotIndex].GetComponent<RawImage>().color =
            new Color(background.r, background.g, background.b, .8f);
    }

    public void ChangeSelectedHandSlot(bool isMovingRight)
    {
        // Get currently selected slot
        int selectedInvHandSlot = GetComponent<PlayerInventory>().GetSelectedInvHandSlot();

        // Make currently selected slot lose its highlight
        Color background = inventoryUI.GetInvHandSelectedSlotUI()[selectedInvHandSlot].GetComponent<RawImage>().color;
        inventoryUI.GetInvHandSelectedSlotUI()[selectedInvHandSlot].GetComponent<RawImage>().color =
            new Color(background.r, background.g, background.b, 0f);

        if (!isMovingRight)
        {
            // Change selected slot
            GetComponent<PlayerInventory>().MoveSelectedInvHandSlotLeft();
        }
        else
        {
            GetComponent<PlayerInventory>().MoveSelectedInvHandSlotRight();
        }

        // Get newly selected slot
        selectedInvHandSlot = GetComponent<PlayerInventory>().GetSelectedInvHandSlot();
        
        // Make new selected slot highlighted
        background = inventoryUI.GetInvHandSelectedSlotUI()[selectedInvHandSlot].GetComponent<RawImage>().color;
        inventoryUI.GetInvHandSelectedSlotUI()[selectedInvHandSlot].GetComponent<RawImage>().color =
            new Color(background.r, background.g, background.b, .8f);
    }

    #endregion KeyboardAndMouse

    #region PickupsAndInventory
    private void CheckForPickups()
    {
        Vector3 cameraDirection = playerCamera.transform.forward;

        // Bit shift the index of layer 6 to get a bit mask
        int layerMask = 1 << 6;

        // Inverse the mask to ignore anything in layer 6 (ignore the player)
        layerMask = ~layerMask;

        Ray ray = new Ray(playerCamera.transform.position, cameraDirection);
        RaycastHit hit = new RaycastHit();

        bool isHitSuccess = Physics.Raycast(ray.origin, ray.direction, out hit, pickupRange, layerMask);

        Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.5f);

        worldToolTipUI.text = "";

        if (!isHitSuccess)
        {
            return;
        }

        InventoryItem hitInventoryItem = hit.collider.gameObject.GetComponent<InventoryItem>();
        PlayerInventory inventory = this.gameObject.GetComponent<PlayerInventory>();

        if (hitInventoryItem == null || inventory == null)
        {
            return;
        }

        worldToolTipUI.text = hitInventoryItem.GetItemCount() + " " + hitInventoryItem.GetItem();

        // If there is not a single empty slot in the player's inventory
        if (GetComponent<PlayerInventory>().IsInventoryFull())
        {
            // If they have a non-max stack of the same item, pick up as much as possible
            InventoryItem itemToTransferTo = GetComponent<PlayerInventory>().HasSameItemOfNonMaxStackSize(hitInventoryItem);

            // Must recieve empty InvItem because itemToTransferTo == null will always equal null
            if (itemToTransferTo.GetItem() == InventoryItem.Item.None)
            {
                worldToolTipUI.text = "Inventory Full";
                return;
            }
            else if (Input.GetKeyDown(pickUpKey))
            {
                // Pass item count from the object in the world to the object in the inventory
                int newAmount = itemToTransferTo.GetItemCount() + hitInventoryItem.GetItemCount();

                // Can't transfer the whole stack size
                if (newAmount > itemToTransferTo.GetMaxStackSize())
                {
                    itemToTransferTo.SetItemCount(itemToTransferTo.GetMaxStackSize());
                    hitInventoryItem.SetItemCount(newAmount - itemToTransferTo.GetMaxStackSize());
                }
                // Can transfer everything
                else
                {
                    itemToTransferTo.SetItemCount(newAmount);
                    Destroy(hitInventoryItem.gameObject);
                }

                inventory.RefreshInventoryVisuals();
                return;
            }
        }

        // Player can pick the item up no problem
        if (Input.GetKeyDown(pickUpKey))
        {
            hitInventoryItem.PickItemUp(inventory);
            Destroy(hitInventoryItem.gameObject);

            inventory.RefreshInventoryVisuals();
        }
    }

    public void ToggleInventoryUI()
    {
        isInventoryActive = !isInventoryActive;

        if (isInventoryActive)
        {
            foreach (GameObject obj in inventoryUI.GetInvSlotsUI())
            {
                obj.gameObject.SetActive(true);
            }
            foreach (GameObject obj in inventoryUI.GetInvItemsUI())
            {
                obj.gameObject.SetActive(true);
            }
            foreach (GameObject obj in inventoryUI.GetInvArmorSlotsUI())
            {
                obj.gameObject.SetActive(true);
            }
            foreach (GameObject obj in inventoryUI.GetInvArmorItemsUI())
            {
                obj.gameObject.SetActive(true);
            }

            inventoryUI.GetInvItemDiscardUI().SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            isCameraMovementLocked = true;
        }
        else
        {
            inventoryToolTipUI.text = "";

            foreach (GameObject obj in inventoryUI.GetInvSlotsUI())
            {
                obj.gameObject.SetActive(false);
            }
            foreach (GameObject obj in inventoryUI.GetInvItemsUI())
            {
                obj.gameObject.SetActive(false);
            }
            foreach (GameObject obj in inventoryUI.GetInvArmorSlotsUI())
            {
                obj.gameObject.SetActive(false);
            }
            foreach (GameObject obj in inventoryUI.GetInvArmorItemsUI())
            {
                obj.gameObject.SetActive(false);
            }

            inventoryUI.GetInvItemDiscardUI().SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            isCameraMovementLocked = false;
        }    
    }
    
    // This function updates the Inventory tooltip and lets the player use items from their inventory
    public void UpdateInventoryInteractions()
    {
        int invItemIndex = GetInvItemIndexFromMouse();
        bool isInvItemIndexInvHand = IsInvItemIndexInvHand();
        bool isInvArmorItem = IsInvArmorItem();
        
        inventoryToolTipUI.text = "";

        if (invItemIndex == -1)
        {
            return;
        }

        PlayerInventory playerInventory = this.gameObject.GetComponent<PlayerInventory>();
        List<InventoryItem> invItems = playerInventory.GetInvItemList();
        List<InventoryItem> invHandItems = playerInventory.GetInvHandItemList();
        List<InventoryItem> invArmorItems = playerInventory.GetInvItemArmorList();
        InventoryItem invItem = null;

        if (isInvItemIndexInvHand)
        {
            invItem = invHandItems[invItemIndex];
        }
        else if (isInvArmorItem)
        {
            invItem = invArmorItems[invItemIndex];
        }
        else
        {
            invItem = invItems[invItemIndex];
        }

        inventoryToolTipUI.gameObject.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y + 25,
                Input.mousePosition.z);

        if (invItem.GetItem() == InventoryItem.Item.None)
        {
            inventoryToolTipUI.text = "";
            return;
        }
        else
        {
            inventoryToolTipUI.text = invItem.GetItem().ToString();
        }

        // Right click to use consumables
        if (Input.GetKeyDown(rightClickKey) && invItem.GetItemType() == InventoryItem.ItemType.Consumable)
        {
            bool wasConsumed = invItem.PrimaryAction(GetComponent<PowerupManager>());
            int itemCount = invItem.GetItemCount();

            if (wasConsumed == false)
            {
                // Do nothing
            }
            else if (itemCount <= 1)
            {
                playerInventory.RemoveFromInventory(invItemIndex, isInvItemIndexInvHand, isInvArmorItem);
            }
            else
            {
                invItem.SetItemCount(itemCount - 1);
            }

            playerInventory.RefreshInventoryVisuals();
        }
    }

    // Only runs when Inventory is not open, handles using items in inventory hand slots
    public void UpdateHotbarInteractions()
    {
        if (Input.GetKeyDown(leftClickKey))
        {
            bool wasConsumed = false;

            // Get player inventory
            PlayerInventory playerInventory = GetComponent<PlayerInventory>();

            // Get the index of the selected inv hand slot
            int selectedInvHandSlot = playerInventory.GetSelectedInvHandSlot();

            // Get the player's InvHandItemList
            List<InventoryItem> invHandItemList = playerInventory.GetInvHandItemList();

            // Get the item at that index in the player's InvHandItemList
            InventoryItem invItem = invHandItemList[selectedInvHandSlot];

            // Use the inventory item's primary action
            if (invItem.GetItemType() == InventoryItem.ItemType.Consumable)
            {
                wasConsumed = invItem.PrimaryAction(GetComponent<PowerupManager>());
                int itemCount = invItem.GetItemCount();

                if (wasConsumed == false)
                {
                    // Do nothing
                }
                else if (itemCount <= 1)
                {
                    playerInventory.RemoveFromInventory(selectedInvHandSlot, true, false);
                }
                else
                {
                    invItem.SetItemCount(itemCount - 1);
                }

                playerInventory.RefreshInventoryVisuals();
            }
            else if (invItem.GetItemType() == InventoryItem.ItemType.Weapon)
            {
                invItem.PrimaryAction();
            }
        }

        if (Input.GetKeyDown(rightClickKey))
        {
            
        }
    }

    public int GetInvItemIndexFromMouse()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (raycastResults[i].gameObject.GetComponent<IndexValue>() != null)
            {
                return raycastResults[i].gameObject.GetComponent<IndexValue>().GetIndexValue();
            }
        }
        
        return -1;
    }

    public bool IsInvItemIndexInvHand()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (raycastResults[i].gameObject.GetComponent<IsInvHandItem>())
            {
                return true;
            }
        }

        return false;
    }

    public bool IsInvArmorItem()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (raycastResults[i].gameObject.GetComponent<IsInvArmorItem>())
            {
                return true;
            }
        }

        return false;
    }

    #endregion PickupsAndInventory

    #region GetSet
    public float GetPickupMinDropDistance()
    {
        return pickupMinDropDistance;
    }

    public float GetPickupMaxDropDistance()
    { 
        return pickupMaxDropDistance;
    }

    #endregion GetSet
}