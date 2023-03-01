namespace personnage_class.Personage
{
    public class Hacker : Personnage
    {

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

        public Hacker(string name, int life = 10, int damage = 5, int boost = 1, int inventori_size = 8 , int maxlevel = 0) : base(name, life,life, damage, boost * 2, inventori_size * 2 ,  0, maxlevel) // inventory size *2 and boost *2
        {
        }
    }
}