using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Repository;
using WideWorldImporters.Api.UnitTests.TestHelpers;
using WideWorldImporters.Api.Utility;
using Xunit;

namespace WideWorldImporters.Api.UnitTests.Database
{
    public sealed class SuppliersDatabaseUnitTests : IClassFixture<UnitTestBase>
    {
        public SuppliersDatabaseUnitTests(UnitTestBase unitTestBase) { _unitTestBase = unitTestBase; }

        private readonly UnitTestBase _unitTestBase;

        
        /// <summary>
        ///     Create, retrieve, and delete a supplier should pass
        /// </summary>
        [Fact]
        public void Create_Retrieve_Delete_Supplier_Returns_Success()
        {
            // arrange
            var requestContent = CreateEntities.GetSupplierForCreationDtoModel();
            requestContent.SupplierId = UtilityHelpers.RandomPK();

            // act
            using (var context = new RepositoryContext(_unitTestBase.InMemoryDatabase, _unitTestBase.AppConfigSettings))
            {
                context.Purchasing_Suppliers.Add(requestContent);
                context.SaveChanges();
            }

            using (var context = new RepositoryContext(_unitTestBase.InMemoryDatabase, _unitTestBase.AppConfigSettings))
            {
                var supplierRepository = new SupplierRepository(context);

                // assert
                Task<Purchasing_Supplier> supplier = supplierRepository.GetSupplierAsync(requestContent.SupplierId, false);
                Assert.Equal(requestContent.SupplierId, supplier.Result.SupplierId);

                // act
                supplierRepository.DeleteSupplier(supplier.Result);
                context.SaveChanges();

                // assert
                Task<Purchasing_Supplier> supplierDeleted = supplierRepository.GetSupplierAsync(requestContent.SupplierId, false);
                Assert.Null(supplierDeleted.Result);
            }
        }

        /// <summary>
        ///     Create a Configuration object from the appsettings.json file
        /// </summary>
        [Fact]
        public void CreateConfiguration_FromFile_Returns_ValidObject()
        {
            // todo - get from _unitTestBase.ConfigurationRoot
            Assert.True(_unitTestBase.AppConfigSettings.Value.DeleteLog);
        }

        /// <summary>
        ///     Create an IOptions AppConfigSettings object
        /// </summary>
        [Fact]
        public void CreateIOptionsObject_Returns_ValidObject() { Assert.True(_unitTestBase.AppConfigSettings.Value.DeleteLog); }

        /// <summary>
        ///     Get all suppliers
        /// </summary>
        [Fact]
        public void GetAllSuppliers()
        {
            // arrange
            using (var context = new RepositoryContext(_unitTestBase.InMemoryDatabase, _unitTestBase.AppConfigSettings))
            {
                // todo - move to seed

                var requestContent1 = CreateEntities.GetSupplierForCreationDtoModel();
                requestContent1.SupplierId = UtilityHelpers.RandomPK();
                context.Purchasing_Suppliers.Add(requestContent1);

                var requestContent2 = CreateEntities.GetSupplierForCreationDtoModel();
                requestContent2.SupplierId = UtilityHelpers.RandomPK();
                context.Purchasing_Suppliers.Add(requestContent2);

                context.SaveChanges();
            }

            using (var context = new RepositoryContext(_unitTestBase.InMemoryDatabase, _unitTestBase.AppConfigSettings))
            {
                // act
                var supplierRepository = new SupplierRepository(context);
                Task<IEnumerable<Purchasing_Supplier>> suppliers = supplierRepository.GetSuppliersAsync(false);

                // assert
                Assert.True(suppliers.Result.Count() >= 2);
            }
        }

        /// <summary>
        ///     Get supplier by invalid id should fail
        /// </summary>
        [Fact]
        public void GetSupplierBy_InvalidId_Returns_Success()
        {
            // arrange
            var requestContent = CreateEntities.GetSupplierForCreationDtoModel();
            requestContent.SupplierId = UtilityHelpers.RandomPK();

            // act
            using (var context = new RepositoryContext(_unitTestBase.InMemoryDatabase, _unitTestBase.AppConfigSettings))
            {
                context.Purchasing_Suppliers.Add(requestContent);
                context.SaveChanges();
            }

            // assert
            using (var context = new RepositoryContext(_unitTestBase.InMemoryDatabase, _unitTestBase.AppConfigSettings))
            {
                var supplierRepository = new SupplierRepository(context);
                Task<Purchasing_Supplier> supplier = supplierRepository.GetSupplierAsync(0, false);
                Assert.Null(supplier.Result);
            }
        }
    }
}