namespace personnage_class.Personage
{
    public class sniper : Item
    {
        public sniper(int expiry, int energyAmount, bool isEdible) : base(expiry, energyAmount, isEdible)
        {
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}