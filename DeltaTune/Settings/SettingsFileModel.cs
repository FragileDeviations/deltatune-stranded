using Microsoft.Xna.Framework;

namespace DeltaTune.Settings
{
    public class SettingsFileModel
    {
        public int ScaleFactor { get; set; } = 1;
        public Vector2 Position { get; set; } = PositionPresetHelper.GetFractionalPosition(PositionPreset.Original);
        public bool ShowArtistName { get; set; } =  true;
        public bool ShowPlaybackStatus { get; set; } = false;
        public bool HideAutomatically { get; set; } = true;

        public void FromSettings(ISettingsService settingsService)
        {
            ScaleFactor = settingsService.ScaleFactor.Value;
            Position = settingsService.Position.Value;
            ShowArtistName = settingsService.ShowArtistName.Value;
            ShowPlaybackStatus = settingsService.ShowPlaybackStatus.Value;
            HideAutomatically = settingsService.HideAutomatically.Value;
        }

        public void ToSettings(ISettingsService settingsService)
        {
            settingsService.ScaleFactor.Value = ScaleFactor;
            settingsService.Position.Value = Position;
            settingsService.ShowArtistName.Value = ShowArtistName;
            settingsService.ShowPlaybackStatus.Value = ShowPlaybackStatus;
            settingsService.HideAutomatically.Value = HideAutomatically;
        }
    }
}