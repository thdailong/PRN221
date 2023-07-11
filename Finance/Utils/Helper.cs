using Finance.Model;
using Imagekit;
using System.Text.Json;

namespace Finance.Utils
{
    public static class Helper
    {
        public static class Quote
        {
            class QuoteSource
            {
                public string Url { get; set; } = "";
                public string AuthorPath { get; set; } = "";
                public string MessagePath { get; set; } = "";
            }

            private static QuoteSource[] sources =
            {
                new() { Url="https://api.quotable.io/random", AuthorPath="author", MessagePath="content" },
            };

            private static Random rng = new();

            public static async Task<QuoteData> Get()
            {
                var source = sources[rng.Next(1)];
                var json = JsonDocument.Parse(await Clients.Http.GetStringAsync(source.Url)).RootElement;
                return new QuoteData
                {
                    Author = json.GetProperty(source.AuthorPath).GetString()!,
                    Message = json.GetProperty(source.MessagePath).GetString()!
                };
            }
        };

        public static JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public static string SqlUpdate(string table, params string[] props)
        {
            string pairs = $"{string.Join(", ", props.Select(col => $"{col}=@{col}"))}";
            return $"update {table} set {pairs} ";
        }
        public static string SqlInsert(string table, params string[] props)
        {
            string key = string.Join(", ", props);
            string value = $"@{string.Join(", @", props)}";
            return $"insert into {table}({key}) values({value})";
        }

        public static string ActiveClass(HttpContext ctx, string route, bool matchEnd = true)
        {
            var path = (ctx.Request.RouteValues["page"] as string) ?? "/";
            path = path.ToLower();
            return (matchEnd ? path.EndsWith(route) : path.StartsWith(route)) ? "active" : "";
        }

        public static async Task<ImagekitResponse> UploadFile(string path, IFormFile file)
        {
            using var orgStream = file.OpenReadStream();

            var fileBytes = await ToBytes(orgStream);
            var result = await Clients.Imagekit.Folder(path).FileName(file.Name).UploadAsync(fileBytes);
            return result;
        }

        public static string GetImage(string url, Transformation? transform)
        {
            if (transform is null || !url.StartsWith("https://ik.imagekit.io")) return url;
            return Clients.Imagekit.Url(transform).Src(url).Generate();
        }

        public static async Task<byte[]> ToBytes(Stream instream)
        {
            if (instream is MemoryStream)
                return ((MemoryStream)instream).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                await instream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static string FormatNumber(double n)
        {
            if (n < 1000)
                return n.ToString();

            if (n < 10000)
                return String.Format("{0:#,.##}K", n - 5);

            if (n < 100000)
                return String.Format("{0:#,.#}K", n - 50);

            if (n < 1000000)
                return String.Format("{0:#,.}K", n - 500);

            if (n < 10000000)
                return String.Format("{0:#,,.##}M", n - 5000);

            if (n < 100000000)
                return String.Format("{0:#,,.#}M", n - 50000);

            if (n < 1000000000)
                return String.Format("{0:#,,.}M", n - 500000);

            return String.Format("{0:#,,,.##}B", n - 5000000);
        }
    }
}
