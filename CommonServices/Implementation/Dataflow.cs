using CommonServices.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared.Entities;
using Shared.PlannerDTO;
using Shared.WorkoutDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Implementation
{
    public class Dataflow : IDataflow
    {
        private readonly IConfiguration _configuration;
        public Dataflow(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<ResponseDto> LoginAuth(string Email, string Password)
        {
            ResponseDto logD = new ResponseDto();
            List<ResponseDto> FlexRES = new List<ResponseDto>();
            try
            {
                using var client = new HttpClient();
                var url = _configuration.GetRequiredSection("DataSources:BaseUrl").Value;

                var url1 = url + "WebLogin";
                var requestBody = new
                {
                    Email = Email,
                    Password,
                };
                var response = await client.PostAsJsonAsync(url1, requestBody);
                var responseBody = await response.Content.ReadAsStringAsync();
                logD = JsonConvert.DeserializeObject<ResponseDto>(responseBody);
            }
            catch (Exception ex)
            {
                logD.RespCode = "WT_LG_999";
                logD.RespType = "Exception";
                logD.RespMessage = ex.Message;
            }
            return logD;

        
        }

        public async Task<ResponseDto> Register(string fullname, string Email, string Password)
        {
            ResponseDto logD = new ResponseDto();
            List<ResponseDto> FlexRES = new List<ResponseDto>();
            try
            {
                using var client = new HttpClient();
                var url = _configuration.GetRequiredSection("DataSources:BaseUrl").Value;

                var url1 = url + "WebRegister";
                var requestBody = new
                {
                    UserName = fullname,
                    Email = Email,
                    Password,
                };
                var response = await client.PostAsJsonAsync(url1, requestBody);
                var responseBody = await response.Content.ReadAsStringAsync();
                logD = JsonConvert.DeserializeObject<ResponseDto>(responseBody);
            }
            catch (Exception ex)
            {
                logD.RespCode = "WT_RG_999";
                logD.RespType = "Exception";
                logD.RespMessage = ex.Message;
            }
            return logD;
        }
        public async Task<ResponseDto> AddTemplate(TemplateUpsertDto template, string JWT)
        {
            ResponseDto ResponseModel = new ResponseDto();
            List<ResponseDto> FlexRES = new List<ResponseDto>();
            try
            {
                using var client = new HttpClient();
                var url = _configuration.GetRequiredSection("DataSources:BaseUrl").Value;

                var url1 = url + "CreateTemplate";
                var json = JsonConvert.SerializeObject(template);
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWT);
                var response = await client.PostAsync(url1, requestContent);
                var responseBody = await response.Content.ReadAsStringAsync();
                ResponseModel = JsonConvert.DeserializeObject<ResponseDto>(responseBody);
            }
            catch (Exception ex)
            {
                ResponseModel.RespCode = "WT_PL_999";
                ResponseModel.RespType = "Exception";
                ResponseModel.RespMessage = ex.Message;
            }
            return ResponseModel;
        }
      

        public async Task<ResponseDto> GetTemplates(string JWT)
        {
            ResponseDto ResponseModel = new ResponseDto();
            List<ResponseDto> FlexRES = new List<ResponseDto>();
            try
            {
                using var client = new HttpClient();
                var url = _configuration.GetRequiredSection("DataSources:BaseUrl").Value;

                var url1 = url + "GetTemplates";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWT);
                var response = await client.GetAsync(url1);
                var responseBody = await response.Content.ReadAsStringAsync();
                ResponseModel = JsonConvert.DeserializeObject<ResponseDto>(responseBody);
            }
            catch (Exception ex)
            {
                ResponseModel.RespCode = "WT_PL_999";
                ResponseModel.RespType = "Exception";
                ResponseModel.RespMessage = ex.Message;
            }
            return ResponseModel;
        }
        public async Task<ResponseDto> GetTemplateDetail(long Id, string JWT)
        {
            ResponseDto ResponseModel = new ResponseDto();
            List<ResponseDto> FlexRES = new List<ResponseDto>();
            try
            {
                using var client = new HttpClient();
                var url = _configuration.GetRequiredSection("DataSources:BaseUrl").Value;
                var url1 = url + $"GetTemplateDetail?templateId={Id}";
                var request = new HttpRequestMessage(HttpMethod.Post, url1);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JWT);
                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                ResponseModel = JsonConvert.DeserializeObject<ResponseDto>(responseBody);
            }
            catch (Exception ex)
            {
                ResponseModel.RespCode = "WT_AW_999";
                ResponseModel.RespType = "Exception";
                ResponseModel.RespMessage = ex.Message;
            }
            return ResponseModel;
        }
        
        public async Task<ResponseDto> DeleteTemplate(long templateId, string JWT)
        {
            ResponseDto ResponseModel = new ResponseDto();
            List<ResponseDto> FlexRES = new List<ResponseDto>();
            try
            {
                using var client = new HttpClient();
                var url = _configuration.GetRequiredSection("DataSources:BaseUrl").Value;
                var url1 = url + $"DeleteTemplate?templateId={templateId}";
                var request = new HttpRequestMessage(HttpMethod.Post, url1);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JWT);
                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                ResponseModel = JsonConvert.DeserializeObject<ResponseDto>(responseBody);
            }
            catch (Exception ex)
            {
                ResponseModel.RespCode = "WT_PL_999";
                ResponseModel.RespType = "Exception";
                ResponseModel.RespMessage = ex.Message;
            }
            return ResponseModel;
        }

        public async Task<ResponseDto> GetExercises(string JWT)
        {
            ResponseDto ResponseModel = new ResponseDto();
            List<ResponseDto> FlexRES = new List<ResponseDto>();
            try
            {
                using var client = new HttpClient();
                var url = _configuration.GetRequiredSection("DataSources:BaseUrl").Value;

                var url1 = url + "GetExercises";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWT);
                var response = await client.GetAsync(url1);
                var responseBody = await response.Content.ReadAsStringAsync();
                ResponseModel = JsonConvert.DeserializeObject<ResponseDto>(responseBody);
            }
            catch (Exception ex)
            {
                ResponseModel.RespCode = "WT_EXS_999";
                ResponseModel.RespType = "Exception";
                ResponseModel.RespMessage = ex.Message;
            }
            return ResponseModel;
        }



        public async Task<ResponseDto> AddWorkoutSession(WorkoutSessionCreateDto workout, string JWT)
        {
            ResponseDto ResponseModel = new ResponseDto();
            List<ResponseDto> FlexRES = new List<ResponseDto>();
            try
            {
                using var client = new HttpClient();
                var url = _configuration.GetRequiredSection("DataSources:BaseUrl").Value;

                var url1 = url + "AddWorkoutSession";
                var json = JsonConvert.SerializeObject(workout);
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWT);
                var response = await client.PostAsync(url1, requestContent);
                var responseBody = await response.Content.ReadAsStringAsync();
                ResponseModel = JsonConvert.DeserializeObject<ResponseDto>(responseBody);
            }
            catch (Exception ex)
            {
                ResponseModel.RespCode = "WT_WS_999";
                ResponseModel.RespType = "Exception";
                ResponseModel.RespMessage = ex.Message;
            }
            return ResponseModel;
        }
    }
}
