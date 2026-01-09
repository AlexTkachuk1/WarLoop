using System;
using _Scripts.Actors.Units;

namespace _Scripts.Actors.Buildings
{
    public class Building : Actor
    {
        public void Init(FactionColor factionColor)
        {
            InitInternal(factionColor);
        }

        public void Absorb(Unit unit)
        {
            if (unit.Faction != Faction) throw new InvalidOperationException("Factions are not equal");
            CurrentHealth += unit.Cost * unit.HealthPercentage;
            unit.Die(this);
        }

        public override void Die(Actor attacker)
        {
            Capture(attacker);
        }

        protected override float CalculateIncomingDamage(Actor attacker) => attacker.DamagePerSecond;

        private void Capture(Actor attacker)
        {
            Faction = attacker.Faction;
            CurrentHealth = 1;
        }
    }
}