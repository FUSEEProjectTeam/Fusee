using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.Base.Imp.WebAsm
{
    public static class FileAssetProvider
    {
        public static async Task<string> LoadMe(string uri)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(WasmResourceLoader.GetLocalAddress()) };
            try
            {
                var response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"[Error] {nameof(WasmResourceLoader)}.{nameof(LoadMe)}(): {exception}");
                return null;
            }

        }
    }
}
