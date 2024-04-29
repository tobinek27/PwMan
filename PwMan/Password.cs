namespace PwMan;
using Newtonsoft.Json;

public class Password
{
    public string Tag { get; set; }
    public string PasswordValue { get; set; }
    
    
    public void WriteToJson(string filePath)
    {
        List<Password> existingData = ReadJson(filePath);
        // this line overrides an already existing password with a matching tag
        existingData.RemoveAll(p => p.Tag == Tag);
        existingData.Add(this);
        
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };
        string jsonData = JsonConvert.SerializeObject(existingData, settings);
        File.WriteAllText(filePath, jsonData);
    }
    
    public static List<Password> ReadJson(string filePath)
    {
        if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
        {
            return new List<Password>();
        }

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