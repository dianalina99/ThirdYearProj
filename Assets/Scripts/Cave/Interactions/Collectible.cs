using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float radius = 3f;

    bool hasInteracted = false;
    Transform player;


    public virtual void Interact()
    {
        //Method made to be oberwritten.
        Debug.Log("Collectible: Interacting with " + player.name);
    }

    private void Update()
    {
        if (!hasInteracted && Input.GetKeyDown(KeyCode.Z))
        {
            player = GameObject.Find("Player(Clone)").transform;

            float distance = Vector3.Distance(player.position, transform.position);

            if (distance <= radius)
            {
                Interact();
                hasInteracted = true;
            }
        }
    }

 
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
