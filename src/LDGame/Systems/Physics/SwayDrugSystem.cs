using Bang.Contexts;
using Bang.Systems;
using LDGame.Core;
using LDGame.Services;
using Murder;
using Murder.Utilities;

namespace LDGame.Systems.Physics
{
    internal class SwayDrugSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            var save = SaveServices.GetOrCreateSave();

            foreach (var mod in save.Modifiers)
            {
                save.SwayDirection += mod.ConstantInput * Game.FixedDeltaTime;
            }

            if (!save.HasSway || !save.GameplayBlackboard.HyperXEnabled)
                return;

            if (Game.Input.Down(InputButtons.Space))
            {
                save.SwayDirection.X += Game.FixedDeltaTime* (0.5f + MathF.Sin(Game.Now*10f) * 0.5f);
            }
            save.SwayDirection.X = Math.Clamp(save.SwayDirection.X, -0.5f, 0.5f);
        }
    }
}