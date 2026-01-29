using Microsoft.EntityFrameworkCore;
using StockCore.Data;
using StockCore.Entities;
using StockCore.Services.Const;

public class StockService : IStockService
{
    private readonly ApplicationDbContext _db;

    public StockService(ApplicationDbContext context)
    {
        _db = context;
    }

    public async Task<List<StockMovementList>> FilterStockMovements(StockMovementFilter filter)
    {
        var query = _db.StockMovements
            .Include(s => s.Product)
            .AsQueryable();

        if (filter.ProductId.HasValue)
        {
            query = query.Where(s => s.ProductId == filter.ProductId.Value);
        }

        if (filter.MovementType.HasValue)
        {
            query = query.Where(s => s.MovementType == filter.MovementType.Value);
        }

        if (filter.DateFrom.HasValue)
        {
            query = query.Where(s => s.CreatedAt >= filter.DateFrom.Value);
        }

        if (filter.DateTo.HasValue)
        {
            query = query.Where(s => s.CreatedAt <= filter.DateTo.Value);
        }

        return await query
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new StockMovementList
            {
                Id = s.Id,
                ProductName = s.Product!.Name,
                ProductSku = s.Product!.Sku,
                Quantity = s.Quantity,
                MovementType = s.MovementType,
                Reason = s.Reason,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<StockMovementList>> GetAllStockMovements()
    {
        return await _db
            .StockMovements
            .Include(s => s.Product)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new StockMovementList
            {
                Id = s.Id,
                ProductName = s.Product!.Name,
                ProductSku = s.Product!.Sku,
                Quantity = s.Quantity,
                MovementType = s.MovementType,
                Reason = s.Reason,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<StockMovementDetail> GetDetail(int id)
    {
        var movement = await _db
            .StockMovements
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (movement == null)
            throw new Exception(ValidationMessages.NotFound("Stock Movement"));

        return new StockMovementDetail
        {
            Id = movement.Id,
            ProductName = movement.Product!.Name,
            ProductSku = movement.Product!.Sku,
            Quantity = movement.Quantity,
            MovementType = movement.MovementType,
            Reason = movement.Reason,
            CreatedAt = movement.CreatedAt
        };
    }

    public async Task<StockMovementForm> GetForEdit(int id)
    {
        var movement = await _db.StockMovements.FindAsync(id);

        if (movement == null)
            throw new Exception(ValidationMessages.NotFound("Stock Movement"));

        return new StockMovementForm
        {
            Id = movement.Id,
            ProductId = movement.ProductId,
            Quantity = movement.Quantity,
            MovementType = movement.MovementType,
            Reason = movement.Reason
        };
    }

    public async Task<(string, string)> CreateStockMovement(StockMovementForm model)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var movement = new StockMovement
            {
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                MovementType = model.MovementType,
                Reason = model.Reason
            };

            _db.StockMovements.Add(movement);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return (ValidationMessages.SUCCESS, ValidationMessages.CreatedMessage("Stock Movement"));
        }
        catch
        {
            await tx.RollbackAsync();
            return (ValidationMessages.ERROR, ValidationMessages.ErrorMessage("creating", "stock movement"));
        }
    }

    public async Task<(StockMovementForm, string, string)> UpdateStockMovement(StockMovementForm model)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var movement = await _db.StockMovements.FindAsync(model.Id);

            if (movement == null)
                return (model, ValidationMessages.ERROR, ValidationMessages.NotFound("Stock Movement"));

            movement.ProductId = model.ProductId;
            movement.Quantity = model.Quantity;
            movement.MovementType = model.MovementType;
            movement.Reason = model.Reason;

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return (model, ValidationMessages.SUCCESS, ValidationMessages.SavedMessage("Stock Movement"));
        }
        catch
        {
            await tx.RollbackAsync();
            return (model, ValidationMessages.ERROR, ValidationMessages.ErrorMessage("saving", "stock movement"));
        }
    }

    public async Task<(string, string)> DeleteManyStockMovements(List<int> ids)
    {
        if (ids == null || ids.Count == 0)
            return (ValidationMessages.ERROR, ValidationMessages.SelectedMessage("stock movements"));

        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var movements = _db.StockMovements.Where(s => ids.Contains(s.Id));
            _db.StockMovements.RemoveRange(movements);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return (ValidationMessages.SUCCESS, ValidationMessages.DeletedMessage("Stock Movement"));
        }
        catch
        {
            await tx.RollbackAsync();
            return (ValidationMessages.ERROR, ValidationMessages.ErrorMessage("deleting", "stock movement"));
        }
    }
}