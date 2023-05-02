using LDGame.Core;
using Murder.Assets;
using Murder.Attributes;
using Murder.Components;
using Murder.Core.Geometry;

namespace LDGame.Assets
{
    public class LdSaveData : SaveData
    {
        private GameplayBlackboard? _gameplayBlackboard = null;

        /// <summary>
        /// Next game level world which will be triggered from here.
        /// </summary>
        [GameAssetId<DayCycleAsset>]
        public Guid? NextLevel = null;

        public List<Modifier> Modifiers = new();

        /// <summary>
        /// Next level cutscenes that will be triggered, in order.
        /// </summary>
        public SituationComponent? NextLevelCutscene = null;
        
        public int Health = 5;
        
        public float TraveledDistance;
        public int Day;

        /// <summary>
        /// Tinder id matched so we can refresh our pool.
        /// </summary>
        public int TinderIdMatched = -1;
        internal Vector2 SwayDirection;
        internal bool HasSway;
        internal bool Sleepy;
        internal int MessagesSent = 0;

        /// <summary>
        /// Retrieves the gameplay blackboard.
        /// This should be used on a *readonly setting*.
        /// </summary>
        internal GameplayBlackboard GameplayBlackboard =>
            _gameplayBlackboard ??= (GameplayBlackboard)BlackboardTracker.FindBlackboard(null, guid: null)!.Blackboard;

        public LdSaveData(string name) : base(name) { }
    }
}
