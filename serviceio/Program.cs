using CsvHelper.Configuration;
using Newtonsoft.Json;
using RestEase;
using System.Globalization;
using System.Text;
using CsvHelper;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";



builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000");
        }
    );
});

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);
app.MapGet(
    "/calculmoyenne",
    async (HttpContext context) =>
    {
        string cheminStockage = "/etc/data_stored/note.csv";

        try
        {
            using (var reader = new StreamReader(cheminStockage))
            using (
                var csvReader = new CsvReader(
                    reader,
                    new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", }
                )
            )
            {
                var rawData = csvReader.GetRecords<Eleve>().ToList();

                Console.WriteLine(rawData);

                var eleves = rawData
                    .Skip(1)
                    .Where(r => !string.IsNullOrEmpty(r.EtudId) && int.TryParse(r.EtudId, out _));

                Console.WriteLine(eleves);
               
                var coeffs = rawData
                    .Skip(1)
                    .Where(r => r.Name == "Coef.")
                    .Select(
                        r =>
                            new Coeff
                            {
                                S1101 = r.S1101,
                                S1102 = r.S1102,
                                S1103 = r.S1103,
                                S1104 = r.S1104,
                                S1105A = r.S1105A,
                                S1105B = r.S1105B,
                                S1106 = r.S1106,
                                S1107 = r.S1107,
                                S1108 = r.S1108,
                                UE12 = r.UE12,
                                BonusUE12 = r.BonusUE12,
                                S1201 = r.S1201,
                                S1202A = r.S1202A,
                                S1202B = r.S1202B,
                                S1203 = r.S1203,
                                S1204 = r.S1204,
                                S1205 = r.S1205,
                            }
                    );

                var clearData = new { eleves, coeffs };

                //jusque ici ça a l'air de fonctionner

                string clearSerializeData = JsonConvert.SerializeObject(clearData);

                IApiClient apiClient = RestClient.For<IApiClient>("http://servicecalcul:80/");

                var moyennePondere = await apiClient.CalculMoyenne(clearData);

                Console.WriteLine(moyennePondere);

                var deserializeWeightedAverage = JsonConvert.DeserializeObject<
                    Dictionary<string, double>
                >(await moyennePondere.Content.ReadAsStringAsync());

                var finalData = deserializeWeightedAverage
                    .Select(pair => new Moyenne { Name = pair.Key, Moy = pair.Value })
                    .ToList();

                Console.WriteLine("finalData" + finalData);

                nouveauCSV(finalData);
                
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    moyennePondere.Content.ReadAsStringAsync().Result
                );
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error reading file: {ex.Message}");
        }
    }
);

static void nouveauCSV(List<Moyenne> moyennePondere)
{
    string cheminStockage = "/etc/data_stored/moyenne.csv";
    using (var writer = new StreamWriter(cheminStockage))
    using (
        var csvWriter = new CsvWriter(
            writer,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                Encoding = Encoding.UTF8,
								HasHeaderRecord = false,
            }
        )
    )
    {
        csvWriter.WriteRecords(moyennePondere);
    }
}

app.Run();
