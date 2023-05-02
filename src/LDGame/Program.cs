using Murder;
using Murder.Diagnostics;

namespace LDGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                using Game game = new(new LDGame());
                game.Run();
            }
            catch (Exception ex) when (GameLogger.CaptureCrash(ex)) { }
        }
    }
}
