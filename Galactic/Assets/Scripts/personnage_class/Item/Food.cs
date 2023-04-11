namespace personnage_class.Personage
{
    public class Food : Item
    {


        public Food(string name,int expiry, int energyAmount) : base(name,expiry, true, 0, EnumsItem.Food, energyAmount)
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