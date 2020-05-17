using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinTimeSheet.Models;

namespace XamarinTimeSheet
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class EmployeePage : ContentPage
    {
        public EmployeePage()
        {
            InitializeComponent();
            employeeList.ItemsSource = new object[] { "" };
        }
        // Tapahtuman käsittelijä - lataa työntekijät

        private async void LoadEmployees(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://timesheetrestapiriku.azurewebsites.net"); // yhteys RestApi appiin
            string json = await client.GetStringAsync("/api/Employees"); // polku employees sivulle
            object[] employees = JsonConvert.DeserializeObject<object[]>(json); // employees lista

            employeeList.ItemsSource = employees;
        }
        // tapahtuman käsittelijä - valittu työntekijä otetaan talteen ja navigoidaan tehtävä sivulle

        private async void LoadAssignmentPage(object sender,EventArgs e)
        {
            string employee = employeeList.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(employee))
            {
                await DisplayAlert("Valinta puuttuu", "Valitse työntekijä", "Ok"); // otsikko, teksti, kuittausteksti
            } else
            {
                SelectedEmployee.Name = employee;
                await Navigation.PushAsync(new WorkAssignmentPage()); // Navigoidaan uudelle sivulle
            }
        }
    }
}
