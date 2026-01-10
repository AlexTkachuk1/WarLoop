using System.Collections.Generic;
using System.Linq;
using _Scripts.Actors.Buildings;
using _Scripts.AI;
using _Scripts.Controllers;
using _Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Actors
{
    public class GameLoopUpdater : StaticInstance<GameLoopUpdater>
    {
        public List<Actor> Actors { get; } = new();
        private List<BuildingStrategy> _strategies = new();

        public bool PlayerIsAlive => Actors.Any(x => x.Faction == FactionColor.Blue);
        public bool EnemiesIsAlive => Actors.Any(x => x.Faction != FactionColor.Blue);
        
        private bool GameIsOver = false;
        
        public void Add(Actor actor)
        {
            Actors.Add(actor);
            if (actor is Building building)
            {
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
            if (GameIsOver) return;
            
            UpdateActors();
            UpdateAI();

            if (!PlayerIsAlive)
            {
                GameIsOver = true;
                EndGameWindow.Instance.ShowLoseScreen();
            }

            if (!EnemiesIsAlive)
            {
                GameIsOver = true;
                EndGameWindow.Instance.ShowWinScreen();
            }
        }

        private void UpdateAI()
        {
            foreach (var strategy in _strategies)
            {
                if (strategy.Building.Faction == GameplayController.Instance.PlayerColor)
                    continue;
                if (strategy.Building.Faction == FactionColor.Black)
                    continue;
                if (strategy.Building.Faction == FactionColor.Gold)
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
                
                if (actor.HealthPercentage <= 0 || actor.Faction == FactionColor.Gold)
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