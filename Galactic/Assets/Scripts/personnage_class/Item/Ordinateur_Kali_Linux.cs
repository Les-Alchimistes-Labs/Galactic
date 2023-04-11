namespace personnage_class.Personage
{
    public class Ordinateur_Kali_Linux : Item
    {
        public Ordinateur_Kali_Linux(float boost , int expiry) : base(expiry,0, EnumsItem.Equipement , boost)
        {
            Name = "Ordinateur_Kali_Linux";
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