using Bang.Components;

namespace LDGame.Messages
{
    /// <summary>
    /// Indicates that an agent is trying to perform an action symbolized by an InputButton
    /// </summary>
    internal readonly struct AgentInputMessage : IMessage
    {
        public readonly int Button;
        public AgentInputMessage(int button)
        {
            this.Button = button;
        }
    }
}
