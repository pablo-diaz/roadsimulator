namespace Infrastructure.Contracts
{
    public interface IConfig
    {
        int GetMinVehiclesCuota();
        int GetMaxVehiclesCuota();
        int GetMaxTracksToCreate();
        int GetMaxVehiclesToCreate();
        int GetMinAllowedVelocity();
        int GetMaxAllowedVelocity();
    }
}