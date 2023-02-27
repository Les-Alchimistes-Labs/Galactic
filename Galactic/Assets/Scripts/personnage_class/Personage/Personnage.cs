using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public abstract class Personnage : Update 
{
    
    protected string _name;
    protected int _life;
    protected int _damage;
    protected int _boost;
    protected Item[]? _inventory = new Item[8];
    
    
    public string Name => this._name;

    
    public Item[]? Get_Inventory ()=> _inventory;


    public Item? this[int i]
    {
        get
        {
            if (i < 0 || i > 0)
            {
                throw new IndexOutOfRangeException();
            }

            return _inventory?[i];

        }
    }
    
    public abstract int Get_Life();
    
    public abstract void Add_Life(int i);
    
    public abstract void Remove_Life(int i);

    public bool Is_Alive()
    {
        return _life >= 0;
    }
    
    public int Get_damage() => _damage;

    public void Set_boost(int i)
    {
        _boost = i;
    }
    
    public void Reset_boost()
    {
        _boost = 1;
    }
    
    public void Attack(Personnage victim)
    {
        victim.Remove_Life( _damage);
    }
    
    public void Damage(int damage_took)
    {
        _damage -= damage_took;
    }
    
    public bool took(Item item)
    {
        int i = 0;
        while (i<8)
        {
            if (_inventory[i] == null)
            {
                _inventory[i] = item;
                return true;
            }
        }

        return false;
    }
    
    public Item trow(int i)
    {
        if (i < 0 || i > 0)
        {
            throw new IndexOutOfRangeException();
        }
        Item? temp = _inventory[i];
        _inventory[i] = null;
        return temp;

    }
    
    public abstract void Update();
    
}