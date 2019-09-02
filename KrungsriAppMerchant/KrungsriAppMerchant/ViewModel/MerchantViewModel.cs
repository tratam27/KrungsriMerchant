using KrungsriAppMerchant.Models;
using KrungsriAppMerchant.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing;
using ZXing.Net.Mobile.Forms;
using ZXing.QrCode;

namespace KrungsriAppMerchant.ViewModel
{
    public class MerchantViewModel
    {
        public string QRCodeValue { get; set; }

        public MerchantViewModel()
        {
            SignInButton = new Command(async => LogIn());
            QrCodeModel qrCode = new QrCodeModel
            {
                Name = Preferences.Get("Name", ""),
                BookBank = Preferences.Get("BookBank", "")
            };
            QRCodeValue = JsonConvert.SerializeObject(qrCode);
            PushToCreateQRCode = new Command(PushToQRPage);
        }
        public string UserName { get; set; }
        public string Password { get; set; }
        public ICommand SignInButton { get; set; }
        public ICommand PushToCreateQRCode { get; set; }
                
        async Task LogIn()
        {            
                try
                {
                    var objToApi = new
                    {
                        UserName = UserName,
                        Password = Password
                    };
                    
                    StringContent request = new StringContent($"{JsonConvert.SerializeObject(objToApi)}", Encoding.UTF8, "application/json");
                    var client = new HttpClient();
                    HttpResponseMessage response = await client.PostAsync("http://192.168.1.34:5000/api/auth/merchantlogin", request);
                    //response.EnsureSuccessStatusCode();                    
                    string responseBody = await response.Content.ReadAsStringAsync();
                    LoginModel loginModel = JsonConvert.DeserializeObject<LoginModel>(responseBody);
                
                    if (loginModel.token != null)
                    {
                        Preferences.Set("BookBank", loginModel.BookBank);
                        Preferences.Set("Name", loginModel.Name);
                        await Application.Current.MainPage.Navigation.PushAsync(new MotherTabbedPage());
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {

                }            
        }
        private void PushToQRPage()
        {
            Application.Current.MainPage.Navigation.PushAsync(new GenQrCode());
        }
    }
}
