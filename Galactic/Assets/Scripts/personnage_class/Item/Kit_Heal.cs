namespace personnage_class.Personage
{
    public class Kit_Heal : Item
    {
        public Kit_Heal(int expiry, int heal) : base(expiry, true, heal, EnumsItem.Boost)
        {
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