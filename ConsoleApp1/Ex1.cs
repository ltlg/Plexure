using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ex1
{
    public class Ex1
    {
        private HttpClient _client;
        private CancellationToken _token;

        public Ex1(HttpClient client, CancellationToken token)
        {
            _client = client;
            _token = token;
        }

        public void SetCancellationToken(CancellationToken cancellation)
        {
            _token = cancellation;
        }

        /// <summary>
        /// Get the total content length of url reponses
        /// </summary>
        /// <param name="urlList">The list of urls need to trigger</param>
        /// <param name="cancellationToken">The task cancellation token passed by the caller</param>
        /// <returns>The total content length of url passed in</returns>
        public async Task<long> GetWebResourceLength(IEnumerable<string> urlList)
        {
            var downloadTasks = new List<Task<long>>();

            foreach (var url in urlList)
            {
                downloadTasks.Add(GetWebResourceLength(url));
            }
            var task = Task.WhenAll(downloadTasks);
            task.Wait(_token);

            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                    return task.Result.Sum(x => x);
                case TaskStatus.Faulted:
                    return -1;
                default:
                    return 0;
            };

        }

        /// <summary>
        /// Get response length for the single url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task Object</returns>
        private async Task<long> GetWebResourceLength(string url)
        {
            using (HttpResponseMessage response = await _client.GetAsync(url, _token))
            {
                return response.Content.Headers.ContentLength ?? 0;
            }
        }
    }
}
