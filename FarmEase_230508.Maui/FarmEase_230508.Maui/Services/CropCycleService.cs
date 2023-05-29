using DevExpress.Maui.Core.Internal;
using FarmEase_230508.Maui.Common;
using FarmEase_230508.Maui.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.XtraPrinting.Native.PageSizeInfo;

namespace FarmEase_230508.Maui.Services {
    public class CropCycleService : ICropCycleService {
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly string _apiUrl = ON.Platform(android: "http://192.168.1.213:5000/CustomCropTasks/CropCycle/", iOS: "http://192.168.1.213:5000/CustomCropTasks/CropCycle/");
        public async Task<string> CreateCropCycle(CropCreateCommand command) {
            var json = JsonConvert.SerializeObject(command);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var authToken = await SecureStorage.GetAsync("jwt_token");

            if (authToken != null && !JwtTokenValidator.IsTokenValid(authToken)) {
                throw new Exception("Session Expired");
            }

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            var response = await HttpClient.PostAsync($"{_apiUrl}Create", content);

            var reposeContent = await response.Content.ReadAsStringAsync();

            return reposeContent;
        }

        public async Task<string> GetCropCycleById(int Id) {
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://192.168.1.213:5000/api/odata/CropCycle({Id})");

            var authToken = await SecureStorage.GetAsync("jwt_token");

            if (authToken != null && !JwtTokenValidator.IsTokenValid(authToken)) {
                throw new Exception("Session Expired");
            }

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            var response = await HttpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        public async Task<string> GetCropCycleTasksById(int Id) {
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://192.168.1.213:5000/CustomCropTasks/CropCycleTasks/{Id}");

            var authToken = await SecureStorage.GetAsync("jwt_token");

            if (authToken != null && !JwtTokenValidator.IsTokenValid(authToken)) {
                throw new Exception("Session Expired");
            }

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            var response = await HttpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        public async Task<string> GetCrops() {
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://192.168.1.213:5000/api/odata/Crop");

            var authToken = await SecureStorage.GetAsync("jwt_token");

            if (authToken != null && !JwtTokenValidator.IsTokenValid(authToken)) {
                throw new Exception("Session Expired");
            }

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            var response = await HttpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
    }
}
