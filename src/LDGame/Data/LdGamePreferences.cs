using Murder;
using Murder.Save;

namespace LDGame.Data
{
    /// <summary>
    /// Tracks preferences of the current session. This is unique per run.
    /// </summary>
    public class LdGamePreferences : GamePreferences
    {
        public LdGamePreferences() : base() { }

        public override void OnPreferencesChangedImpl()
        {
            Game.Sound.SetVolume(id: LDGame.Profile.MusicBus, _musicVolume);
            // TODO: Implement sound.
            // Game.Sound.SetVolume(id: LDGame.Profile.SoundBus, _soundVolume);
        }
    }
}
