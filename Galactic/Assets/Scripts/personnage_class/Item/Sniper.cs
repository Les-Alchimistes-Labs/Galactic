namespace personnage_class.Personage
{
    public class Sniper_a : Item
    {
        public Sniper_a(int damage , float boost , int expiry) : base(expiry ,damage, EnumsItem.Armes , boost)
        {
            Name = "Sniper";
        }

        public override void Update()
        {
            if (Expiry == 0)
            {
                Type = EnumsItem.None;
            }
            else
            {
                Expiry -= 1;
            }
        }
    }
}