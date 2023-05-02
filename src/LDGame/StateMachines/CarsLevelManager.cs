using Bang;
using Bang.Entities;
using Bang.Interactions;
using Bang.StateMachines;
using LDGame.Assets;
using LDGame.Components;
using LDGame.Core;
using LDGame.Interactions;
using LDGame.Services;
using Murder;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;

namespace LDGame.StateMachines
{
    public class CarsLevelManager : StateMachine
    {
        private int _currentPossibleCar = 0;

        private ImmutableArray<Guid> _possibleCarsShuffled = ImmutableArray<Guid>.Empty;
        private readonly ImmutableArray<Guid> _possibleCars = new();

        public CarsLevelManager(ImmutableArray<Guid> possibleCars)
        {
            _possibleCars = possibleCars;
            GetNextCarEvent();
            State(Level);
        }

        public CarsLevelManager()
        {
            State(Level);
        }

        public void GetNextCarEvent()
        {
            if (_currentPossibleCar >= _possibleCarsShuffled.Length)
            {
                // Time to shuffle
                _currentPossibleCar = 0;
                _possibleCarsShuffled = _possibleCars.Shuffle();
            }
        }

        private IEnumerator<Wait> Level()
        {
            // "Tag" ourselves with the car level manager.
            Entity.SetCarLevelManager();

            var library = LibraryServices.GetRoadLibrary();
            yield return Wait.ForSeconds(1.8f);

            if (_possibleCarsShuffled.Length == 0)
            {
                GameLogger.Warning("There are no cars here! Was this intentional?");
                yield break;
            }

            while (true)
            {
                if (_currentPossibleCar >= _possibleCarsShuffled.Length)
                    GetNextCarEvent();
                
                RoadCarsAsset level = Game.Data.GetAsset<RoadCarsAsset>(_possibleCarsShuffled[_currentPossibleCar]);
                _currentPossibleCar++;
                
                int current = 0;
                float time = 0;
                while (current < level.Events.Length)
                {
                    var currentEvent = level.Events[current];
                    time += Game.DeltaTime;
                    float target = time + currentEvent.WaitTime * 4f;

                    while (time < target)
                    {
                        time += Game.DeltaTime;
                        yield return Wait.NextFrame;
                    }

                    // Show warning
                    if (currentEvent.Warning)
                    {
                        SpawnWarning(library.Warning, currentEvent.Position, currentEvent.Lane);
                    }

                    target = time + currentEvent.WaitTime;

                    while (time < target)
                    {
                        time += Game.DeltaTime;
                        yield return Wait.NextFrame;
                    }

                    if (currentEvent.PrefabToSpawn != Guid.Empty)
                    {
                        SpawnEntity(currentEvent.PrefabToSpawn, currentEvent.Position, currentEvent.Lane);
                    }

                    current++;
                    yield return Wait.NextFrame;

                }

                while (World.GetEntitiesWith(typeof(EnemyComponent)).Any())
                {
                    yield return Wait.NextFrame;
                }
                yield return Wait.ForSeconds(0.5f);
            }
        }

        private void SpawnWarning(Guid prefabToSpawn, LanePosition lanePosition, int lane)
        {
            var prefab = Game.Data.GetPrefab(prefabToSpawn);
            var entity = prefab.CreateAndFetch(World);

            var library = LibraryServices.GetRoadLibrary();
            float xPosition = library.GetLanePosition(lane);

            switch (lanePosition)
            {
                case LanePosition.Top:
                    entity.SetGlobalPosition(new Vector2(xPosition, library.Bounds.Top + 15));
                    entity.SetFacing(Murder.Helpers.Direction.Down);
                    break;

                case LanePosition.Bottom:
                    entity.SetGlobalPosition(new Vector2(xPosition, library.Bounds.Bottom - 20));
                    entity.SetFacing(Murder.Helpers.Direction.Up);
                    break;
                case LanePosition.TopReverse:
                    entity.SetGlobalPosition(new Vector2(xPosition, library.Bounds.Top + 15));
                    entity.SetFacing(Murder.Helpers.Direction.Down);
                    break;
                default:
                    break;
            }
        }
        private void SpawnEntity(Guid prefabToSpawn, LanePosition lanePosition, int lane)
        {
            var prefab = Game.Data.GetPrefab(prefabToSpawn);
            var entity = prefab.CreateAndFetch(World);

            LibraryAsset library = LibraryServices.GetRoadLibrary();
            float xPosition = library.GetLanePosition(lane);
            var car = entity.GetCar();

            Vector2 direction;
            switch (lanePosition)
            {
                case LanePosition.Top:
                    entity.SetGlobalPosition(new Vector2(xPosition, library.Bounds.Top - 80));
                    direction = new Vector2(0, 2f);
                    entity.SetFacing(Murder.Helpers.Direction.Down);
                    break;

                case LanePosition.Bottom:
                    entity.SetGlobalPosition(new Vector2(xPosition, library.Bounds.Bottom + 150));
                    direction = new Vector2(0,-7f);
                    break;
                case LanePosition.TopReverse:
                    entity.SetGlobalPosition(new Vector2(xPosition, library.Bounds.Top - 10));
                    direction = new Vector2(0, -1f);
                    entity.SetFacing(Murder.Helpers.Direction.Up);
                    break;
                default:
                    throw new Exception("unknown direction");
            }

            entity.SetCarEngine(direction, 0.1f);
            entity.SetRelativeVelocity(direction * car.Speed);

            bool isNearMissEnabled = SaveServices.GetOrCreateSave().GameplayBlackboard.NearMissEnabled;
            if (!isNearMissEnabled)
            {
                entity.TryFetchChild("NearMiss")?.Destroy();
            }
        }
    }
}
