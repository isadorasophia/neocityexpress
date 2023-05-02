using Bang.Entities;
using Bang.StateMachines;
using Bang;
using LDGame.StateMachines;
using Murder.Components;
using Murder.Services;
using LDGame.Components;
using Murder;
using LDGame.Assets;
using Murder.Assets.Graphics;
using Murder.Core;

namespace LDGame.Services
{
    public static class DialogueServices
    {
        /// <param name="profilesGuid">List of profiles that will appear in the tinder app.</param>
        /// <param name="tracker">Optional tracker, when some events are hard tracked by the story.</param>
        public static void TriggerDating(World world, Guid profilesGuid, TriggeredEventTrackerComponent? tracker = null)
        {
            Entity entity = world.AddEntity();

            if (tracker is not null)
            {
                entity.SetTriggeredEventTracker(tracker.Value);
            }

            DatingProfileAsset asset = Game.Data.GetAsset<DatingProfileAsset>(profilesGuid);
            entity.SetStateMachine(new StateMachineComponent<TinderStateMachine>(new TinderStateMachine(asset.Profiles, asset.ThresholdForStartMatching)));
        }

        /// <param name="tracker">Optional tracker, when some events are hard tracked by the story.</param>
        public static void TriggerDialogue(World world, SituationComponent situation, InputType inputType, MessageType messageType, TriggeredEventTrackerComponent? tracker = null, bool canInterrupt = true)
        {
            world.RunCoroutine(DoTalkIntro(world, situation, inputType, messageType, tracker));
        }

        private static IEnumerator<Wait> DoTalkIntro(World world, SituationComponent situation, InputType inputType, MessageType messageType, TriggeredEventTrackerComponent? tracker = null, bool canInterrupt = true)
        {
            if (world.TryGetUniqueEntity<MonologueComponent>() is Entity existingEntity && 
                messageType == MessageType.Monologue)
            {
                if (canInterrupt)
                {
                    // uhhhhhhh i wonder what could possibly backfire here?
                    existingEntity.Destroy();
                }
                else
                {
                    // don't do anything, we are not interrupting it.
                    yield break;
                }
            }

            Entity dialogEntity = world.AddEntity();

            dialogEntity.SetSituation(situation);
            dialogEntity.SetStateMachine(new StateMachineComponent<MonologueStateMachine>(new MonologueStateMachine(inputType, messageType)));
            dialogEntity.SetDoNotPause();

            if (tracker is not null)
            {
                dialogEntity.SetTriggeredEventTracker(tracker.Value);
            }

            yield return Wait.Stop;
        }

        public static (SpriteAsset Asset, string Animation)? GetSpriteAssetForSituation(Portrait portrait)
        {
            if (Game.Data.TryGetAsset<SpriteAsset>(portrait.Aseprite) is SpriteAsset asset)
            {
                return (asset, portrait.AnimationId);
            }

            return null;
        }
    }
}
