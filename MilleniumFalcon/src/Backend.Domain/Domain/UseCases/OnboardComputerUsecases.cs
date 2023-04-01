
namespace Backend.Domain.UseCases
{
    public interface OnboardComputerUsecases
    {
        /// <summary>
        /// Usecase existing for both 3CPO and R2D2
        /// </summary>
        /// <param name="pathToMilleniumFalconDatas"></param>
        /// <returns>if loaded successfully</returns>
        bool LoadMilleniumFalconDatas(string pathToMilleniumFalconDatas);

        /// <summary>
        /// Usecase existing only for 3CPO
        /// </summary>
        /// <param name="pathToEmpireDatas"></param>
        /// <returns>if loaded successfully</returns>
        bool LoadEmpireDatas(string pathToEmpireDatas);

        /// <summary>
        /// Usecase existing for both 3CPO and R2D2
        /// </summary>
        /// <returns>odds of success to reach destination</returns>
        double ComputeOddsToDestination();
    }
}
