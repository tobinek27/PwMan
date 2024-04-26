namespace PwMan;
using Newtonsoft.Json;

public class Password
{
    public string Tag { get; set; }
    public string PasswordValue { get; set; }
    
    
    public void WriteToJSON(string filePath, List<Password> trackDataList)
    {
        string json = JsonConvert.SerializeObject(trackDataList);

        using (StreamWriter sw = new StreamWriter(filePath))
        {
            sw.Write(json);
        }
    }

    public List<Password> ReadJSON(string filePath)
    {
        using (StreamReader sr = new StreamReader(filePath))
        {
            string json = sr.ReadToEnd();

            return JsonConvert.DeserializeObject<List<Password>>(json);
        }
    }
}