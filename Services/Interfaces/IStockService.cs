public interface IStockService
{
    Task<List<StockMovementList>> GetAllStockMovements();
    Task<List<StockMovementList>> FilterStockMovements(StockMovementFilter filter);
    Task<StockMovementDetail> GetDetail(int id);
    Task<StockMovementForm> GetForEdit(int id);
    Task<(string, string)> CreateStockMovement(StockMovementForm model);
    Task<(StockMovementForm, string, string)> UpdateStockMovement(StockMovementForm model);
    Task<(string, string)> DeleteManyStockMovements(List<int> ids);
    Task<string> ExportStockMovementsCsvAsync(StockMovementFilter filter);
}