using Bang;
using Bang.Entities;
using Bang.Interactions;
using LDGame.Core.Sounds;
using Murder.Core.Sounds;

namespace LDGame.Systems.Interactions
{
    public readonly struct PlayCustomSoundInteraction : Interaction
    {
        public readonly SoundEventId Sound;
        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            LDGameSoundPlayer.Instance.PlayEvent(Sound, false);
        }
    }
}