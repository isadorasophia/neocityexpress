using Bang.Entities;
using Bang;
using Bang.Interactions;
using Murder.Utilities.Attributes;
using LDGame.Services;
using Bang.StateMachines;
using Murder;
using LDGame.Messages;
using Murder.Services;
using LDGame.Core;

namespace LDGame.Interactions
{
    [RuntimeOnly]
    public readonly struct SpawnNearMissInteraction : Interaction
    {
        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            world.RunCoroutine(WaitForCrash(world, interactor));
        }

        private IEnumerator<Wait> WaitForCrash(World world, Entity player)
        {
            float wait = Game.Now;
            float interval = .35f;

            while (Game.Now - wait < interval)
            {
                if (player is null || player.HasMessage<CarCollisionMessage>())
                {
                    yield break;
                }

                yield return Wait.NextFrame;
            }

            if (player is null)
            {
                yield break;
            }

            SaveServices.AddGameplayValue(nameof(GameplayBlackboard.NearMissCount), 1);

            // All right, all looks good! Go for it.
            Guid prefab = LibraryServices.GetRoadLibrary().NearMissPrefab;

            Entity? e = AssetServices.TryCreate(world, prefab);
            e?.SetTransform(player.GetTransform().Add(new Murder.Core.Geometry.Vector2(x: 0, y: -10)));
        }
    }
}
