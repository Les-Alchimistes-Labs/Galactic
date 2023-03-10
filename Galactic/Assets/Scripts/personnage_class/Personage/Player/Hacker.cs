namespace personnage_class.Personage
{
    public class Hacker : Player
    {
        public override EnumType Type() => EnumType.Hacker;

        public Hacker(string name, int life = 10, int damage = 5, int boost = 1, int inventori_size = 8 , int maxlevel = 0) : base(name, life,life, damage, boost * 2, inventori_size * 2 ,  0, maxlevel) // inventory size *2 and boost *2
        {
        }
    }
}