using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Data;
using Newtonsoft.Json;
using TodoBackend.Models;
using TodoFrontend_MVC.Models;


namespace TodoFrontend_MVC.Services
{
   

    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7110/"); // Replace with backend URL
        }


        public async Task<bool> SignupAsync(UserModel userModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/User/Signup", userModel);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log the error message for further insights
                Console.WriteLine($"Error in SignupAsync: {ex.Message}");
                return false;
            }
        }



        //public async Task<bool> LoginAsync(TodoFrontend_MVC.Models.LoginRequest loginRequest)
        //{
        //    try
        //    {
        //        var response = await _httpClient.PostAsJsonAsync("api/User/Login", loginRequest);
        //        return response.IsSuccessStatusCode;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error in LoginAsync: {ex.Message}");
        //        return false;
        //    }
        //}


        public async Task<int?> LoginAsync(TodoFrontend_MVC.Models.LoginRequest loginRequest)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/User/Login", loginRequest);
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the LoginResponse object
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    return loginResponse?.UserId; // Return the user ID from the response
                }
                return null; // Login failed, return null
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoginAsync: {ex.Message}");
                return null;
            }
        }



        //public async Task<int?> LoginAsync(TodoFrontend_MVC.Models.LoginRequest loginRequest)
        //{
        //    try
        //    {
        //        var response = await _httpClient.PostAsJsonAsync("api/User/Login", loginRequest);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            // Assume the API returns the user ID as an integer in the response body
        //            var userId = await response.Content.ReadFromJsonAsync<int>();
        //            return userId;
        //        }
        //        else
        //        {
        //            return null; // Login failed, return null
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error in LoginAsync: {ex.Message}");
        //        return null;
        //    }
        //}




        public async Task<bool> ForgotPasswordAsync(TodoFrontend_MVC.Models.ForgotPasswordRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/User/ForgotPassword", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ForgotPasswordAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<HttpResponseMessage> ResetPasswordAsync(TodoFrontend_MVC.Models.ResetPasswordRequest request)
        {
            // Ensure the token and email are passed in the JSON body to the backend
            var response = await _httpClient.PostAsJsonAsync("api/User/ResetPassword", request);
            return response;
        }




    }

}
