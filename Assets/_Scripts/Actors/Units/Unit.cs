using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Actors.Buildings;
using _Scripts.GameLoop;
using _Scripts.Helpers;
using UnityEngine;

namespace _Scripts.Actors.Units
{
    public class Unit : Actor
    {
        [SerializeField] private GameObject _deathEffectPrefab;

        [field: SerializeField] public UnitType UnitType { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float Cost { get; private set; }
        
        private List<Vector2> _positions;
        private float[] _segmentLengths;
        private float _totalLength;
        private float _distanceTraveled;
        private int _segmentIndex;

        public Building Origin { get; private set; }
        public Building Target { get; private set; }

        public void Init(List<Cell> cells, FactionColor factionColor)
        {
            InitInternal(factionColor);

            _positions = cells.Select(a => new Vector2(a.X, a.Y)).ToList();

            BuildPathCache();
            _distanceTraveled = 0f;
            _segmentIndex = 0;

            transform.position = _positions[0];

            if (!BuildingUtils.TryGetBuildingOnCell(cells.First(), out var playerBuilding))
            {
                throw new NullReferenceException("Cell is not Building");
            }
            
            Origin = playerBuilding;
            
            if (!BuildingUtils.TryGetBuildingOnCell(cells.Last(), out var enemyBuilding))
            {
                throw new NullReferenceException("Cell is not Building");
            }
            
            Target = enemyBuilding;
        }

        public override ProcessFrameResult ProcessFrame()
        {
            var result = base.ProcessFrame();
            if (result == ProcessFrameResult.Attacking)
                return result;
            
            if (TryMove())
                return ProcessFrameResult.Running;
                
            Target.Absorb(this);
            return ProcessFrameResult.ScheduledForDisposal;
        }

        public override void Die(Actor attacker)
        {
            Instantiate(_deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        protected override float CalculateIncomingDamage(Actor attacker)
        {
            if (attacker is not Unit attackerUnit)
                return attacker.DamagePerSecond;

            if (HasAdvantage(attackerUnit.UnitType, UnitType))
                return attacker.DamagePerSecond * Bootstrap.Instance.GameData.RockPaperScissorsDamageMod;

            return attacker.DamagePerSecond;
        }
        
        private void BuildPathCache()
        {
            if (_positions == null || _positions.Count < 2)
            {
                _segmentLengths = null;
                _totalLength = 0f;
                return;
            }

            _segmentLengths = new float[_positions.Count - 1];
            _totalLength = 0f;

            for (int i = 0; i < _positions.Count - 1; i++)
            {
                float len = Vector2.Distance(_positions[i], _positions[i + 1]);
                _segmentLengths[i] = len;
                _totalLength += len;
            }
        }
        
        private static bool HasAdvantage(UnitType attacker, UnitType defender)
        {
            // attacker wins if it is next in cycle: (defender + 1) % 3
            return attacker == (UnitType)(((int)defender + 1) % 3);
        }

        private bool TryMove()
        {
            if (_positions == null || _positions.Count < 2 || _segmentLengths == null)
                return false;

            // Двигаемся по длине пути
            _distanceTraveled += Speed * Time.deltaTime;

            // Дошли до конца
            if (_distanceTraveled >= _totalLength)
            {
                transform.position = _positions[^1];
                return false;
            }

            // Продвигаем индекс сегмента, пока не попадём в актуальный
            float dist = _distanceTraveled;
            int idx = _segmentIndex;

            while (idx < _segmentLengths.Length && dist > _segmentLengths[idx])
            {
                dist -= _segmentLengths[idx];
                idx++;
            }

            // на всякий, если что-то пошло не так
            idx = Mathf.Clamp(idx, 0, _segmentLengths.Length - 1);
            _segmentIndex = idx;

            float segLen = _segmentLengths[idx];
            float t = segLen > 0f ? dist / segLen : 1f;

            Vector2 a = _positions[idx];
            Vector2 b = _positions[idx + 1];
            Vector2 pos = Vector2.Lerp(a, b, t);

            transform.position = pos;
            return true;
        }
        
        public override void Animate(ProcessFrameResult result) 
            => CurrentAnimation.Animate(result);
    }
}