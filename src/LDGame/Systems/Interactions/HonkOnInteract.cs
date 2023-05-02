using Bang;
using Bang.Entities;
using Bang.Interactions;
using LDGame.Core.Sounds;
using LDGame.Services;
using Murder;
using Murder.Utilities;

namespace LDGame.Systems.Interactions;

internal class HonkOnInteract : Interaction
{
    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        if (Game.Random.TryWithChanceOf(0.75f))
            LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().Horn, isLoop: false);
    }
}