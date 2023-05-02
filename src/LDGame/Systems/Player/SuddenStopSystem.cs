using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using LDGame.Components;
using LDGame.Services;
using Murder;
using Murder.Utilities;

namespace LDGame.Systems;

[Filter(typeof(SuddenStopComponent), typeof(PlayerComponent))]
public class SuddenStopSystem : IFixedUpdateSystem
{
    public void FixedUpdate(Context context)
    {
        var save = SaveServices.GetOrCreateSave();
        float nextStopRange = float.MaxValue;

        foreach (var mod in save.Modifiers)
        {
            if (mod.SuddenBreakEvery>0)
                nextStopRange = Math.Min(nextStopRange, mod.SuddenBreakEvery);
        }

        if (!context.HasAnyEntity)
        {
            Entity? player = context.World.TryGetUniqueEntity<PlayerComponent>();
            if (player is null)
            {
                return;
            }

            player.SetSuddenStop(Game.Now + Game.Random.NextFloat(nextStopRange, nextStopRange * 5));
        }
        
        foreach (var e in context.Entities)
        {
            var suddenStop = e.GetSuddenStop();

            if (Game.Now > suddenStop.When)
            {
                e.SetSuddenStop(Game.Now + Game.Random.NextFloat(nextStopRange, nextStopRange * 5));
                e.RemoveVelocity();
                e.SetPlayerSpeed(1);
            }
        }
    }
    
}