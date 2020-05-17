using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XamarinTimeSheet.Models;
using Xamarin.Essentials; // Tätä tarvitaan geolokaatiota varten

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinTimeSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkAssignmentPage : ContentPage
    {

        public WorkAssignmentPage()
        {
            InitializeComponent();
            assignmentList.ItemsSource = new string[] { "" };
        }


        // tapahtuma käsittelijä - lataa työtehtävät

        private async void LoadAssignments(object sender, EventArgs e)
        {
            // Alustus ennen palvelimen tietojen lataamista näkymään
            assignmentList.ItemsSource = new String[] { "Ladataan..." };
            

            // Sijaintiin liittyvä koodi
            try
            {

                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {                 
                    longtitudeLabel.Text = location.Longitude.ToString();
                    latitudeLabel.Text = location.Latitude.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                longtitudeLabel.Text = "Error" + fnsEx;
            }
            catch (FeatureNotEnabledException fneEx)
            {
                longtitudeLabel.Text = "Error" + fneEx;
            }
            catch (PermissionException pEx)
            {
                longtitudeLabel.Text = "Error" + pEx;
            }
            catch (Exception ex)
            {
                longtitudeLabel.Text = "Error" + ex;
            }
            if (sender == getAll)
            {
                try
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("https://timesheetrestapiriku.azurewebsites.net"); // yhteys RestApi appiin
                    string json = await client.GetStringAsync("/api/WorkAssignments/getAll"); // polku workAssignments all sivulle
                    object[] assignments = JsonConvert.DeserializeObject<object[]>(json); // work lista

                    assignmentList.ItemsSource = assignments;
                    StartBtn.IsVisible = true;
                    StopBtn.IsVisible = true;
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.GetType().Name + ": " + ex.Message;
                    assignmentList.ItemsSource = new string[] { errorMessage };
                }
            }
            if (sender == getCompleted)
            {
                try
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("https://timesheetrestapiriku.azurewebsites.net"); // yhteys RestApi appiin
                    string json = await client.GetStringAsync("/api/WorkAssignments/getCompleted"); // polku workAssignments completed sivulle
                    object[] assignments = JsonConvert.DeserializeObject<object[]>(json); // completedWork lista

                    assignmentList.ItemsSource = assignments;
                    StartBtn.IsVisible = false;
                    StopBtn.IsVisible = false;
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.GetType().Name + ": " + ex.Message;
                    assignmentList.ItemsSource = new string[] { errorMessage };
                }
            }
        }

        // tapahtuma käsittelijä - aloita työ

        private async void StartWork(object sender, EventArgs e)
        {
            string assignmentName = assignmentList.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(assignmentName))
            {
                await DisplayAlert("Työ puuttuu","Valitse ensin aloitettava työtehtävä","OK");
            }
             else
            {
                WriteComment(); // Kommentin metodi josta napataan kommentti talteen

                try
                {
                    // Käytetään Xamarin sovellukseen luotua model luokkaa ja perustetaan objekti palvelimelle lähetettäväksi

                    WorkAssignmentOperationModel data = new WorkAssignmentOperationModel()
                    {
                        Operation = "Start",
                        AssignmentTitle = assignmentName,
                        Name = SelectedEmployee.Name 
                    };

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("https://timesheetrestapiriku.azurewebsites.net"); // yhteys RestApi appiin

                    // muutetaan em data objekti Jsoniksi
                    string input = JsonConvert.SerializeObject(data);
                    StringContent content = new StringContent(input, Encoding.UTF8, "application/json");

                    //Lähetetään serialisoitu objekti bacn-endiin Post pyyntönä
                    HttpResponseMessage message = await client.PostAsync("/api/workassignments", content);

                    // Otetaan vastaan palvelimen vastaus
                    string reply = await message.Content.ReadAsStringAsync();

                    // Asetetaan vastaus serialisoituna success muuttujaan
                    bool success = JsonConvert.DeserializeObject<bool>(reply);

                    if (success)
                    {
                        await DisplayAlert("Työn aloitus", "Työ aloitettu", "Sulje");
                    } else
                    {
                        await DisplayAlert("Aloitus ei onnistu", "Työ on jo käynnissä!", "Sulje");
                    }
                }

                catch (Exception ex)
                {
                    string errorMessage = ex.GetType().Name + ": " + ex.Message; //Poikkeuksen customoitu selvittäminen ja näyttäminen list viewssä
                    assignmentList.ItemsSource = new string[] { errorMessage };
                }
            }
        }

        private async void StopWork(object sender, EventArgs e)
        {
            string assignmentName = assignmentList.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(assignmentName))
            {
                await DisplayAlert("Työ puuttuu", "Valitse ensin aloitettava työtehtävä", "OK");
            }
            else
            {
                try
                {
                    // Käytetään Xamarin sovellukseen luotua model luokkaa ja perustetaan objekti palvelimelle lähetettäväksi

                    WorkAssignmentOperationModel data = new WorkAssignmentOperationModel()
                    {
                        Operation = "Stop",
                        AssignmentTitle = assignmentName,
                        Name = SelectedEmployee.Name
                    };

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("https://timesheetrestapiriku.azurewebsites.net"); // yhteys RestApi appiin

                    // muutetaan em data objekti Jsoniksi
                    string input = JsonConvert.SerializeObject(data);
                    StringContent content = new StringContent(input, Encoding.UTF8, "application/json");

                    //Lähetetään serialisoitu objekti back-endiin Post pyyntönä
                    HttpResponseMessage message = await client.PostAsync("/api/workassignments", content);

                    // Otetaan vastaan palvelimen vastaus
                    string reply = await message.Content.ReadAsStringAsync();

                    // Asetetaan vastaus serialisoituna success muuttujaan
                    bool success = JsonConvert.DeserializeObject<bool>(reply);

                    if (success)
                    {
                        await DisplayAlert("Työn lopetus", "Työ lopetettu", "Sulje");

                        assignmentList.ItemsSource = "";
                    }
                    else
                    {
                        await DisplayAlert("Työn lopetus", "Työtä ei voitu lopettaa, koska sitä ei ole aloitettu", "Sulje");
                    }
                }

                catch (Exception ex)
                {
                    string errorMessage = ex.GetType().Name + ": " + ex.Message; //Poikkeuksen customoitu selvittäminen ja näyttäminen list viewssä
                    assignmentList.ItemsSource = new string[] { errorMessage };
                }
            }
        }

        private void WriteComment()
        {
            // Tähän koodia jossa tulee input field kommenttia varten.
        }
    }

}