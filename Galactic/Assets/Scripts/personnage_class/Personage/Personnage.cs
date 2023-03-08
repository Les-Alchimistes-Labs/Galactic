using System;
using System.Net.Security;


namespace personnage_class.Personage
{

public abstract class Personnage : Update 
{
    
    private string _name;
    private int _xp;
    private int _maxxp;
    protected int Life;
    protected int MaxLife;
    protected int Damage;
    protected int Boost;
    protected Item[]? Inventory ;
    private int _maxlevel;
    public int MaxLevel // rajouter
    {
        get { return _maxlevel;}
        set
        {
            if (value > MaxLevel)
                _maxlevel = value;
        }
    }
    
    public int Maxxp // rajouter
    {
        get { return _maxxp;}
        set
        {
            if (value > Maxxp)
                _maxlevel = value;
        }
    }

    public abstract EnumsPersonage Type();

    public Item? pricipale_Weapon { protected set; get; }
    public int level { protected set; get; }
    public bool canMove { protected set; get; } // if personnage can move
    public bool inFight { protected set; get; } // if personnage is in fight
    



    public Personnage(string name , int life = 10 , int maxlife = 20,int damage = 5,int boost = 1, int inventorySize = 8, int levelt = 0, int maxlevel = 0) // create one personage with name life inventory level and xp 
    {
        _name = name;
        canMove = true;
        Life = life;
        Inventory = new Item[inventorySize];
        Damage = damage;
        Boost = boost;
        level = levelt;
        inFight = false;
        MaxLife = maxlife;
        _xp = 0;
        _maxxp = 30;
        _maxlevel = maxlevel;

    }
    
    
    public string name => this._name; 

    
    public Item[] Get_Inventory ()=> Inventory;


    public Item this[int i] 
    {
        get
        {
            if (i < 0 || i > Inventory.Length )
            {
                throw new IndexOutOfRangeException();
            }

            return Inventory?[i];

        }
    }

    public int Getlife => Life;

    public abstract bool Add_Life(int i);
    
    public abstract void Remove_Life(int i);

    public bool Is_Alive() => Life >= 0;

    public int GetXP() => _xp;

    public (int damage, Item better_weapon) better_weapon() // return tuple with dammage and the better weapon in inventory
    {
        int max = 0;
        Item better = null;
        foreach (var item in Inventory)
        {
            if (item != null && item.Type == EnumsItem.Armes)
            {
                if (better != null  &&  better.GetDamage() < item.GetDamage())
                {
                    max = item.GetDamage();
                    better = item;
                }
                else 
                {
                    max = item.GetDamage();
                    better = item;
                }
            }
            
        }
        
        return (max, better);
    }

    public void Change_Weapon_Equipped(int i) // change the Weapon equipped  by the item at the index 
    {
        if (Inventory[i] == null || Inventory[i].Type == EnumsItem.Armes) 
            (Inventory[i], pricipale_Weapon) = (pricipale_Weapon, Inventory[i]);
    }


    public int Get_damage()
    {
        if (pricipale_Weapon != null)
        {
            if (level != 0)
                return pricipale_Weapon.GetDamage() * Boost * level;
            else 
                return pricipale_Weapon.GetDamage() * Boost ;}
        else
        {
            if (level != 0)
                return Damage * Boost * level;
            else 
                return Damage * Boost ;
        }
    }

    public void Set_boost(int i) // set boost only if i > 0
    {
        if (i> 0)
           Boost = i;
    }
    
    public void Reset_boost()
    {
        Boost = 1;
    }
    
    public void Attack(Personnage victim) 
    {
        victim.Remove_Life( Get_damage());
        if (pricipale_Weapon != null)
        {
            Add_Xp(pricipale_Weapon.GetDamage() * Boost); // a rajouter 
            pricipale_Weapon.Update();
        }
        else
        {
            Add_Xp(  Boost); 
        }
    }
    
    public void Take_Damage(int damage_took)
    {
        Remove_Life(damage_took);
    }
    
    public bool Took(Item item) // add one item to inventory if inventory not full
    {
        int i = 0;
        while (i<Inventory.Length)
        {
            if (Inventory[i] == null)
            {
                Inventory[i] = item;
                return true;
            }

            i++;
        }

        return false;
    }
    
    public Item Trow(int i) // return one item to inventory and delete it
    {
        if (i < 0 || i > Inventory.Length)
        {
            throw new IndexOutOfRangeException();
        }
        Item temp = Inventory[i];
        Inventory[i] = null;
        return temp;

    }

    public bool Use(Item item) // use one item 
    {
        
        if (item.Type != EnumsItem.None)
        {
            switch (item.Type)
            {
                case EnumsItem.Armes :
                    return false;
                case EnumsItem.Boost :
                    if (item.GetHeal() == 0 && Boost + item.GetBoost() < 0)
                    {
                        Boost += item.GetBoost();
                        return true;
                    }
                    else if (Add_Life(item.GetHeal()* Boost * level))
                    {
                        return true;
                    }
                    break;
                case EnumsItem.Equipement :
                    break;
                case EnumsItem.Food :
                    if (Life + item.GetBoost() < MaxLife)
                    {
                        Boost += (item.GetBoost());
                        return true;
                    }
                    break;
            }       
        }
        return false;
    }
    public bool Use(int i) //use item in inventory at index i
    {
        if (i >= 0 && i < Inventory.Length && Use(Inventory[i]) )
        {
            Inventory[i] = null;
            return true;
        }
        
        return false;
    }


    public bool LevelUp()
    {
        if (level <= MaxLevel && _xp >= level * _maxxp)
        {
            level ++;
            _xp = 0;
            return true;
        }

        return false;
    }

    public void Add_Xp(int nb ) // add xp and level while xp > 0
    {
        while (nb> 0 && level <=  MaxLevel )
        {
            if (nb + _xp > level * _maxxp  )
            {
                nb -= level * _maxxp + _xp;
                _xp = level * _maxxp; 
                if (!LevelUp())
                    break;
            }
            else
            {
                _xp += nb;
                nb = 0;
                if (!LevelUp())
                    break;
            }
        }
    }

    public abstract void Update();


}
}