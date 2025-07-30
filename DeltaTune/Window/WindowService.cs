using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DeltaTune.Settings;
using Microsoft.Xna.Framework;
using R3;
using SharpDX;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DeltaTune.Window
{
    public class WindowService : IWindowService, IDisposable
    {
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern int SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref int[] pMarInset);
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private const int GWL_EXSTYLE = -20;
        private const int LWA_ALPHA = 0x00000002;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_TOPMOST = 0x00000008;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_APPWINDOW = 0x00040000;
        private static readonly IntPtr HWND_TOPMOST = (IntPtr)(-1);
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        
        private readonly Game game;
        private readonly GameWindow window;
        private readonly GraphicsDeviceManager graphicsDeviceManager;
        private readonly ISettingsMenu settingsMenu;
        private readonly ISettingsService settingsService;
        private readonly float lineHeight;

        private NotifyIcon trayIcon;
        private IDisposable scaleFactorSubscription;
        private IDisposable windowSizeSubscription;
        private IDisposable windowTopmostIntervalSubscription;

        public WindowService(Game game, GraphicsDeviceManager graphicsDeviceManager, ISettingsMenu settingsMenu, ISettingsService settingsService, float lineHeight)
        {
            this.game = game;
            this.window = game.Window;
            this.graphicsDeviceManager = graphicsDeviceManager;
            this.settingsMenu = settingsMenu;
            this.settingsService = settingsService;
            this.lineHeight = lineHeight;
        }

        public void InitializeWindow()
        {
            IntPtr windowHandle = window.Handle;
            
            game.IsMouseVisible = true;
            game.IsFixedTimeStep = true;
            game.TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / ScreenRefreshRateProvider.GetScreenRefreshRate(windowHandle, 60));
            game.InactiveSleepTime = TimeSpan.Zero;
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;

            window.IsBorderless = true;
            window.AllowUserResizing = false;
            
            SetWindowLong(windowHandle, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_TOPMOST | WS_EX_TOOLWINDOW);
            SetLayeredWindowAttributes(windowHandle, 0, 255, LWA_ALPHA);
            
            int[] margins = { -1 };
            DwmExtendFrameIntoClientArea(window.Handle, ref margins);

            MakeTopmostWindow();
            
            SetWindowScale(settingsService.ScaleFactor.Value);
            UpdateWindowPosition();
            
            CreateTrayIcon(windowHandle);
            
            scaleFactorSubscription = settingsService.ScaleFactor.Subscribe(scale =>
            {
                SetWindowScale(scale);
                UpdateWindowPosition();
            });
            
            windowSizeSubscription = settingsService.Position.Subscribe(_ => UpdateWindowPosition());

            windowTopmostIntervalSubscription = Observable.Interval(TimeSpan.FromSeconds(0.05)).Subscribe(_ =>
            {
                if (!trayIcon.ContextMenuStrip.Visible)
                {
                    MakeTopmostWindow();
                }
            });

            if (settingsService.IsFactorySettings)
            {
                settingsService.ScaleFactor.Value = GetRecommendedScale();
            }
        }

        private void MakeTopmostWindow()
        {
            SetWindowPos(window.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        private void UpdateWindowPosition()
        {
            Vector2 fractionalPosition = settingsService.Position.Value;
            Rectangle currentScreenBounds = GetCurrentScreenBounds();
            Point windowPosition = new Point(
                0,
                (int)(currentScreenBounds.Height * fractionalPosition.Y)
            );

            if (fractionalPosition.Y > 0.5f)
            {
                windowPosition.Y -= (int)lineHeight * settingsService.ScaleFactor.Value;
            }
            
            window.Position = windowPosition;
        }

        // In WindowService.cs
        private void SetWindowScale(int scaleFactor)
        {
            Rectangle currentScreenBounds = GetCurrentScreenBounds();
            // Get the number of lines to display (e.g., 3 for title, artist, courtesy)
            int lineCount = 3; // Or get dynamically from displayService/musicTitleDisplay
            int totalHeight = (int)(lineHeight * scaleFactor * lineCount);
            SetWindowSize(new Point(currentScreenBounds.Width, totalHeight));
        }
        
        private Rectangle GetCurrentScreenBounds()
        {
            System.Drawing.Rectangle boundsSystemType = Screen.FromHandle(window.Handle).WorkingArea;
            return new Rectangle(boundsSystemType.X, boundsSystemType.Y, boundsSystemType.Width, boundsSystemType.Height);
        }
        
        private int GetRecommendedScale()
        {
            System.Drawing.Rectangle bounds = Screen.FromHandle(window.Handle).WorkingArea;
            return MathUtil.Clamp((int)Math.Ceiling(bounds.Width / 960f) + 1, 1, 8);
        }

        private void SetWindowSize(Point size)
        {
            graphicsDeviceManager.PreferredBackBufferWidth = size.X;
            graphicsDeviceManager.PreferredBackBufferHeight = size.Y;
            graphicsDeviceManager.ApplyChanges();
        }

        private void CreateTrayIcon(IntPtr windowHandle)
        {
            Form form = (Form)Control.FromHandle(windowHandle);
            Icon icon = form.Icon;
            trayIcon = new NotifyIcon();
            trayIcon.Icon = icon;
            trayIcon.Text = "DeltaTune";
            trayIcon.Visible = true;
            trayIcon.ContextMenuStrip = settingsMenu.GetSettingsMenu();

            trayIcon.BalloonTipTitle = "DeltaTune is now running!";
            trayIcon.BalloonTipText = "Play some music to get started or right-click the DeltaTune icon in your system tray for customization options.";
            trayIcon.ShowBalloonTip(1000);
        }

        public void Dispose()
        {
            scaleFactorSubscription?.Dispose();
            windowSizeSubscription?.Dispose();
            windowTopmostIntervalSubscription?.Dispose();
        }
    }
}