using System.Collections.Generic;
using UnityEngine.UIElements;

namespace personnage_class.Personage
{
    public class Medecin : Personnage
    {
        public override EnumsPersonage TypePersonage() => EnumsPersonage.Monster;

        public override EnumType Type() => EnumType.Medecin;

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
            for (int i= 0; i< Inventory.Length; i++)
            {
                if (Inventory[i] != null && Inventory[i].Type != EnumsItem.Armes &&
                    Inventory[i].Type != EnumsItem.Equipement)
                {
                    Inventory[i].Update();
                    if (Inventory[i].Type == EnumsItem.None)
                    {
                        Inventory[i] = null;
                    }
                }
                
            }


        }

        
        
        
        public Medecin(string name, int life = 10, int damage = 5, int boost = 1, int inventori_size = 8,
            int maxlevel = 10) : base(name, (int)(life * 0.8), (int)(life * 0.8), damage, boost, inventori_size, 0,
            maxlevel) // life *0.8
        {
        }


        public void Heal(Personnage heros)
        {
            int life = (int)(Life * 0.8);
            while (heros.Add_Life(life) == false)
                life -= 1;
            heros.Add_Life(life);
        }
        public void Target(List<Personnage> heros, int find)
        {
            var target = heros[0];
            for (int i = 1; i < heros.Count; i++)
            {
                if (target.IsAlive())
                {
                    if (heros[i].IsAlive())
                    {
                        if (heros[i].Getlife > target.Getlife)
                            target = heros[i];
                    }
                }
            }
            Heal(target);
        }
        
        
    }
}