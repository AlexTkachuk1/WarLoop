using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Actors
{
    public abstract class Actor : MonoBehaviour
    {
        [SerializeField] private GameObject[] _factionPrefabs;
        
        [field: SerializeField] public float CurrentHealth { get; protected set; }
        [field: SerializeField] public float RegenPerSecond { get; private set; }
        [field: SerializeField] public float DamagePerSecond { get; private set; }
        
        public FactionColor Faction
        {
            get => _faction;
            protected set
            {
                _faction = value;
                foreach (var factionPrefab in _factionPrefabs) factionPrefab.SetActive(false);
                _factionPrefabs[(int)_faction].SetActive(true);
            }
        }

        public float HealthPercentage => Mathf.Clamp01(CurrentHealth / _healthMax);

        private readonly List<Actor> _actorsInRange = new();
        private float _healthMax;
        private FactionColor _faction;

        public void SpawnUnit(int cost)
        {
            if (cost >= CurrentHealth) return;
            
            CurrentHealth -= cost;
            UpdateCurrentHealth();
        }
        
        protected virtual void UpdateCurrentHealth() {}
        
        protected void InitInternal(FactionColor factionColor)
        {
            _healthMax = CurrentHealth;
            Faction = factionColor;
        }

        private void OnDestroy()
        {
            _actorsInRange.Clear();
        }

        public virtual ProcessFrameResult ProcessFrame()
        {
            RegenerateHealth();
            return TryAttack() ? ProcessFrameResult.Attacking : ProcessFrameResult.Idle;
        }
        
        public abstract void Die(Actor attacker);

        private bool TryAttack()
        {
            for (var i = 0; i < _actorsInRange.Count; i++)
            {
                var actor = _actorsInRange[i];
                if (!actor)
                {
                    _actorsInRange.RemoveAt(i);
                    i--;
                    continue;
                }
                
                if (actor.Faction == Faction)
                    continue;
                
                var isDead = actor.TakeDamage(this);
                if (isDead)
                    actor.Die(this);
                return true;
            }

            return false;
        }

        protected virtual void RegenerateHealth() => CurrentHealth += RegenPerSecond * Time.deltaTime;
        
        private bool TakeDamage(Actor attacker)
        {
            var damageAmount = CalculateIncomingDamage(attacker) * Time.deltaTime;
            CurrentHealth = Mathf.Max(0, CurrentHealth - damageAmount);
            
            UpdateCurrentHealth();

            return CurrentHealth <= 0;
        }


        protected abstract float CalculateIncomingDamage(Actor attacker);
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Actor actor))
                return;
            
            if (actor.Faction == Faction) 
                return;
            
            if (!_actorsInRange.Contains(actor))
                _actorsInRange.Add(actor);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out Actor actor))
                return;
            
            _actorsInRange.Remove(actor);
        }

        public void Animate(ProcessFrameResult result)
        {
            // TODO
            switch (result)
            {
                case ProcessFrameResult.Idle:
                    break;
                case ProcessFrameResult.Attacking:
                    break;
                case ProcessFrameResult.Running:
                    break;
            }
        }
    }
}