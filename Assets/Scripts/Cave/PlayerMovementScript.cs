using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D player;
    public Animator animator;
    public Camera cam;
    public LayerMask interactablesMask, collectiblesMask;

    Vector2 movement;
    public Interactable focus;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if(movement.x < 0.0f)
        {
            //Flip sprite
            this.transform.localScale = new Vector3(-1, this.transform.localScale.y, this.transform.localScale.z);
        }
        else
        {
            this.transform.localScale = new Vector3(1, this.transform.localScale.y, this.transform.localScale.z);
        }

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("speed", movement.sqrMagnitude);


        //Handle mouse inputs.
        HandleInteractables();

        //Collecting items is managed by ItemCollect.cs which extends Collecticle.cs.
  
    }

    private void HandleInteractables()
    {
        //Focus on interactable objects on right click.
        if (Input.GetMouseButtonDown(1))
        {
            //Create a ray.
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100, interactablesMask);

            //Check is we hit interactable. 
            if (hit.collider != null)
            {
                Debug.Log("We hit" + hit.collider.name + " " + hit.point);
                Interactable interactable = hit.collider.GetComponent<Interactable>();

                if (interactable != null)
                {
                    SetFocus(interactable);
                }

            }
        }

        //Unfocus when moving too far away.
        float distance = 0;

        if(focus != null)
        {
           distance = Vector3.Distance(this.gameObject.transform.position, focus.transform.position);
           
            if(distance > focus.radius)
            {
                RemoveFocus();
            }
        }

        //Unfocus interactables when hitting ESC.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RemoveFocus();
        }
    }

    void SetFocus(Interactable newFocus)
    {
        if( newFocus != focus)
        {
            if(focus != null)
            {
                focus.OnDefocused();
            }
            
            focus = newFocus;
        }
        
        newFocus.OnFocused(transform);
    }

    void RemoveFocus()
    {
        if(focus != null)
        {
            focus.OnDefocused();
        }

        focus = null;
        
    }


    void FixedUpdate()
    {
        player.MovePosition(player.position + movement * speed * Time.fixedDeltaTime);
        
    }
}
