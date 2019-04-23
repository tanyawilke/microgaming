using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace FinanceRequest.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RequestModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please provide a title.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please provide a description.")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please provide a charity.")]
        [Display(Name = "Charity")]
        [DataType(DataType.MultilineText)]
        public string Charity { get; set; }

        [Required(ErrorMessage = "Previously supported by PlayItForward?")]
        [Display(Name = "PlayItForward")]
        public bool PlayItForward { get; set; }

        public RequestModel()
        {
            Amount = Convert.ToDecimal(0.00);
        }

        [Range(1, double.MaxValue, ErrorMessage = "Please enter valid amount.")]
        [Required(ErrorMessage = "Please provide a charity.")]
        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Display(Name = "Status")]
        public int StatusId { get; set; }
        [ForeignKey(nameof(StatusId))]
        public virtual StatusModel Status { get; set; }

        [Display(Name = "Submission date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? SubmissionDate { get; set; }

        [Display(Name = "Last modified date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? ModifyDate { get; set; }

        [Required]
        public string User { get; set; }
        [ForeignKey(nameof(User))]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [MaxLength(50)]
        public string ConfirmationCode { get; set; }

    }

    public class StatusModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class AttachmentModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }
        [StringLength(400)]
        [Required(ErrorMessage = "Please select a file to upload.")]
        [Display(Name = "Browse file(s)")]
        [DataType(DataType.Upload)]
        public string File { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public int RequestId { get; set; }
        [ForeignKey(nameof(RequestId))]
        public virtual RequestModel Request { get; set; }
    }
}