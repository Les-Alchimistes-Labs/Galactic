namespace personnage_class.Personage
{
    public class Sniper : Player
    {
        public override EnumType Type() => EnumType.Sniper;
        
        public Sniper(string name, int life = 10, int damage = 5, int boost = 1, int inventori_size = 8 , int maxlevel = 10) : base(name, (int) (life*0.5), (int) (life*0.8), damage, boost, inventori_size ,  0, maxlevel) // life *0.5
        {
        }
    }
}