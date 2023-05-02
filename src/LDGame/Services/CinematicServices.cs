using Bang.Entities;
using Bang;
using Murder.Components;
using LDGame.Systems;
using Murder.Systems;
using Murder;
using Murder.Systems.Physics;
using LDGame.Components;
using Murder.Core.Geometry;
using Murder.Utilities;
using LDGame.StateMachines;
using LDGame.Systems.Physics;
using LDGame.Systems.Player;

namespace LDGame.Services
{
    /// <summary>
    /// This will enable a cinematic border within the world.
    /// This will not pause or freeze the game.
    /// </summary>
    public struct PartiallyPauseGame : IDisposable
    {
        private readonly World _world;

        private Entity? _freezeWorldEntity;

        public PartiallyPauseGame(World world)
        {
            _world = world;

            Freeze();
        }

        public void Dispose()
        {
            Unfreeze();
        }

        private void Freeze()
        {
            if (_world.TryGetUnique<FreezeWorldComponent>() is FreezeWorldComponent)
            {
                return;
            }

            _freezeWorldEntity = _world.AddEntity(new FreezeWorldComponent());

            _world.DeactivateSystem<PlayerInputSystem>();
            _world.DeactivateSystem<SATPhysicsSystem>();

            _world.DeactivateSystem<CarProgressSystem>();
            _world.DeactivateSystem<DayCycleSystem>();

            _world.DeactivateSystem<SwayDrugSystem>();

            Game.Instance.Pause();
        }

        private void Unfreeze()
        {
            if (_freezeWorldEntity != null && !_freezeWorldEntity.IsDestroyed)
            {
                _freezeWorldEntity.Destroy();
            }
            else
            {
                // Nothing to do.
                return;
            }

            _world.ActivateSystem<PlayerInputSystem>();
            _world.ActivateSystem<SATPhysicsSystem>();

            _world.ActivateSystem<CarProgressSystem>();
            _world.ActivateSystem<DayCycleSystem>();

            _world.ActivateSystem<SwayDrugSystem>();

            Game.Instance.Resume();
        }

        public static PartiallyPauseGame Create(World world)
        {
            return new(world);
        }
    }

    public struct StopMovingCar : IDisposable
    {
        private readonly World _world;

        private Entity? _carManager = null;
        private Entity? _freezeWorldEntity;

        public StopMovingCar(World world)
        {
            _world = world;

            Freeze();
        }

        public void Dispose()
        {
            Unfreeze();
        }

        private void Freeze()
        {
            if (_world.TryGetUnique<FreezeWorldComponent>() is FreezeWorldComponent)
            {
                return;
            }

            _freezeWorldEntity = _world.AddEntity(new FreezeWorldComponent());

            Entity? car = _world.TryGetUniqueEntity<PlayerComponent>();
            car?.SetCarEngineStopped();

            _carManager = _world.TryGetUniqueEntity<CarLevelManagerComponent>();
            _carManager?.Deactivate();

            _world.DeactivateSystem<PlayerInputSystem>();
            
            _world.DeactivateSystem<CarProgressSystem>();
            _world.DeactivateSystem<DayCycleSystem>();
            
            _world.DeactivateSystem<SwayDrugSystem>();
        }

        private void Unfreeze()
        {
            if (_freezeWorldEntity != null && !_freezeWorldEntity.IsDestroyed)
            {
                _freezeWorldEntity.Destroy();
            }
            else
            {
                // Nothing to do.
                return;
            }

            _carManager?.Activate();

            _world.ActivateSystem<PlayerInputSystem>();

            _world.ActivateSystem<CarProgressSystem>();
            _world.ActivateSystem<DayCycleSystem>();

            _world.ActivateSystem<SwayDrugSystem>();
        }
    }
}
