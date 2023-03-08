namespace personnage_class.Personage.Monsters
{
    public class LittelMonster : Personnage
    {
        
        public override EnumsPersonage Type() => EnumsPersonage.Monster;
        public LittelMonster(string name, int life = 10, int maxlife = 20, int damage = 5, int boost = 1, int inventorySize = 2, int level = 0) : base(name, (int) (life * 0.5), (int) (maxlife * 0.5), (int) (damage * 0.5), boost, (int) (inventorySize * 0.5), (int) (level * 0.5)) // all 0.5 without boost
        {
            
        }

        public override bool Add_Life(int i)
        {
            if (Life + i <= MaxLife)
            {
                Life += i;
                return true;
            }

            return false;
        }

        public override void Remove_Life(int i)
        {
            if (Life-i>0)
                Life -= i;
            else
            {
                Life = 0;
            }
        }
        
        public override void Update()
        {
            
        }
    }
}