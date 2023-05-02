using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using LDGame.Components;
using LDGame.Services;
using Murder;
using Murder.Components;

namespace LDGame.Systems;

[Filter(typeof(CarComponent))]
[Filter(ContextAccessorFilter.AnyOf, typeof(VelocityComponent), typeof(AgentImpulseComponent))]
[Filter(ContextAccessorFilter.NoneOf, typeof(DisableAgentComponent))]
internal class CarCleanupService : IFixedUpdateSystem
{
    public void FixedUpdate(Context context)
    {
        var save = SaveServices.GetOrCreateSave();
        bool hasTheGoodShit = false;
        foreach (var item in save.Modifiers)
        {
            if (item.TheGoodShit)
            {
                hasTheGoodShit = true;
                break;
            }
        }

        foreach (var e in context.Entities)
        {
            var agent = e.GetCar();
            

            if (e.TryGetCarEngineStopped() is CarEngineStoppedComponent carEngine)
            {
                e.RemoveFriction();
                if (carEngine.StopUntil <= Game.Now)
                {
                    e.RemoveCarEngineStopped();
                }
            }
            else
            {
                if (!e.RemoveAgentImpulse())     // Cleanup the impulse
                {
                    // Set the friction if there is no impulse
                    
                    if (e.HasPlayer() && hasTheGoodShit)
                    {
                        e.SetFriction(0.3f);
                    }
                    else
                    {
                        e.SetFriction(agent.Friction);
                    }
                }
            }
        }
    }
}
