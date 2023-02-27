using System;


namespace personnage_class.Personage
{

public abstract class Personnage : Update 
{
    
    private string _name;
    protected int Life;
    protected int MaxLife;
    protected int Damage;
    protected int Boost;
    protected Item[] Inventory ;
    public int level { protected set; get; }
    public bool canMove { protected set; get; }
    public bool InFight { protected set; get; }
    



    public Personnage(string name , int life = 10 , int maxlife = 20,int damage = 5,int boost = 1, int inventorySize = 8)
    {
        _name = name;
        canMove = true;
        Life = life;
        Inventory = new Item[inventorySize];
        Damage = damage;
        Boost = boost;
        level = 0;
        InFight = false;
        MaxLife = maxlife;
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

    public bool Is_Alive()
    {
        return Life >= 0;
    }
    
    public int Get_damage() => Damage;

    public void Set_boost(int i)
    {
        Boost = i;
    }
    
    public void Reset_boost()
    {
        Boost = 1;
    }
    
    public void Attack(Personnage victim)
    {
        victim.Remove_Life( Damage);
    }
    
    public void Take_Damage(int damage_took)
    {
        Remove_Life(damage_took);
    }
    
    public bool took(Item item)
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
    
    public Item trow(int i)
    {
        if (i < 0 || i > Inventory.Length)
        {
            throw new IndexOutOfRangeException();
        }
        Item temp = Inventory[i];
        Inventory[i] = null;
        return temp;

    }

    public bool Use(Item item)
    {
        
        if (item.Type != EnumsItem.None)
        {
            switch (item.Type)
            {
                case EnumsItem.Armes :
                    return false;
                case EnumsItem.Boost :
                    if (item.GetHeal() == 0 && Boost + item.GetEnergyAmount() < 0)
                    {
                        Boost += item.GetEnergyAmount();
                        return true;
                    }
                    else if (Add_Life(item.GetHeal()))
                    {
                        return true;
                    }
                    break;
                case EnumsItem.Equipement :
                    break;
                case EnumsItem.Food :
                    if (Life + item.GetEnergyAmount() < MaxLife)
                    {
                        Boost += (item.GetEnergyAmount());
                        return true;
                    }
                    break;
            }       
        }
        return false;
    }
    public bool Use(int i)
    {
        if (i >= 0 && i < Inventory.Length && Use(Inventory[i]) )
        {
            Inventory[i] = null;
            return true;
        }
        
        return false;
    }
    
    
    public abstract void Update();
    
    }
}