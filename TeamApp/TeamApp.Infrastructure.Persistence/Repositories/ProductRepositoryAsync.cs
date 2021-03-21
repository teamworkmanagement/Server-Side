using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Domain.Models.Task;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.Infrastructure.Persistence.Repository;
using Task = TeamApp.Infrastructure.Persistence.Entities.Task;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class ProductRepositoryAsync : GenericRepositoryAsync<TaskRequest>, IProductRepositoryAsync
    {
        private readonly DbSet<Task> _products;

        public ProductRepositoryAsync(KhoaLuanContext dbContext) : base(dbContext)
        {
            _products = dbContext.Set<Task>();
        }

        public Task<bool> IsUniqueBarcodeAsync(string barcode)
        {
            return System.Threading.Tasks.Task.FromResult(true);
        }
    }
}
