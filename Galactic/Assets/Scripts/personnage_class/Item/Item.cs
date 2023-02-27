using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace personnage_class.Personage
{
public abstract class Item : Update
{
    protected int Damage;
    protected int Expiry;
    protected int EnergyAmount;
    protected bool IsEdible;
    protected EnumsItem Type;

    protected Item(int expiry, int energyAmount, bool isEdible)
    {

        Expiry = expiry;
        EnergyAmount = energyAmount;
        IsEdible = isEdible;
    }
    
    

    public abstract void Update();

    public int GetEnergyAmount()
    {
        return EnergyAmount;
    }

    public int GetExpiry()
    {
        return Expiry;
    }

    public bool GetIsEdible()
    {
        return IsEdible;
    }
    
    public int GetDamage()
    {
        return Damage;
    }
}
}