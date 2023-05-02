using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Bang.Systems;
using LDGame.Components;
using Murder.Messages;

namespace LDGame.Systems.Interactions
{
    [Filter(typeof(InteractorComponent))]
    [Messager(typeof(InteractorMessage))]
    internal class InteractOnButtonPressSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            //if (entity.TryGetCollisionCache() is not CollisionCacheComponent isColliding)
            //{
            //    return;
            //}

            //IEnumerable<Entity> others = isColliding.GetCollidingEntities(world).Where((a) => a.HasInteractOnButtonPress())
            //  .OrderByDescending(a => a.GetInteractOnButtonPress().Priority);

            //IEnumerable<Entity> others = world.GetEntitiesWith(typeof(InteractOnButtonPressComponent));

            //foreach (Entity other in others)
            //{
            //    var component = other.GetInteractOnButtonPress();

            //    //if (component.Duration > 0)
            //    //{
            //    //    other.RunCoroutine(WaitForButtonHeld(other, component, entity));
            //    //}
            //    //else
            //    //{
            //    other.SendMessage(new InteractMessage(entity));

            //    // Interaction successful! Stop here.
            //    return;
            //}
        }
    }
}
