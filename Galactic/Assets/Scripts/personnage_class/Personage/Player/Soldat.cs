namespace personnage_class.Personage
{
    public class Soldat : Player
    {

        public Soldat(string name, int life = 10, int damage = 5, int boost = 1, int inventori_size = 8 , int maxlevel = 0) : base(name, life*2, life*3, (int)(damage*1.5), boost, inventori_size ,  0, maxlevel) // boost life *2 boost damage*1.5 boost max life *3
        {
            Inventory[0] = new Food("Banana Variant",1,1);

        }

        public override EnumType Type() => EnumType.Soldat;
    }
}