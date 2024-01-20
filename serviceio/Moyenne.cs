using CsvHelper.Configuration.Attributes;

public class Moyenne
{
    [Name("Prenom")]
    public string? Name { get; set; }

    [Name("Moy")]
    public double Moy { get; set; }
}
