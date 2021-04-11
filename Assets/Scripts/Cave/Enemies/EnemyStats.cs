using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private StatUI healthUI;
    private PlayerStats playerStats;
    public bool hasKey = false;
    

    void Start()
    {
        healthUI = this.GetComponent<StatUI>();
        healthUI.SetAllSliderValues(0, maxHealth, maxHealth - 2);

        //Set enemy health to be max on awake.
        this.IncreaseHealth(maxHealth);

        //Add key to enemy at a random chance.
        AddKey();

        InvokeRepeating("InflictDamage", 1, 1);
    }

    private void AddKey()
    {
        //hasKey = true;
        bool hasKey = (Random.value > 0.5f);

        if(hasKey && GameManagerScript.instance.NoOfAvailableKeys < GameManagerScript.instance.NoOfHiddenRooms)
        {
            GameManagerScript.instance.NoOfAvailableKeys++;
        }
        else if(GameManagerScript.instance.NoOfAvailableKeys > GameManagerScript.instance.NoOfHiddenRooms)
        {
            hasKey = false;
        }

    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        //Update health bar UI.
        healthUI.SetValue(currentHealth);
    }

    public void InflictDamage()
    {
        if(GameManagerScript.instance.dungeonInUse)
        {
            float distance;

            distance = Vector3.Distance(this.transform.position, GameManagerScript.instance.playerRef.transform.position);

            if (distance < 2f)
            {
                GameManagerScript.instance.playerRef.GetComponent<PlayerStats>().TakeDamage(this.strength.GetValue());
            }
        }
    }


    public override void Die()
    {
        base.Die();

        //Drop key on the ground if one is equiped.
        if(hasKey)
        { 
            GameObject key = Instantiate(GameManagerScript.instance.keyPrefab, this.transform.position , Quaternion.identity) as GameObject;
            key.transform.SetParent(GameManagerScript.instance.dungeonMapRef.transform, true);
        }

        //Add some graphics to this.

        GameObject.Destroy(this.gameObject);
    }
    
}
