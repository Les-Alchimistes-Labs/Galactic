using System;


namespace personnage_class.Personage
{

public abstract class Personnage : Update 
{
    
    private string _name;
    protected int Life;
    protected int Damage;
    protected int Boost;
    protected Item[] Inventory ;
    public int level { protected set; get; }
    public bool canMove { protected set; get; }
    public bool InFight { protected set; get; }
    



    public Personnage(string name , int life = 10,int damage = 5,int boost = 1, int inventorySize = 8)
    {
        _name = name;
        canMove = true;
        Life = life;
        Inventory = new Item[inventorySize];
        Damage = damage;
        Boost = boost;
        level = 0;
        InFight = false;
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

    public abstract void Add_Life(int i);
    
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
        Damage -= damage_took;
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
    
    public abstract void Update();
    
    }
}