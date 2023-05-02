using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using LDGame.Components;
using LDGame.Core.Sounds;
using LDGame.Services;
using Murder;

namespace LDGame.Systems;

[Filter(typeof(BoostComponent))]
internal class RemoveBoostSystem : IFixedUpdateSystem
{
    public void FixedUpdate(Context context)
    {
        foreach (var e in context.Entities)
        {
            if (e.GetBoost().StopWhen < Game.Now)
            {
                e.RemoveBoost();
                LDGameSoundPlayer.Instance.Stop(LibraryServices.GetRoadLibrary().CarBoostLoop, true);
                LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().CarBoostEnd, isLoop: false);
                
            }
        }
    }
}