using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using LDGame.Components;
using LDGame.Core;
using LDGame.Core.Sounds;
using LDGame.Messages;
using LDGame.Services;
using Murder;
using Murder.Diagnostics;
using Murder.Services;
using Murder.Utilities;

namespace LDGame.Systems.Interactions
{
    
    [Filter(typeof(CarComponent))]
    [Messager(typeof(CarCollisionMessage))]
    internal class OnCarCrashSystem : IMessagerSystem
    {
        private float _lastDamage = 0;
        
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            var msg = (CarCollisionMessage)message;
            entity.SetCarEngineStopped(Game.Now + 0.5f);

            if (entity.HasPlayer())
            {
                LibraryServices.Explode(0, world, msg.Center);

                SaveServices.SetGameplayValue(nameof(GameplayBlackboard.ScaredByAlmostAccident), true);
                entity.SetPlayerSpeed(entity.GetPlayerSpeed().Lower(0.5f));
                
                var save = SaveServices.GetOrCreateSave();
                if (save.Health <= 2)
                {
                    entity.TryFetchChild("Smoke")?.RemoveDisableParticleSystem();
                    LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().Fire, isLoop: true, stopLastMusic: false);
                }

                if (_lastDamage < Game.Now - 1.0f && --save.Health<=0)
                {
                    LDGameSoundPlayer.Instance.Stop(LibraryServices.GetRoadLibrary().Fire, true);
                    
                    GameLogger.Log("Player died.");

                    entity.RemoveCollider();

                    SaveServices.OnDeath(world, seconds: 2);
                    CoroutineServices.RunCoroutine(entity, Stop(entity));
                    CoroutineServices.RunCoroutine(entity, ExplodeAndDestroy(world, entity));
                }
                _lastDamage = Game.Now;
            }
        }

        private IEnumerator<Wait> Stop(Entity entity)
        {
            while (entity.GetPlayerSpeed().Speed > 0)
            {
                entity.SetPlayerSpeed(entity.GetPlayerSpeed().Brake(Game.DeltaTime));
                yield return Wait.NextFrame;
            }
        }

        private IEnumerator<Wait> ExplodeAndDestroy(World world, Entity entity)
        {
            entity.RemoveCollider();
            entity.RemovePlayer();

            for (int i = 0; i < 18; i++)
            {
                var position = entity.GetGlobalTransform().Vector2;
                LibraryServices.Explode(1, world, position + Calculator.RandomPointInCircleEdge() * 24f);
                yield return Wait.ForMs(200);
            }

            LibraryServices.Explode(0, world, entity.GetGlobalTransform().Vector2);
            entity.Destroy();

            SaveServices.OnDeath();
        }
    }
}
