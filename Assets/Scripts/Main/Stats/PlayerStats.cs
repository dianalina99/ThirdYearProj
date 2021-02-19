using UnityEngine;

public class PlayerStats : CharacterStats
{
    #region Singleton
    public static PlayerStats instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More player stats found!");
            return;
        }
        instance = this;
    }

    #endregion


    private StatUI healthStatUI;
    private StatUI strengthStatUI;
    private StatUI protectionStatUI;


    // Start is called before the first frame update
    void Start()
    {
        healthStatUI = GameObject.Find("PlayerHealthBar") .GetComponent<StatUI>();
        strengthStatUI = GameObject.Find("PlayerStrengthBar").GetComponent<StatUI>();
        protectionStatUI = GameObject.Find("PlayerProtectionBar").GetComponent<StatUI>();

       EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;

        InitializeSliders();

        //Set player health to be max on awake.
        this.IncreaseHealth(maxHealth);
    }

    void InitializeSliders()
    {
        healthStatUI.SetAllSliderValues(0, maxHealth, maxHealth - 2);
        strengthStatUI.SetAllSliderValues(0, 10, 0);
        protectionStatUI.SetAllSliderValues(0, 10,0);
    }

    void OnEquipmentChanged( Equipment newItem, Equipment oldItem)
    {
        if(newItem != null)
        {
            protection.AddModifier(newItem.armorModifier);
            strength.AddModifier(newItem.damageModifier);
        }

        if(oldItem != null)
        {
            protection.RemoveModifier(oldItem.armorModifier);
            strength.RemoveModifier(oldItem.damageModifier);
        }


        //Update protection and strength UI.
        protectionStatUI.SetValue(protection.GetValue());
        strengthStatUI.SetValue(strength.GetValue());
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        //Update health bar UI.
        healthStatUI.SetValue(currentHealth);
    }

    public override void IncreaseHealth(int amount)
    {
        base.IncreaseHealth(amount);

        //Update health bar UI.
        healthStatUI.SetValue(currentHealth);
    }

}
