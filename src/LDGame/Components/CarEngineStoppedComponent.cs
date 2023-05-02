using Bang.Components;

namespace LDGame.Components
{
    public readonly struct CarEngineStoppedComponent : IComponent
    {
        public readonly float StopUntil = 0f;

        public CarEngineStoppedComponent()
        {
        }

        public CarEngineStoppedComponent(float stopUntil)
        {
            StopUntil = stopUntil;
        }
    }
}