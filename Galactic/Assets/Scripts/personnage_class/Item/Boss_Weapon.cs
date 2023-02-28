namespace personnage_class.Personage
{
    public class Boss_Weapon : Item
    {
        public Boss_Weapon(int expiry, int damage, float boost = 1) : base(expiry, damage, EnumsItem.Armes, boost)
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