namespace personnage_class.Personage
{
    public class Kit_Heal : Item
    {
        public Kit_Heal(string name,int expiry, int heal) : base(name,expiry, true, heal, EnumsItem.Boost)
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