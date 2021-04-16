using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D player;
    public Animator animator;
    public Camera cam;
    public LayerMask interactablesMask, collectiblesMask, enemyLayer;
    public float attackRange;

    Vector2 movement;
    public Interactable focus;

    private void Start()
    {
        //Save reference to the camera.
        cam = Camera.main;
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if(movement.x < 0.0f)
        {
            //Flip sprite.
            this.transform.localScale = new Vector3(-1, this.transform.localScale.y, this.transform.localScale.z);
        }
        else
        {
            this.transform.localScale = new Vector3(1, this.transform.localScale.y, this.transform.localScale.z);
        }

        //Make sprite have the correct orientation when pressing the move keys.
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("speed", movement.sqrMagnitude);


        //Handle mouse inputs.
        HandleInteractables();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            // StartCoroutine("AttackWithCooldown");
            Attack();
        }
  
    }

    private void Attack()
    {
        //Play attack animation
        animator.SetTrigger("Attack");

        //Detect enemies in range.
        Collider2D[] enemies =  Physics2D.OverlapCircleAll(this.transform.position, attackRange, enemyLayer );

        //Damage enemies detected previously.
        foreach(Collider2D enemy in enemies)
        {
                enemy.gameObject.GetComponent<EnemyStats>().TakeDamage(GameManagerScript.instance.playerRef.GetComponent<PlayerStats>().strength.GetValue());
        }

        //Cooldown for half a second.

    }

    private IEnumerator AttackWithCooldown()
    {
        Attack();
        return new WaitForSecondsRealtime(2f);
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

        //Unfocus interactables when hitting CTRL.
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
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
