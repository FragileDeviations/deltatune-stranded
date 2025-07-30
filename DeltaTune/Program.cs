using System.Windows.Forms;

namespace DeltaTune
{
    public static class Program
    {
        private static SingleInstance singleInstance = new SingleInstance(ProgramInfo.Name);
        
        public static void Main(string[] args)
        {
            using (singleInstance)
            {
                if (singleInstance.IsRunning)
                {
                    MessageBox.Show("Another instance of BRIDGES Audio Notifier is already running.\nTo close it, right-click the BRIDGES icon in your system tray and choose \"Quit\".", ProgramInfo.Name, MessageBoxButtons.OK);
                    return;
                }
                
                using (var game = new DeltaTune())
                {
                    game.Run();
                }
            }
        }
    }
}