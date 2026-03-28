using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Actividad_Significativa
{
    public partial class App : Application
    {
        // Al iniciar la app registramos un manejador global: asi un error no controlado se muestra y no tumba el proceso de golpe
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        // Mostramos mensaje y marcamos Handled para que WPF no cierre la ventana sin avisar al usuario
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Se detecto un error inesperado\n\n{e.Exception.Message}\n\nDetalle tecnico\n{e.Exception.StackTrace}",
                            "Error critico controlado", MessageBoxButton.OK, MessageBoxImage.Error);

            e.Handled = true;
        }
    }
}
