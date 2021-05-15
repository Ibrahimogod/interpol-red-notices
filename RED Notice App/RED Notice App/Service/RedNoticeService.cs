using RED_Notice_App.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace RED_Notice_App.Service
{
    public class RedNoticeService
    {
        Response response;
        int page;
        string ApiUrl;
        string filePath;


        public RedNoticeService()
        {
            GetPageAsync();
            ApiUrl = "https://ws-public.interpol.int/notices/v1/red?resultPerPage=20&page=";
            filePath = Path.Combine(FileSystem.AppDataDirectory, "Response.json");
        }

        async void GetPageAsync()
        {
            var stringpage = await SecureStorage.GetAsync(nameof(page));
            page = int.Parse(stringpage);
        }


        public async void GetResponseAsync()
        {
            var jsonresponse = "";
            try
            {
                jsonresponse = await GetResponseFromFileAsync();
            }
            catch (Exception)
            {
                jsonresponse = await GetResponseFromWebAsync();
                if (!File.Exists(filePath))
                {
                    using (var file = File.CreateText(filePath))
                    {
                        await file.WriteAsync(jsonresponse);
                        if(file != null)
                        {
                            file.Close();
                        }
                    }
                }
            }
            response = Response.FromJson(jsonresponse);
            
        }

        public async Task<string> GetResponseFromFileAsync()
        {
            using (Stream stream = new FileStream(filePath, FileMode.Open))
            {
                using(StreamReader reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public async Task<string> GetResponseFromWebAsync()
        {
            using (var client = new HttpClient())
            {
                var task = client.GetStringAsync(ApiUrl + ++page);
                task.GetAwaiter().OnCompleted(async () =>
                {
                    await SecureStorage.SetAsync(nameof(page), page.ToString());
                });
                return await task;
            }
        }
    }
}
