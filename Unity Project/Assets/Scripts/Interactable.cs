using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string promptMessage;

    //this will be called by our player
    public void BaseInteract()
    {
        Interact();
    }
    protected virtual void Interact()
    {
        //meant to be overridden by subclasses
    }
}
