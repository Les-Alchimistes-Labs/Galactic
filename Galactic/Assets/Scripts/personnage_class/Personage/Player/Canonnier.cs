namespace personnage_class.Personage
{
    public class Canonnier : Player

    {
        public override EnumType Type() => EnumType.Canonnier;

        

        public Canonnier(string name, int life = 10, int damage = 5, int boost = 1, int inventori_size = 8 , int maxlevel = 10) : base(name, life,life, damage, boost * 3, inventori_size ,  0, maxlevel) // boost *4
        {
            Inventory[0] = new Gun(10, 10);
            Inventory[1] = new Sniper_a(10,10, 1);

        }
    }
}