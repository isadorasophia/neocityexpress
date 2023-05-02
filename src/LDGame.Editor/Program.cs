using Murder.Editor;

namespace LDGame.Editor
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var editor = new Architect(new LDGameArchitect()))
            {
                editor.Run();
            }
        }
    }
}
