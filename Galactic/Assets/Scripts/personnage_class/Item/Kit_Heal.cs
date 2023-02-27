namespace personnage_class.Personage
{
    public class Kit_Heal : Item
    {
        public Kit_Heal(int expiry, int energyAmount, bool isEdible) : base(expiry, energyAmount, isEdible)
        {
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}