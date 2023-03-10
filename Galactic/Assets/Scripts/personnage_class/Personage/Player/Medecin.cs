namespace personnage_class.Personage
{
    public class Medecin : Player
    {
        public override EnumType Type() => EnumType.Medecin;
        
        public Medecin(string name, int life = 10, int damage = 5, int boost = 1, int inventori_size = 8 , int maxlevel = 0) : base(name, (int) (life*0.8), (int) (life*0.8), damage, boost, inventori_size ,  0, maxlevel) // life *0.8
        {
        }
    }
}