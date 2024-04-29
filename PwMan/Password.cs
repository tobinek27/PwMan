namespace PwMan;
using Newtonsoft.Json;

public class Password
{
    public string Tag { get; set; }
    public string PasswordValue { get; set; }
    
    /*public void WriteToJson(string filePath)
    {
        //string json = JsonConvert.SerializeObject(this);
        JsonSerializer serializer = new JsonSerializer();
        serializer.NullValueHandling = NullValueHandling.Ignore;
        using (StreamWriter sw = new StreamWriter(filePath))
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            serializer.Serialize(writer, this);
        }
    }*/
    
    public void WriteToJson(string filePath) // this doesn't work yet
    {
        List<Password> existingData = ReadJson(filePath);
        existingData.Add(this);

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };
        string jsonData = JsonConvert.SerializeObject(existingData, settings);
        File.WriteAllText(filePath, jsonData);
    }
    
    public List<Password> ReadJson(string filePath)
    {
        using (StreamReader sr = new StreamReader(filePath))
        {
            string json = sr.ReadToEnd();
            return JsonConvert.DeserializeObject<List<Password>>(json);
        }
    }

    public Password(string tag, string passwordValue)
    {
        Tag = tag;
        PasswordValue = passwordValue;
    }
}