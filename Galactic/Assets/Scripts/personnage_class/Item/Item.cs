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
    protected int Heal;
    protected bool IsEdible;
    public float Boost  {protected set; get; }
    public EnumsItem Type  {protected set; get; }

    protected Item(int expiry, int energyAmount, bool isEdible, int heal, EnumsItem type ,float boost = 0)
    {

        Expiry = expiry;
        EnergyAmount = energyAmount;
        IsEdible = isEdible;
        Heal = heal;
        Type = type;
        Boost = boost;
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
    
    public int GetHeal()
    {
        return Heal;
    }
}
}