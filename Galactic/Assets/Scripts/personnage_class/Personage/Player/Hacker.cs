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
            foreach (var item in Inventory)
            {
                if (item.GetIsEdible() || item.Type == EnumsItem.Boost)
                    item.Update();
            }
        }

        public Hacker(string name, int life = 10, int damage = 5, int boost = 1, int inventori_size = 8) : base(name, life,life, damage, boost * 2, inventori_size * 2 ) // inventory size *2 and boost *2
        {
        }
    }
}