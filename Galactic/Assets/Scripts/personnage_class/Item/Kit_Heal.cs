namespace personnage_class.Personage
{
    public class Kit_Heal : Item
    {
        public Kit_Heal(int expiry, int energyAmount, int heal) : base(expiry, energyAmount, true, heal, EnumsItem.Boost)
        {
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}