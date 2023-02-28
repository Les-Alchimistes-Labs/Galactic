namespace personnage_class.Personage
{
    public class Medecin : Personnage
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

        public Medecin(string name, int life = 10, int damage = 5, int boost = 1, int inventori_size = 8) : base(name, (int) (life*0.8), (int) (life*0.8), damage, boost, inventori_size) // life *0.8
        {
        }
    }
}