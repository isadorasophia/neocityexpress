using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Core;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Core.Geometry;
using Murder.Editor.CustomFields;
using Murder;
using Murder.Editor.Reflection;

namespace LDGame.Editor.CustomFields
{
    [CustomFieldOf(typeof(Portrait))]
    internal class PortraitField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            Portrait portrait = (Portrait)fieldValue!;

            bool fileChanged = false;
            if (DrawPortrait($"item_{member.Name}", new(25, 25), ref portrait, description: "display portrait"))
            {
                fileChanged = true;
            }

            return (fileChanged, portrait);
        }
        
        private static bool _pressed = false;

        public static bool DrawPortrait(string id, Vector2 size, ref Portrait portrait, string description)
        {
            if (Game.Data.TryGetAsset<SpriteAsset>(portrait.Aseprite) is not SpriteAsset aseprite)
            {
                _pressed = ImGuiHelpers.IconButton('\uf576', $"##{id}");
            }
            else
            {
                _pressed = EditorAssetHelpers.DrawPrettyPreviewButton(
                    aseprite, portrait.AnimationId, size, pressed: _pressed);
            }

            ImGuiHelpers.HelpTooltip($"Modify or add a {description}.");
            
            string popupName = $"##{id}_portrait";

            if (_pressed)
            {
                ImGui.OpenPopup(popupName);
            }
            
            return DrawReplacePortraitPopup(id, popupName, ref portrait);
        }

        private static bool DrawReplacePortraitPopup(string id, string popupName, ref Portrait portrait)
        {
            bool modified = false;

            if (ImGui.BeginPopup(popupName))
            {
                ImGui.BeginChild($"{id}_new_popup", new(200, 44));

                modified = DrawValue(ref portrait, nameof(Portrait.Aseprite));
                if (modified)
                {
                    string? firstAnimation = Game.Data.GetAsset<SpriteAsset>(portrait.Aseprite).Animations.Keys.FirstOrDefault();
                    portrait = portrait.WithAnimationId(firstAnimation ?? string.Empty);
                }

                ImGui.PushItemWidth(-1);
                
                // Draw combo box for the animation id
                if (portrait.Aseprite == Guid.Empty)
                {
                    ImGui.TextColored(Game.Profile.Theme.Faded, "No animations yet :-(");
                }
                else
                {
                    string animation = portrait.AnimationId;
                    if (EditorAssetHelpers.DrawComboBoxFor(portrait.Aseprite, ref animation))
                    {
                        portrait = portrait.WithAnimationId(animation);
                        modified = true;
                    }
                }

                ImGui.PopItemWidth();

                ImGui.EndChild();
                ImGui.EndPopup();
            }

            return modified;
        }
    }
}
