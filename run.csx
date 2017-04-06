#r "Newtonsoft.Json"
using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static void Run(TimerInfo myTimer, out string outputEventHubMessage, TraceWriter log)
{
    var exchangerates = GetRateAsync().Result;
    var jsonmessage = JsonConvert.SerializeObject(exchangerates);
    outputEventHubMessage = jsonmessage.ToString();
}
private static async Task<List<ExchangeRate>> GetRateAsync()
{
    string url = "https://www.gaitameonline.com/rateaj/getrate";
    var client = new HttpClient();
    var response = new HttpResponseMessage();
    response = await client.GetAsync(url);
    string result = await response.Content.ReadAsStringAsync();

    var requestJson = JObject.Parse(result);
    var quotes = requestJson["quotes"];
    var lists = new List<ExchangeRate>();
    foreach (var quote in quotes)
    {
        ExchangeRate rate = new ExchangeRate();
        rate.entrytype = "ExchangeRate";
        rate.date = DateTime.Now.ToLocalTime();
        rate.currencyPairCode = quote["currencyPairCode"].ToString();
        rate.high = float.Parse(quote["high"].ToString());
        rate.open = float.Parse(quote["open"].ToString());
        rate.bid = float.Parse(quote["bid"].ToString());
        rate.ask = float.Parse(quote["ask"].ToString());
        rate.low = float.Parse(quote["low"].ToString());
        lists.Add(rate);
    }
    return lists;
}

public class ExchangeRate
{
    public string entrytype { get; set; }
    public DateTime date { get; set; }
    public float high { get; set; }
    public float open { get; set; }
    public float bid { get; set; }
    public string currencyPairCode { get; set; }
    public float ask { get; set; }
    public float low { get; set; }
}
