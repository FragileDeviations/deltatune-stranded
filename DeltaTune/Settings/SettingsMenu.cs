using System.Windows.Forms;

namespace DeltaTune.Settings
{
    public class SettingsMenu : ISettingsMenu
    {
        private readonly ISettingsService settingsService;
        private ContextMenuStrip settingsMenuStrip;

        public SettingsMenu(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public ContextMenuStrip GetSettingsMenu()
        {
            settingsMenuStrip = new ContextMenuStrip();
            settingsMenuStrip.AutoClose = true;
            settingsMenuStrip.TopLevel = true;

            settingsMenuStrip.Opening += (sender, args) =>
            {
                settingsMenuStrip.Items.Clear();
                
                ToolStripMenuItem headingItem = new ToolStripMenuItem();
                headingItem.Text = "Stranded On The Beach";
                headingItem.Enabled = false;
                settingsMenuStrip.Items.Add(headingItem);
            
                settingsMenuStrip.Items.Add(new ToolStripSeparator());
            
                settingsMenuStrip.Items.Add(GetPositionMenuItem());
                // settingsMenuStrip.Items.Add(GetScaleMenuItem());
                settingsMenuStrip.Items.Add(GetBehaviorMenuItem());
                
                settingsMenuStrip.Items.Add(new ToolStripSeparator());
                
                ToolStripMenuItem aboutItem = new ToolStripMenuItem();
                aboutItem.Text = "About...";
                aboutItem.Click += (o, eventArgs) => ShowAboutScreen();
                settingsMenuStrip.Items.Add(aboutItem);

                ToolStripMenuItem quitItem = new ToolStripMenuItem();
                quitItem.Text = "Quit";
                quitItem.Click += (o, eventArgs) => Application.Exit();
                settingsMenuStrip.Items.Add(quitItem);
            };
            
            return settingsMenuStrip;
        }

        private ToolStripMenuItem GetPositionMenuItem()
        {
            ToolStripMenuItem positionItem = new ToolStripMenuItem();
            positionItem.Text = "Position";
            
            ToolStripMenuItem topLeftItem = new ToolStripMenuItem();
            topLeftItem.Text = "Top Left";
            topLeftItem.Checked = PositionPresetHelper.GetFractionalPosition(PositionPreset.TopLeft) == settingsService.Position.Value;
            topLeftItem.Click += (sender, args) => settingsService.Position.Value = PositionPresetHelper.GetFractionalPosition(PositionPreset.TopLeft);
            positionItem.DropDownItems.Add(topLeftItem);
            
            ToolStripMenuItem topRightItem = new ToolStripMenuItem();
            topRightItem.Text = "Top Right";
            topRightItem.Checked = PositionPresetHelper.GetFractionalPosition(PositionPreset.TopRight) == settingsService.Position.Value;
            topRightItem.Click += (sender, args) => settingsService.Position.Value = PositionPresetHelper.GetFractionalPosition(PositionPreset.TopRight);
            positionItem.DropDownItems.Add(topRightItem);
            
            ToolStripMenuItem bottomLeftItem = new ToolStripMenuItem();
            bottomLeftItem.Text = "Bottom Left";
            bottomLeftItem.Checked = PositionPresetHelper.GetFractionalPosition(PositionPreset.BottomLeft) == settingsService.Position.Value;
            bottomLeftItem.Click += (sender, args) => settingsService.Position.Value = PositionPresetHelper.GetFractionalPosition(PositionPreset.BottomLeft);
            positionItem.DropDownItems.Add(bottomLeftItem);
            
            ToolStripMenuItem bottomRightItem = new ToolStripMenuItem();
            bottomRightItem.Text = "Bottom Right";
            bottomRightItem.Checked = PositionPresetHelper.GetFractionalPosition(PositionPreset.BottomRight) == settingsService.Position.Value;
            bottomRightItem.Click += (sender, args) => settingsService.Position.Value = PositionPresetHelper.GetFractionalPosition(PositionPreset.BottomRight);
            positionItem.DropDownItems.Add(bottomRightItem);
            
            ToolStripMenuItem originalItem = new ToolStripMenuItem();
            originalItem.Text = "Original";
            originalItem.Checked = PositionPresetHelper.GetFractionalPosition(PositionPreset.Original) == settingsService.Position.Value;
            originalItem.Click += (sender, args) => settingsService.Position.Value = PositionPresetHelper.GetFractionalPosition(PositionPreset.Original);
            positionItem.DropDownItems.Add(originalItem);

            return positionItem;
        }
        
        private ToolStripMenuItem GetScaleMenuItem()
        {
            ToolStripMenuItem scaleItem = new ToolStripMenuItem();
            scaleItem.Text = "Size";

            for (int i = 1; i < 9; i++)
            {
                var factor = i;
                
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = $"{factor}";
                item.Checked = settingsService.ScaleFactor.Value == factor;
                item.Click += (sender, args) => settingsService.ScaleFactor.Value = factor;
                scaleItem.DropDownItems.Add(item);
            }
            
            return scaleItem;
        }

        private ToolStripMenuItem GetBehaviorMenuItem()
        {
            ToolStripMenuItem behaviorItem = new ToolStripMenuItem();
            behaviorItem.Text = "Behavior";
            
            // ToolStripMenuItem showArtistNameItem = new ToolStripMenuItem();
            // showArtistNameItem.Text = "Show Artist Name";
            // showArtistNameItem.Checked = settingsService.ShowArtistName.Value;
            // showArtistNameItem.Click += (sender, args) => settingsService.ShowArtistName.Value = !settingsService.ShowArtistName.Value;
            // behaviorItem.DropDownItems.Add(showArtistNameItem);
            
            // ToolStripMenuItem showPlaybackStatusItem = new ToolStripMenuItem();
            // showPlaybackStatusItem.Text = "Show Playback Status";
            // showPlaybackStatusItem.Checked = settingsService.ShowPlaybackStatus.Value;
            // showPlaybackStatusItem.Click += (sender, args) => settingsService.ShowPlaybackStatus.Value = !settingsService.ShowPlaybackStatus.Value;
            // behaviorItem.DropDownItems.Add(showPlaybackStatusItem);
            
            ToolStripMenuItem hideAutomaticallyItem = new ToolStripMenuItem();
            hideAutomaticallyItem.Text = "Hide Automatically";
            hideAutomaticallyItem.Checked = settingsService.HideAutomatically.Value;
            hideAutomaticallyItem.Click += (sender, args) => settingsService.HideAutomatically.Value = !settingsService.HideAutomatically.Value;
            behaviorItem.DropDownItems.Add(hideAutomaticallyItem);
            
            return behaviorItem;
        }
        
        private void ShowAboutScreen()
        {
            MessageBox.Show($"{ProgramInfo.Name} {ProgramInfo.Version}{ProgramInfo.VersionSuffix}\nCreated by {ProgramInfo.Author}\n\n{ProgramInfo.Credits}\n\n{ProgramInfo.Disclaimer}", ProgramInfo.Name, MessageBoxButtons.OK, MessageBoxIcon.None);
        }
    }
}