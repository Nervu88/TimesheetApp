using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinTimeSheet
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Navigointi osuus pitää määrittää itse
            MainPage = new NavigationPage(new EmployeePage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
