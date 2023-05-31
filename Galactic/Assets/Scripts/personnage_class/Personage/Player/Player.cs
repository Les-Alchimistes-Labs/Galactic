namespace personnage_class.Personage
{
    public abstract class Player : Personnage
    {
        public override EnumsPersonage TypePersonage() => EnumsPersonage.Player;
        
        protected Player(string name, int life = 10, int maxlife = 20, int damage = 5, int boost = 1, int inventorySize = 8, int levelt = 0, int maxlevel = 0) : base(name, life, maxlife, damage, boost, inventorySize, levelt, maxlevel)
        {
            
        }
        
        
        public override bool Add_Life(int i)
        {
            if (Life + i <= MaxLife)
            {
                Life += i;
                return true;
            }

            return false;
        }

        public override void Remove_Life(int i)
        {
            if (Life-i>0)
                Life -= i;
            else
            {
                Life = 0;
            }
        }

        public override void Update()
        {
            for (int i= 0; i< Inventory.Length; i++)
            {
                if (Inventory[i] != null && Inventory[i].Type != EnumsItem.Armes &&
                    Inventory[i].Type != EnumsItem.Equipement)
                {
                    Inventory[i].Update();
                    if (Inventory[i].Type == EnumsItem.None)
                    {
                        Inventory[i] = null;
                    }
                }
                
            }
        }
        
        
        
        
        
    }
}