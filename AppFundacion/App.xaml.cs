namespace AppFundacion
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            UserAppTheme = AppTheme.Dark;
        }

        public void SetTheme(AppTheme theme)
        {
            UserAppTheme = theme;
        }
    }
}
