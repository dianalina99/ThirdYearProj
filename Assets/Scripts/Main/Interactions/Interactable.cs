using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;

    bool isFocus = false, hasInteracted = false;
    Transform player;

    public virtual void Interact ()
    {
        //Method made to be overwritten.
        Debug.Log("Interactable: Interacting with " + player.name);
    }

    public virtual void FinishInteract()
    {
        //Method made to be overwritten.
        Debug.Log("Interactable: Finish interacting with " + player.name);
    }

    private void Update()
    {
        if(isFocus && !hasInteracted)
        {
            float distance = Vector3.Distance(player.position, transform.position);

            if(distance <= radius)
            {
                Interact();
                hasInteracted = true;
            }
        }
    }

    public void OnFocused(Transform playerTransform)
    {
        isFocus = true;
        player = playerTransform;
        hasInteracted = false;
    }

    public void OnDefocused()
    {
        FinishInteract();

        isFocus = false;
        player = null;
        hasInteracted = false;
    }
        

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
