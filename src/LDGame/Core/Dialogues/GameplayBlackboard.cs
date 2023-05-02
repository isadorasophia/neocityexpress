using Murder.Core.Dialogs;

namespace LDGame.Core
{
    [Blackboard(Name, @default: true)]
    public class GameplayBlackboard : IBlackboard
    {
        public const string Name = "Gameplay";

        public bool ScaredByAlmostAccident = false;

        public bool FinishedRaceOfDay = false;

        public bool PanickedAndCantReply = false;

        public bool NearMissEnabled = false;

        public int NearMissCount = 0;

        internal bool HyperXEnabled = false;

        public bool ShowGranny = false;

        public bool TrueEndingUnlocked = false;
    }
}
