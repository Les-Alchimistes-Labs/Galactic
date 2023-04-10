namespace personnage_class.Personage
{
    public class Potion_Boost : Item
    {
        public Potion_Boost(string name,int expiry, bool isEdible, int heal, EnumsItem type, float boost = 1) : base(name,expiry, isEdible, heal, type, boost)
        {
        }

        public override void Update()
        {
            if (Boost > 0.5)
            {
                if (Expiry == 0)
                {
                    
                    Boost =  (int)(Boost * 0.8);
                }
                else
                {
                    Expiry -= 1;
                }
            }
            else
            {
                Type = EnumsItem.None;
            }

        }
    }
}