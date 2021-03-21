using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Domain.Models;
using TeamApp.Domain.Models.Task;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IProductRepositoryAsync : IGenericRepositoryAsync<TaskRequest>
    {
        Task<bool> IsUniqueBarcodeAsync(string barcode);
    }
}
