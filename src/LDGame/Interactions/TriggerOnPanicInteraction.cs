using Bang;
using Bang.Entities;
using Bang.Interactions;
using LDGame.Core;
using Murder.Core.Dialogs;
using Murder.Utilities.Attributes;
using Newtonsoft.Json;

namespace LDGame.Interactions
{
    // Be careful! This has not been tested as a dropped halfway through.
    [RuntimeOnly] // Hide from the editor 🙏
    public readonly struct TriggerOnPanicInteraction : Interaction
    {
        [JsonProperty]
        private readonly LdMonologueInteraction _reaction = new();

        public TriggerOnPanicInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            Entity e = world.AddEntity();

            Fact fact = new(GameplayBlackboard.Name, nameof(GameplayBlackboard.PanickedAndCantReply), FactKind.Bool);

            Criterion criterion = new(fact, CriterionKind.Is, true);
            CriterionNode criterionNode = new(criterion, CriterionNodeKind.And);

            e.SetInteractOnRuleMatch(new CriterionNode[] { criterionNode });
            e.SetInteractive(new InteractiveComponent<LdMonologueInteraction>(_reaction));

            e.SetRemoveAfterTrackedTriggeredEventIsGone();
        }
    }
}
