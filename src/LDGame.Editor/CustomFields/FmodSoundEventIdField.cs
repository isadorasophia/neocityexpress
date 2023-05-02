using ImGuiNET;
using LDGame.Attributes;
using LDGame.Editor.Utilities;
using Murder.Core.Sounds;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace LDGame.Editor.CustomFields
{
    [CustomFieldOf(typeof(SoundEventId))]
    internal class FmodSoundEventIdField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(
            EditorMember member, object? fieldValue)
        {
            SoundEventId description = (SoundEventId)fieldValue!;
            
            if (ImGuiHelpers.DeleteButton("No event selected."))
            {
                return (modified: true, new SoundEventId());
            }
            
            ImGui.SameLine();

            FmodIdKind kind = FmodIdKind.Event;
            if (AttributeExtensions.TryGetAttribute(member, out FmodIdAttribute? fmodId))
            {
                kind = fmodId.Kind;
            }

            switch (kind)
            {
                case FmodIdKind.Event:
                    if (SearchBoxExtensions.SearchFmodSounds(id: string.Empty, description) is SoundEventId newEvent)
                    {
                        return (modified: true, newEvent);
                    }
                    
                    break;

                case FmodIdKind.Bus:
                    if (SearchBoxExtensions.SearchFmodBuses(id: string.Empty, description) is SoundEventId newBus)
                    {
                        return (modified: true, newBus);
                    }

                    break;
            }
            
            return (modified: false, description);
        }
    }
}
