using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteractable : Interactable
{
    public GameObject inventoryUI;

    public override void Interact()
    {
        base.Interact();

        //Show chest inventory.
        inventoryUI.SetActive(!inventoryUI.activeSelf);
        Inventory.instance.interactingWithChest = this.gameObject.transform;

    }

    public override void FinishInteract()
    {
        base.FinishInteract();

        inventoryUI.SetActive(false);
        Inventory.instance.interactingWithChest = null;
    }


}
