using Backend.Application.Config;

namespace Backend.Application.Interfaces.Common;

public interface IConfigFileReader
{
    public EmpireConfig ReadEmpireData(string filePath);
    public MilleniumFalconConfig ReadMilleniumFalconData(string filePath);
}
