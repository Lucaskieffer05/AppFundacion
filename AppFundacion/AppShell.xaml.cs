namespace AppFundacion
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private void OnThemeSwitchToggled(object sender, ToggledEventArgs e)
        {
            AplicarTema(e.Value);
        }

        private static void AplicarTema(bool activarModoOscuro)
        {
            if (Application.Current != null)
            {
                if (activarModoOscuro)
                {
                    Application.Current.UserAppTheme = AppTheme.Dark;
                }
                else
                {
                    Application.Current.UserAppTheme = AppTheme.Light;
                }
            }
        }
    }
}
