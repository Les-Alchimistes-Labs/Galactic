namespace personnage_class.Personage
{
    public class Medecin : Personnage
    {

        public override void Add_Life(int i)
        {
            throw new System.NotImplementedException();
        }

        public override void Remove_Life(int i)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public Medecin(string name, int life = 10, int damage = 5, int boost = 1, int inventori_size = 8) : base(name, life, damage, boost, inventori_size)
        {
        }
    }
}