using System.Globalization;

namespace AppFundacion
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            // Forzar el idioma a español
            var culture = new CultureInfo("es-ES"); // Cambia a "es-MX" si prefieres español latinoamericano
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

            const int newWidth = 1360;
            const int newHeight = 768;

            window.Width = newWidth;
            window.Height = newHeight;

            return window;
        }

        public void SetTheme(AppTheme theme)
        {
            UserAppTheme = theme;
        }
    }
}
