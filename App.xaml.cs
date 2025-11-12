namespace Mökinvaraus
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Aloitetaan AppShellista
            MainPage = new AppShell();
        }
    }
}
