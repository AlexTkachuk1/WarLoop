using System.Collections.Generic;

namespace _Scripts.Actors
{
    public class ActorsUpdater : StaticInstance<ActorsUpdater>
    {
        private readonly List<Actor> _actors = new();

        public void Add(Actor actor)
        {
            _actors.Add(actor);
        }

        private void Update()
        {
            for (var i = 0; i < _actors.Count; i++)
            {
                var actor = _actors[i];
                if (!actor)
                {
                    _actors.RemoveAt(i);
                    i--;
                    continue;
                }
                
                if (actor.HealthPercentage <= 0)
                    continue;

                var result = actor.ProcessFrame();
                if (result == ProcessFrameResult.ScheduledForDisposal)
                {
                    _actors.RemoveAt(i);
                    i--;
                    continue;
                }
                
                actor.Animate(result);
            }
        }
    }
}