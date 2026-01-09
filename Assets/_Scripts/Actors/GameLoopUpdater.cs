using System.Collections.Generic;
using _Scripts.Actors.Buildings;
using _Scripts.AI;
using _Scripts.Controllers;
using _Scripts.GameLoop;
using UnityEngine;

namespace _Scripts.Actors
{
    public class GameLoopUpdater : StaticInstance<GameLoopUpdater>
    {
        public List<Actor> Actors { get; } = new();
        private List<BuildingStrategy> _strategies = new();

        public void Add(Actor actor)
        {
            Actors.Add(actor);
            if (actor is Building building)
            {
                if (building.Faction == GameplayController.Instance.PlayerColor) return;
                
                var buildingStrategy = new BuildingStrategy();
                _strategies.Add(buildingStrategy);
                buildingStrategy.Init(building, 
                    (UnitTypeStrategyType)Random.Range(0, 3),
                    (RoadSelectionStrategyType)Random.Range(0, 3),
                    (UnitCountStrategyType)Random.Range(0, 3));
            }
        }

        public void Clear()
        {
            foreach (var actor in Actors) 
                Destroy(actor.gameObject);
            Actors.Clear();
            _strategies.Clear();
        }

        private void Update()
        {
            UpdateActors();
            UpdateAI();
        }

        private void UpdateAI()
        {
            foreach (var strategy in _strategies)
            {
                if (strategy.Building.Faction == Bootstrap.Instance.GameData.Player)
                    continue;
                
                strategy.ProcessFrame();
            }
        }

        private void UpdateActors()
        {
            for (var i = 0; i < Actors.Count; i++)
            {
                var actor = Actors[i];
                if (!actor)
                {
                    Actors.RemoveAt(--i + 1);
                    continue;
                }
                
                if (actor.HealthPercentage <= 0)
                    continue;

                var result = actor.ProcessFrame();
                if (result == ProcessFrameResult.ScheduledForDisposal)
                {
                    Actors.RemoveAt(--i + 1);
                    continue;
                }
                
                actor.Animate(result);
            }
        }
    }
}