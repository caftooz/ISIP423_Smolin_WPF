using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Models.Entities
{

    public abstract class Enemy
    {
        public string Name { get; protected set; }
        public int MaxHp { get; protected set; }
        public int CurrentHp { get; protected set; }
        public int Attack { get; protected set; }
        public int Defense { get; protected set; }
        public bool IsAlive => CurrentHp > 0;

        public string ImagePath { get; protected set; }

        protected Enemy(string name, int hp, int attack, int defense, string imagePath)
        {
            Name = name;
            MaxHp = hp;
            CurrentHp = hp;
            Attack = attack;
            Defense = defense;
            ImagePath = imagePath;
        }

        public int TakeDamage(int rawDamage)
        {
            int actual = System.Math.Max(0, rawDamage - Defense);
            CurrentHp = System.Math.Max(0, CurrentHp - actual);
            return actual;
        }

        public abstract string ApplySpecialAbility(Player player, int rawDamage);

        public override string ToString() => $"{Name} (HP: {CurrentHp}/{MaxHp})";
    }
}
