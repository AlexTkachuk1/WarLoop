using System.Collections.Generic;
using _Scripts.Vfx;
using UnityEngine;

namespace _Scripts.Actors
{
    public abstract class Actor : MonoBehaviour
    {
        [SerializeField] private ActorAnimation[] _factionPrefabs;
        
        [field: SerializeField] public float CurrentHealth { get; protected set; }
        [field: SerializeField] public float RegenPerSecond { get; private set; }
        [field: SerializeField] public float DamagePerSecond { get; private set; }
        
                
        [SerializeField] protected float regenInterval = 4f;
        protected int regenAmount;
        protected float regenTimer = 0;
        
        [SerializeField] protected float attackInterval = 3f;
        protected int attackAmount;
        protected float attackTimer = 0;

        public FactionColor Faction
        {
            get => _faction;
            protected set
            {
                _faction = value;
                foreach (var factionPrefab in _factionPrefabs) factionPrefab.gameObject.SetActive(false);
                _factionPrefabs[(int)_faction].gameObject.SetActive(true);
            }
        }

        protected ActorAnimation CurrentAnimation => _factionPrefabs[(int)_faction];

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
            
            attackAmount = (int)(DamagePerSecond * attackInterval);
            regenAmount = (int)(RegenPerSecond * regenInterval);
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
                
                if (actor.Faction == Faction) continue;

                if (attackTimer == 0)
                {
                    attackTimer = Time.realtimeSinceStartup + attackInterval;
                }
                else
                {
                    if (attackTimer > Time.realtimeSinceStartup) return true;

                    attackTimer = 0;
                    var isDead = actor.TakeDamage(attackAmount);
                    if (isDead) actor.Die(this);
                
                    return true;
                }
            }

            return false;
        }

        protected virtual void RegenerateHealth()
        {
            if (regenTimer == 0)
            {
                regenTimer = Time.realtimeSinceStartup + regenInterval;
            }
            else
            {
                if (regenTimer <= Time.realtimeSinceStartup)
                {
                    regenTimer = 0;
                    CurrentHealth += regenAmount;
                    UpdateCurrentHealth();
                }
            }
        }
        
        private bool TakeDamage(float damageAmount)
        {
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

        public virtual void Animate(ProcessFrameResult result) { }
    }
}