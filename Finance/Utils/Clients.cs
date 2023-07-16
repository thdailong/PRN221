using Dapper;
using Imagekit;
using Microsoft.Data.Sqlite;
using Serilog;

namespace Finance.Utils
{
    public static class Clients
    {
        public static SqliteConnection Sql { get; set; } = null!;

        public static ServerImagekit Imagekit { get; set; } = null!;

        public static HttpClient Http { get; set; } = null!;

        private static bool localDev = true;

        public static async Task Init(IConfiguration cfg)
        {
            Log.Debug("Sqlite: Initializing... ");
            var connStr = localDev ? "Data Source=db/_database.local.db" : cfg["Sql"];
            Sql = new SqliteConnection(connStr);
            Sql.Open();
            Log.Debug("HttpClient: Initializing... ");
            Http = new HttpClient();
            Log.Debug("HttpClient: Testing with simple query...");
            var res = await Http.GetStringAsync("https://jsonplaceholder.typicode.com/todos/1");
            Log.Debug("HttpClient: RandomDogFact={DogFact}", res);
            Log.Debug("HttpClient: Initialized\n");

            Log.Debug("Imagekit: Initializing... ");
            Imagekit = new ServerImagekit(cfg["Imagekit:PublicKey"], cfg["Imagekit:PrivateKey"], cfg["Imagekit:Url"]);
            Log.Debug("Imagekit: Initialized\n");
        }

        public static void CloseAll()
        {
            if (Sql != null) Sql.Close();
            if (Http != null) Http.Dispose();
        }
    }
}
