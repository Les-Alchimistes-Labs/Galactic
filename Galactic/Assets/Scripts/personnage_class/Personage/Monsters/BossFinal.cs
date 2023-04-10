namespace personnage_class.Personage.Monsters
{
    public class BossFinal : Monster
    {
        public override EnumType Type() => EnumType.FinalBoss;
        public BossFinal(string name, int life = 10, int maxlife = 20, int damage = 5, int boost = 1, int inventorySize = 8, int level = 0 , int nb_Food = 0, int nb_Boost = 0) : base(name, life * 5, maxlife * 5, damage * 3 , boost, inventorySize, level * 3) // life *5 damage *3 level *3 
        {
            inFight = false;
            
            Inventory[0] = new Boss_Weapon(100 , Damage , Boost);
            Change_Weapon_Equipped(0);

            if (nb_Food + nb_Boost > inventorySize)
            {
                if (nb_Boost - inventorySize < 0)
                {
                    if (nb_Food - nb_Boost - inventorySize < 0)
                    {
                        nb_Food -= nb_Boost - inventorySize;
                        nb_Boost = 0;
                    }
                    else
                    {
                        nb_Food = 0;
                        nb_Boost = 0;
                    }
                }
                else
                {
                    nb_Boost = 0;
                }
            }
            while (nb_Boost >0)
            {
                Inventory[nb_Boost-1] = new Potion_Boost("Potion_Stamina Variant",50 , false , 0 , EnumsItem.Boost , boost);
                nb_Boost--;
            }
            while (nb_Food >0)
            {
                Inventory[nb_Boost + nb_Food -2] = new Food("Hamburger Variant", 50, life);
                nb_Food--;
            }
            
        }

    }
}