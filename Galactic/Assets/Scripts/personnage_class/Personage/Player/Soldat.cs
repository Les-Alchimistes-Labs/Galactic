namespace personnage_class.Personage
{
    public class Soldat : Personnage
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
            throw new System.NotImplementedException();
        }

        public Soldat(string name, int life = 10, int damage = 5, int boost = 1, int inventori_size = 8) : base(name, life, life, damage, boost, inventori_size)
        {
        }
    }
}