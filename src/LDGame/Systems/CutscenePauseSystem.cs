using Bang.Contexts;
using Bang.Systems;
using LDGame.Core;
using LDGame.Core.Sounds;
using LDGame.Services;
using Murder;

namespace LDGame.Systems
{
    internal class CutscenePauseSystem : IUpdateSystem
    {
        public void Update(Context context)
        {
            if (Game.Input.Pressed(InputButtons.Pause) && !context.World.IsPaused)
            {
                LibraryServices.GetPausePrefab().Create(context.World);

                Game.Input.Consume(InputButtons.Pause);
                Game.Input.Consume(InputButtons.Cancel);

                LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().UiBack, isLoop: false);
            }
        }
    }
}
