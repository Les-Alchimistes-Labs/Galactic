

namespace personnage_class.Personage.Monsters
{
    public class LittelMonster : Monster
    {
        public override EnumType Type() => EnumType.LittleMonster;
        public LittelMonster(string name, int life = 10, int maxlife = 20, int damage = 5, int boost = 1, int level = 0) : base(name, (int) (life * 0.5), (int) (maxlife * 0.5), (int) (damage * 0.5), boost, (int) (2), (int) (level * 0.5)) // all 0.5 without boost
        {
            inFight = false;
            Inventory[0] = new Food("Banana Variant", 50, life);
            Inventory[1] = new Food("Cheese Variant", 50, life);
            
        }

        
        
        
    }
}