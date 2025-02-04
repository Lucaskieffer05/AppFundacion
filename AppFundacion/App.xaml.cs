namespace AppFundacion
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        public void SetTheme(AppTheme theme)
        {
            UserAppTheme = theme;
        }
    }
}
