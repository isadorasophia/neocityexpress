using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using FMOD;
using LDGame.Assets;
using LDGame.Components;
using LDGame.Messages;
using LDGame.Services;
using LDGame.StateMachines;
using Murder;
using System.Collections.Immutable;

namespace LDGame.Systems.Ui
{
    [DoNotPause]
    [Filter(typeof(CellphoneLineComponent))]
    [Watch(typeof(CellphoneLineComponent))]
    public class CellphoneUiSystem : IReactiveSystem, IStartupSystem
    {
        Entity? _cellphoneEntity;

        public void Start(Context context)
        {
            Game.Instance.ActiveScene?.RenderContext?.
                CreateMaskForDimensions(
                    new(CellphoneStateMachine.MaskDimensions.X, 
                        CellphoneStateMachine.MaskDimensions.Y - 36), isSmaller: true);

            Game.Instance.ActiveScene?.RenderContext?.
                CreateMaskForDimensions(
                    new(TinderStateMachine.MaskDimensions.X,
                        TinderStateMachine.MaskDimensions.Y), isSmaller: false);
        }

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            _cellphoneEntity ??= world.AddEntity();

            Entity e = entities[0];

            _cellphoneEntity.SetStateMachine(new StateMachineComponent<CellphoneStateMachine>(
                new CellphoneStateMachine(e.GetCellphoneLine().Owner)));

            _cellphoneEntity.SendMessage<TargetEntityMessage>(new() { Entity = e });

            SendTextMessage(e);

        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            Entity e = entities[0];
            SendTextMessage(e);
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            _cellphoneEntity?.SendMessage(TextMessage.CreateClear());
        }

        private void SendTextMessage(Entity e)
        {
            CellphoneLineComponent cellphone = e.GetCellphoneLine();

            TextMessage message;

            LibraryAsset library = LibraryServices.GetRoadLibrary();

            bool isOurs = cellphone.Line.Speaker == library.DriverSpeaker;
            string sender = isOurs ? string.Empty : cellphone.Owner;

            if (cellphone.Choices is ImmutableArray<string> choices)
            {
                message = TextMessage.Create(cellphone.Line.Text!, choices, sender);
            }
            else
            {
                message = TextMessage.Create(cellphone.Line.Text!, isOurs: isOurs, sender);
            }

            _cellphoneEntity?.SendMessage(message);
        }
    }
}
