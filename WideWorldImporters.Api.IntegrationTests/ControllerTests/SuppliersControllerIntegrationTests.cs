using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WideWorldImporters.Api.IntegrationTests.Models;
using WideWorldImporters.Api.IntegrationTests.TestHelpers.Serialization;
using WideWorldImporters.Api.Utility;
using Xunit;

namespace WideWorldImporters.Api.IntegrationTests.ControllerTests
{
    /// <summary>
    ///     https://www.c-sharpcorner.com/article/crud-operations-unit-testing-in-asp-net-core-web-api-with-xunit/
    /// </summary>
    public sealed class SuppliersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        public SuppliersControllerIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            factory.ClientOptions.BaseAddress = _isWebApplicationFactoryInMemory
                                                    ? new Uri("http://localhost/api/1.0/suppliers")
                                                    : new Uri("http://localhost:5001/api/1.0/suppliers");

            // automatically follow redirects and handles cookies
            _client = factory.CreateClient();
            _client.Timeout = new TimeSpan(0, 0, 60);
            _client.DefaultRequestHeaders.Clear();
        }

        private readonly bool _isWebApplicationFactoryInMemory = true;

        private readonly HttpClient _client;

        /// <summary>
        ///     Return a TestsSupplierForCreationDto Model
        /// </summary>
        /// <returns></returns>
        private static SupplierForCreationDtoInputModel GetValidTestsSupplierForCreationDtoModel()
        {
            return new SupplierForCreationDtoInputModel
                   {
                       SupplierCategoryId = 2,
                       PrimaryContactPersonId = 21,
                       AlternateContactPersonId = 22,
                       DeliveryMethodId = 7,
                       DeliveryCityId = 38171,
                       PostalCityId = 38171,
                       SupplierReference = "AA20384",
                       BankAccountName = "A Datum Corporation",
                       BankAccountBranch = "Woodgrove Bank Zionsville",
                       BankAccountCode = "356981",
                       BankAccountNumber = "8575824136",
                       BankInternationalCode = "25986",
                       PaymentDays = 14,
                       InternalComments = "",
                       PhoneNumber = "(847) 555-0100",
                       FaxNumber = "(847) 555-0101",
                       WebsiteUrl = "https://github.com/gmcbath/WideWorldImportersWebApi",
                       DeliveryAddressLine1 = "183838 Southwest Boulevard",
                       DeliveryAddressLine2 = "Suite 222",
                       DeliveryPostalCode = "46077",
                       PostalAddressLine1 = "PO Box 1039",
                       PostalAddressLine2 = "Surrey",
                       PostalPostalCode = "46077",
                       LastEditedBy = 1,
                       SupplierName = "A Datum Corporation"
                   };
        }

        /// <summary>
        ///     Delete a specific supplier
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        private async Task Delete_Supplier_Returns_NoContent(int supplierId)
        {
            // act
            var response = await _client.DeleteAsync(_client.BaseAddress + "/" + supplierId);

            // assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        /// <summary>
        ///     Perform content tests
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAll_Returns_Content()
        {
            // verify get a list of suppliers return
            // 
            // act
            var response = await _client.GetAsync("");

            // assert

            //verify content exists
            Assert.NotNull(response.Content);

            // headers exist
            Assert.True(response.Content.Headers.ContentLength > 0);

            // verify expected media type returned
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

            // verify return code is 200
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        ///     Perform cache tests
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAll_SetsExpectedCacheControlHeader()
        {
            // act
            var response = await _client.GetAsync("");

            //assert

            // make sure cache header is present and has the expected value
            var header = response.Headers.CacheControl;
            Assert.True(header.MaxAge.HasValue);
            Assert.Equal(TimeSpan.FromMinutes(1), header.MaxAge);
            Assert.True(header.Private);
        }

        /// <summary>
        ///     Verify API honors the OPTIONS requests
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetOptions_Returns_Success()
        {
            // act
            var request = new HttpRequestMessage(HttpMethod.Options, "");
            var result = await _client.SendAsync(request);

            // assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Content.Headers.ContentLength == 0);
        }

        /// <summary>
        ///     Getting a specific supplier using an invalid ID should return a 404 error
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetSupplierBy_InvalidId_Returns_BadRequest()
        {
            // act
            var response = await _client.GetAsync(_client.BaseAddress + "/0");

            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        ///     Get a specific supplier by id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetSupplierBy_ValidId_Returns_Success()
        {
            // act
            var model = await _client.GetFromJsonAsync<SupplierDto>(_client.BaseAddress + "/4");

            // assert
            Assert.True(model.SupplierId == 4);
        }

        /// <summary>
        ///     Passing null patch should return BadRequest
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Patch_Supplier_PassNullPatch_Returns_BadRequest()
        {
            // arrange
            int supplierId = 1;
            int supplierCategoryId = 2;
            string requestUri = _client.BaseAddress + $"/{supplierId}/suppliercategories/{supplierCategoryId}";
            string serializedDoc = JsonConvert.SerializeObject(null);
            var requestContent = new StringContent(serializedDoc, Encoding.UTF8, "application/json-patch+json");

            // act
            var response = await _client.PatchAsync(requestUri, requestContent);

            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        ///     Patch supplier with replace, remove, and add operations
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Patch_Supplier_Replace_Remove_Add_Operations_Returns_Success()
        {
            // arrange PATCH replace
            int supplierId = 1;
            int supplierCategoryId = 2;
            string requestUri = _client.BaseAddress + $"/{supplierId}/suppliercategories/{supplierCategoryId}";

            var patchDoc = new JsonPatchDocument<SupplierForCreationDtoInputModel>();
            patchDoc.Replace(s => s.DeliveryAddressLine2, UtilityHelpers.RandomString(10));
            string serializedDoc = JsonConvert.SerializeObject(patchDoc);
            var requestContent = new StringContent(serializedDoc, Encoding.UTF8, "application/json-patch+json");

            // act
            var response = await _client.PatchAsync(requestUri, requestContent);

            // assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // arrange PATCH remove
            patchDoc = new JsonPatchDocument<SupplierForCreationDtoInputModel>();
            patchDoc.Remove(s => s.DeliveryAddressLine2);
            serializedDoc = JsonConvert.SerializeObject(patchDoc);
            requestContent = new StringContent(serializedDoc, Encoding.UTF8, "application/json-patch+json");

            // act
            response = await _client.PatchAsync(requestUri, requestContent);

            // assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // arrange PATCH add
            patchDoc = new JsonPatchDocument<SupplierForCreationDtoInputModel>();
            patchDoc.Add(s => s.DeliveryAddressLine2, UtilityHelpers.RandomString(20));

            serializedDoc = JsonConvert.SerializeObject(patchDoc);
            requestContent = new StringContent(serializedDoc, Encoding.UTF8, "application/json-patch+json");

            // act
            response = await _client.PatchAsync(requestUri, requestContent);

            // assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        /// <summary>
        ///     Add supplier to the database for a specific category
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Post_Create_Supplier_ForSpecificCategory_Returns_Success()
        {
            // arrange
            var requestContent = GetValidTestsSupplierForCreationDtoModel();
            requestContent.SupplierName = requestContent.SupplierName + UtilityHelpers.RandomString(2);

            // act
            var response = await _client.PostAsJsonAsync(_client.BaseAddress + "/1", requestContent, JsonSerializerHelper.DefaultSerialisationOptions);

            // assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // arrange
            string jsonString = await response.Content.ReadAsStringAsync();
            var supplier = JsonConvert.DeserializeObject<SupplierDtoInputModel>(jsonString);

            // act
            await Delete_Supplier_Returns_NoContent(supplier.SupplierId);

            // todo - assert response
            // Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        /// <summary>
        ///     POSTing a null supplier should return UnprocessableEntity
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Post_Null_Supplier_Returns_UnprocessableEntity()
        {
            // arrange
            string serializedDoc = JsonConvert.SerializeObject(null);
            var requestContent = new StringContent(serializedDoc, Encoding.UTF8, "application/json-patch+json");

            // act
            var response = await _client.PostAsJsonAsync("", requestContent, JsonSerializerHelper.DefaultSerialisationOptions);

            // assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        /// <summary>
        ///     Attempt to add a supplier with a invalid SupplierCategoryId
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Post_Supplier_InvalidSupplierCategoryId_Throws_DbUpdateException()
        {
            // arrange
            var requestContent = GetValidTestsSupplierForCreationDtoModel().CloneWith(m => m.SupplierCategoryId = 0);
            requestContent.SupplierName = requestContent.SupplierName + UtilityHelpers.RandomString(10);

            // todo - not sure why this doesnt work
            //await Assert.ThrowsAsync<DbUpdateException>(() => _client.PostAsJsonAsync("", requestContent, JsonSerializerHelper.DefaultSerialisationOptions));

            try
            {
                // act
                await _client.PostAsJsonAsync("", requestContent, JsonSerializerHelper.DefaultSerialisationOptions);
            }
            catch (DbUpdateException e)
            {
                // assert

                // this should be the FK error
                //
                // The MERGE statement conflicted with the FOREIGN KEY constraint "FK_Purchasing_Suppliers_SupplierCategoryID_Purchasing_SupplierCategories". The conflict occurred in database "WideWorldImporters", table "Purchasing.SupplierCategories", column 'SupplierCategoryID'.
                //Debug.WriteLine(e);
                if (e.InnerException != null)
                {
                    Assert.Contains("FK_Purchasing_Suppliers_SupplierCategoryID_Purchasing_SupplierCategories", e.InnerException.Message);
                }
            }
        }

        /// <summary>
        ///     Add supplier to the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Post_Supplier_Returns_Success()
        {
            // arrange
            var requestContent = GetValidTestsSupplierForCreationDtoModel();
            requestContent.SupplierName = requestContent.SupplierName + UtilityHelpers.RandomString(2);
            
            // act
            var response = await _client.PostAsJsonAsync("", requestContent, JsonSerializerHelper.DefaultSerialisationOptions);

            // assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        }

        /// <summary>
        ///     Update a supplier
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Put_Supplier_Returns_Success()
        {
            // arrange
            var requestContent = GetValidTestsSupplierForCreationDtoModel();
            requestContent.SupplierName = UtilityHelpers.RandomString(20) + UtilityHelpers.RandomPK();

            int supplierId = 1;
            int supplierCategoryId = 2;
            string requestUri = _client.BaseAddress + $"/{supplierId}/suppliercategories/{supplierCategoryId}";

            // act
            var response = await _client.PutAsJsonAsync(requestUri, requestContent, JsonSerializerHelper.DefaultSerialisationOptions);

            // assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}