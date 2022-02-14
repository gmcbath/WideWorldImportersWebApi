using System.Threading.Tasks;

namespace Interfaces
{
    public interface IRepositoryManager
    {
        ISupplierCategoriesRepository SupplierCategories { get; }
        ISupplierRepository Supplier { get; }
        ISupplierTransactionsRepository SupplierTransactions { get; }

        Task SaveAsync();
    }
}