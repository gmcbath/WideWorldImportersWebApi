﻿using System.ComponentModel.DataAnnotations;

namespace WideWorldImporters.Api.UnitTests.Models
{
    public abstract class SupplierForManipulationDtoInputModel
    {
        /// <summary>
        ///     Supplier&apos;s full name (usually a trading name)
        /// </summary>
        [Required(ErrorMessage = "SupplierName name is a required field.")]
        [MaxLength(100, ErrorMessage = "Maximum length for the SupplierName is 100 characters.")]
        public string SupplierName { get; set; } // SupplierName (length: 100)
    }
}