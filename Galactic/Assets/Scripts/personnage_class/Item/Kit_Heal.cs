namespace personnage_class.Personage
{
    public class Kit_Heal : Item
    {
        public Kit_Heal(int expiry, int energyAmount, bool isEdible, int heal) : base(expiry, energyAmount, isEdible, heal, EnumsItem.Boost)
        {
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}