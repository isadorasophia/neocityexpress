using Bang.Components;

namespace LDGame.Messages
{
    internal readonly struct AgentReleaseInputMessage : IMessage
    {
        public readonly int Button;
        public AgentReleaseInputMessage(int button)
        {
            this.Button = button;
        }
    }
}
