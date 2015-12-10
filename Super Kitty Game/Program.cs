/*namespace Super_Kitty_Game
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
#endif
}*/
using System;
using System.Windows.Forms;

namespace Super_Kitty_Game
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Start start = new Start();
            while (start.ShowDialog() == DialogResult.OK)
            {
                using (var game = new Game1(start.Client))
                {
                    game.Run();
                }
            }
        }
    }
#endif
}


