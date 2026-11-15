namespace RareCrewCSharp;

public class AppSettings
{
    public ApiSettings ApiSettings { get; set; }
}

public class ApiSettings
{
    public string Url { get; set; }
    public string Endpoint { get; set; }
    public string Key { get; set; }
}
