namespace personnage_class.Personage.Monsters
{
    public class BossIntermediate : Personnage
    {
        public override EnumsPersonage Type() => EnumsPersonage.Monster;
        
        public BossIntermediate(string name, int life = 10, int maxlife = 20, int damage = 5, int boost = 1, int inventorySize = 8, int level = 0) : base(name, (int) (maxlife * 2.5),  (int) (maxlife * 2.5), (int) (damage * 1.5), boost, inventorySize, (int) (level* 1.5)) // life *2.5 damage *1.5 level *1.5 
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
            if (Life - i > 0)
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