namespace personnage_class.Personage
{
    public class Ordinateur_Kali_Linux : Item
    {
        public Ordinateur_Kali_Linux(int expiry, int energyAmount, bool isEdible) : base(expiry, energyAmount, isEdible)
        {
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}