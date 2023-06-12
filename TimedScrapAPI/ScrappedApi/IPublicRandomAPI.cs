using Refit;
using System.Threading.Tasks;

namespace TimedScrapAPI
{
    public interface IPublicRandomAPI
    {
        [Get("/random?auth=null")]
        Task<ApiResponse<ScrapData>> GetApi();
    }
}
