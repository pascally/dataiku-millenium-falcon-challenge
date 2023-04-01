using Backend.Application.Config;
using Backend.Application.Interfaces.Common;
using Newtonsoft.Json;

namespace Backend.Infrastructure.Common;

public class JsonFileReader : IConfigFileReader
{
    public EmpireConfig ReadEmpireData(string filePath)
    {
        return ReadConfigFile<EmpireConfig>(filePath);
    }

    public MilleniumFalconConfig ReadMilleniumFalconData(string filePath)
    {
        return ReadConfigFile<MilleniumFalconConfig>(filePath);
    }

    private T ReadConfigFile<T>(string filePath) where T : class
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return null;
        }
        T res = null;
        var serializer = new JsonSerializer();
        using (var streamReader = new StreamReader(filePath))
        using (var textReader = new JsonTextReader(streamReader))
        {
            res = serializer.Deserialize<T>(textReader);
        }
        return res;
    }
}
