using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Bikes.Models
{
    public class Order
    {
        internal string ShipppingPostalCode;
        [Key]
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateCreated { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Total price is required")]
        [Column(TypeName = "decimal(9,2)")]
        public decimal Total { get; set; }
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, MinimumLength = 2)]
        #region Billing
        public string BillingFirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, MinimumLength = 2)]

        public string BillingLastName { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [StringLength(200)]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]

        public string BillingEmail { get; set; }
        [Required(ErrorMessage = "Phone number address is required")]
        [StringLength(50)]
        [DataType(DataType.PhoneNumber)]
        public string BillingPhone { get; set; }
        [Required(ErrorMessage = "Address is required")]
        [StringLength(200)]
        public string BillingAddress { get; set; }
        [Required(ErrorMessage = "City is required")]
        [StringLength(200)]
        public string BillingCity { get; set; }
        [Required(ErrorMessage = "PostalCode is required")]
        [StringLength(10)]
        [DataType(DataType.PostalCode)]
        public string BillingPostalCode { get; set; }
        [Required(ErrorMessage = "Country is required")]
        [StringLength(200)]
        public string BillingCountry { get; set; }
        #endregion
        //shipping
        #region Shipping
        [StringLength(100)]
        [Required(ErrorMessage = "First name is required")]
        public string ShipppingFirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100)]
        public string ShipppingLastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [StringLength(100)]
        public string SipppingEmail { get; set; }
        [Required(ErrorMessage = "PhoneNumber is required")]
        [StringLength(200)]
        public string SipppingPhoneNumber { get; set; }
        [Required(ErrorMessage = "Address is required")]
        [StringLength(200)]
        public string SipppingAddress { get; set; }
        [Required(ErrorMessage = "City is required")]
        [StringLength(200)]
        public string SipppingCity { get; set; }
        [Required(ErrorMessage = "PostalCode is required")]
        [StringLength(200)]
        public string SipppingPostalCode { get; set; }
        [Required(ErrorMessage = "Country is required")]
        [StringLength(200)]
        public string SipppingCountry { get; set; }
        [AllowNull]
        public string Message { get; set; }
        public string UserId { get; set; }
        [ForeignKey("OrderId")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }






        #endregion
    }

}