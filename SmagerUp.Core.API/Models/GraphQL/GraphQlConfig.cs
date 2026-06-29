using System.Text.Json.Serialization;

namespace SmagerUp.Core.API.Models.GraphQL; 
public class GraphQlConfig {
    [JsonPropertyName("endpoint")]
    public string Endpoint { get; set; }
    [JsonPropertyName("query")]
    public string Query { get; set; }
    [JsonPropertyName("headers")]
    public Dictionary<string, string>? Headers { get; set; }
}
