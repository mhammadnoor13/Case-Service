using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public class EmbeddingClient : IEmbeddingClient
    {
        private const string EmbedCase = "/embedding/case";

        private readonly HttpClient httpClient;

        public EmbeddingClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task <bool> EmbedCaseAsync(string caseText, Guid consultantId, CancellationToken ct)
        {
            var payload = new { text = caseText };
            using var req = new HttpRequestMessage(HttpMethod.Post, EmbedCase)
            {
                Content = JsonContent.Create(payload)
            };

            req.Headers.Add("X-User-Id", consultantId.ToString());
            
            using var resp = await httpClient.SendAsync(req, ct);

            if (!resp.IsSuccessStatusCode)
                return false;

            return true;

        }
    }
}
