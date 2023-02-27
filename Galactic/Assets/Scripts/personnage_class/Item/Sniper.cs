namespace personnage_class.Personage
{
    public class Sniper_a : Item
    {
        public Sniper_a(int expiry) : base(expiry, 0, false, 0, EnumsItem.Armes)
        {
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}