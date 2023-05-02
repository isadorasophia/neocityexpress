using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using LDGame.Assets;
using LDGame.Components;
using LDGame.Core;
using LDGame.Messages;
using LDGame.Services;
using LDGame.StateMachines;
using Murder;
using Murder.Core.Dialogs;
using Murder.Core.Graphics;
using System.Collections.Immutable;

namespace LDGame.Systems
{
    [DoNotPause]
    [Filter(typeof(MonologueComponent))]
    [Watch(typeof(MonologueComponent))]
    public class MonologueUiSystem : IReactiveSystem
    {
        Entity? _monologueEntity;

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            Game.Input.Consume(InputButtons.Submit);

            _monologueEntity ??= world.AddEntity();

            Entity e = entities[0];

            _monologueEntity.SetStateMachine(new StateMachineComponent<MonologueUiStateMachine>());
            _monologueEntity.SendMessage<TargetEntityMessage>(new() { Entity = e });

            SendDialogue(e);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            Game.Input.Consume(InputButtons.Submit);

            Entity e = entities[0];
            SendDialogue(e);
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            _monologueEntity?.SendMessage(DialogueMessage.CreateClear());
        }

        private void SendDialogue(Entity e)
        {
            LibraryAsset library = LibraryServices.GetRoadLibrary();

            SpeakerKind speaker;
            Line line = e.GetMonologue().Line;

            if (line.Speaker == library.DriverSpeaker)
            {
                speaker = SpeakerKind.Driver;
            }
            else if (line.Speaker == library.GrannySpeaker)
            {
                speaker = SpeakerKind.Granny;
            }
            else
            {
                speaker = SpeakerKind.Passenger;
            }

            _monologueEntity?.SendMessage(new DialogueMessage(e.GetMonologue(), speaker));
        }
    }
}
