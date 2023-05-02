using Bang.Contexts;
using Bang.Systems;
using Murder;

namespace LDGame.Systems
{
    internal class GameOverSystem : IStartupSystem
    {
        public void Start(Context context)
        {
            // Restore previous save.
            Game.Data.ResetActiveSave();
        }
    }
}
