using AppFundacion.Views;

namespace AppFundacion
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(DonanteModificarView), typeof(DonanteModificarView));
            Routing.RegisterRoute(nameof(DonanteAgregarView), typeof(DonanteAgregarView));
            Routing.RegisterRoute(nameof(CobradorModificarView), typeof(CobradorModificarView));

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
