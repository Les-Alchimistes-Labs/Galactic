namespace personnage_class.Personage
{
    public class Canonnier : Player

    {
        public override EnumType Type() => EnumType.Canonnier;

        

        public Canonnier(string name, int life = 10, int damage = 5, int boost = 1, int inventori_size = 8 , int maxlevel = 0) : base(name, life,life, damage, boost * 4, inventori_size ,  0, maxlevel) // boost *4
        {
        }
    }
}