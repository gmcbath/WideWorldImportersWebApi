using Entities;

namespace WideWorldImporters.Api.UnitTests.TestHelpers
{
    internal static class CreateEntities
    {
        /// <summary>
        ///     Return a TestsSupplierForCreationDto Model
        /// </summary>
        /// <returns></returns>
        public static Purchasing_Supplier GetSupplierForCreationDtoModel()
        {
            return new Purchasing_Supplier
                   {
                       SupplierId = 1,
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

    }
}
