using Bang.Interactions;
using ImGuiNET;
using Murder;
using Murder.Assets;
using Murder.Core.Dialogs;
using Murder.Editor.Attributes;
using Murder.Editor.CustomComponents;
using Murder.Editor.CustomEditors;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using LDGame.Interactions;
using System.Collections.Immutable;

namespace LDGame.Editor.CustomComponents
{
    [CustomComponentOf(typeof(InteractiveComponent<LdMonologueInteraction>))]
    public class CustomInteractiveMonologueComponent : InteractiveTalkToComponent
    {
        protected override bool DrawInteraction(object? interaction)
        {
            if (interaction is not LdMonologueInteraction talkToInteraction)
            {
                throw new ArgumentException(nameof(interaction));
            }

            using TableMultipleColumns table = new($"talk_to_interaction",
                flags: ImGuiTableFlags.SizingFixedSame | ImGuiTableFlags.BordersOuter,
                (-1, ImGuiTableColumnFlags.WidthFixed), (-1, ImGuiTableColumnFlags.WidthStretch));

            if (DrawSituationField(talkToInteraction, out int result))
            {
                EditorMember situationField = typeof(LdMonologueInteraction).
                    TryGetFieldForEditor(nameof(LdMonologueInteraction.Situation))!;

                situationField.SetValue(interaction, result);

                return true;
            }

            return DrawMembersForTarget(interaction, TalkToMembers());
        }

        /// <summary>
        /// Draw field for <see cref="RoadTalkToInteraction.Situation"/> in <see cref="RoadTalkToInteraction"/>.
        /// </summary>
        private bool DrawSituationField(in LdMonologueInteraction interaction, out int result)
        {
            result = 0;

            ImGui.TableNextRow();
            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            // -- Field --
            ImGui.Text("Situation:");

            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);

            if (Game.Data.TryGetAsset<CharacterAsset>(interaction.Character) is not CharacterAsset asset)
            {
                ImGui.TextColored(Game.Profile.Theme.Warning, "Choose a valid character first!");
                return false;
            }

            ImmutableArray<(string name, int id)> situations = CharacterEditor.FetchAllSituations(asset);
            string[] situationNames = situations.Select(s => s.name).ToArray();

            if (situationNames.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Warning, "No situation to go next?");
                return false;
            }

            int item = 0;
            if (asset.TryFetchSituation(interaction.Situation) is Situation target)
            {
                item = situations.IndexOf((target.Name, target.Id));
            }
            else
            {
                // Set a initial value for this.
                result = situations[item].id;

                return true;
            }

            if (ImGui.Combo($"##talto_situation", ref item, situationNames, situationNames.Length))
            {
                result = situations[item].id;
                return true;
            }

            ImGui.PopItemWidth();
            return false;
        }

        private IList<(string, EditorMember)>? _cachedMembers = null;

        private IList<(string, EditorMember)> TalkToMembers()
        {
            if (_cachedMembers is null)
            {
                HashSet<string> skipFields = new string[] { "Situation" }.ToHashSet();

                _cachedMembers ??= typeof(LdMonologueInteraction).GetFieldsForEditor()
                    .Where(t => !skipFields.Contains(t.Name)).Select(f => (f.Name, f)).ToList();
            }

            return _cachedMembers;
        }
    }
}