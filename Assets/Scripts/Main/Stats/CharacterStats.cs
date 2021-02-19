
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth { get; private set; }
   
    public Stat strength;
    public Stat protection;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }
    }

    //Overwritten by player.
    public virtual void TakeDamage(int damage)
    {
        damage -= protection.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHealth -= damage;
       
        Debug.Log(transform.name + " takes" + damage + " damage.");

        if(currentHealth <= 0 )
        {
            Die();
        }
    }

    public virtual void IncreaseHealth(int amount)
    {
        amount += currentHealth;
        amount = Mathf.Clamp(amount, currentHealth, maxHealth);

        currentHealth = amount;

        Debug.Log(transform.name + " increased health is " + amount);

    }

    public virtual void Die()
    {
        //Die in some way.
        Debug.Log(transform.name + " died.");
    }

}
