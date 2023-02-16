using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceTest.Client
{
    public class AunthentificateClient
    {
        public BaseClient client;

        public AunthentificateClient()
        {
            client = new BaseClient();
        }

        public async Task<ResponseModel<string>> RegisterAsync(string email)
        {
            var response = await client.SendGetAsync<string>("/api/v1/register?email=" + email);
            return response;
        }

        public async Task<int> RequestActivationCodeAsync(string guestUserId)
        {
            var response = await client.SendGetAsync<string>("/api/v1/request-activation-code?guestUserId=" + guestUserId);
            return response.StatusCode;
        }

        public async Task<int> ConfirmEmailAsync(string guestUserId, string password, string activationCode)
        {
            var response = await client.SendGetAsync<string>($"/api/v1/confirm-email?guestUserId={guestUserId}" +
                $"&password={password}" +
                $"&activationCode={activationCode}");
            return response.StatusCode;
        }

        public async Task<ResponseModel<string>> SignInAsync(string userId, string password)
        {
            var response = await client.SendGetAsync<string>($"/api/v1/sign-in?userId={userId}&password={password}");
            return response;
        }

        public async Task<int> UpdateUserAsync(string authToken, string email, string password)
        {
            var response = await client.SendPutAsync($"/api/v1/update-user?authToken={authToken}" + 
                $"&email={email}" +
                $"&password={password}");
            return response.StatusCode;
        }

        public async Task<ResponseModel<string>> UpdateTokenAsync(string authToken)
        {
            var response = await client.SendPutAsync<string,string>($"/api/v1/update-token?authToken={authToken}", "");
            return response;
        }

        public async Task<int> SignOutAsync()
        {
            var response = await client.SendGetAsync<string>($"/api/v1/sign-out");
            return response.StatusCode;
        }

        public async Task<string> ConfirmationCodeAsync(string guestUserId)
        {
            var response = await client.SendGetAsync<string>($"/api/v1/confirmation-code?guestUserId={guestUserId}");
            return response.Content;
        }

    }
}
