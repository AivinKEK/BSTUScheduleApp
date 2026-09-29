using Microsoft.Extensions.DependencyInjection;

namespace UniversityScheduleApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = new Window(new MainPage());
            window.Width = 400;
            window.Height = 800;
            window.MinimumWidth = 400;
            window.MaximumWidth = 400;
            window.MinimumHeight = 800;
            window.MaximumHeight = 800;
            return window;
        }
    }
}