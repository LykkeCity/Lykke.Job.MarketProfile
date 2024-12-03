using System.Threading.Tasks;
using Antares.Service.MarketProfile.Client;

namespace Console.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("PRESS ENTER:");
            System.Console.ReadLine();

            var marketProfileClient = new MarketProfileServiceClient("nosql.share.svc.cluster.local:5125", 
                "http://market-profile.lykke-service.svc.cluster.local/");

            marketProfileClient.Start();

            var allPairs = marketProfileClient.EventualCache.GetAll();

            foreach (var pair in allPairs)
            {
                System.Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(pair));
            }
        }
    }
}
