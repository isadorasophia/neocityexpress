using Bang;
using Bang.Entities;
using Bang.Interactions;
using LDGame.Core.Sounds;
using LDGame.Services;
using Murder;

namespace LDGame.Systems.Interactions
{
    public readonly struct GiveBoostInteraction : Interaction
    {
        public readonly float Duration = 0f;
        
        public GiveBoostInteraction()
        {
        }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            interactor.SetBoost(Game.Now + Duration);
            LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().CarBoostStart, isLoop: false);
//            LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().CarBoostLoop, isLoop: false);
        }
    }
}