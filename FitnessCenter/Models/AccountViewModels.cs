using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Името е задолжително.")]
        [Display(Name ="Име")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Презимето е задолжително.")]
        [Display(Name = "Презиме")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Е-поштата е задолжителна.")]
        [EmailAddress]
        [Display(Name = "Е-пошта")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Лозинката е задолжителна.")]
        [StringLength(100, ErrorMessage = "Лозинката мора да има најмалку {2} карактери.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Лозинка")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Потврди лозинка")]
        [Compare("Password", ErrorMessage = "Лозинките не се совпаѓаат.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage ="Висината е задолжителна.")]
        [Range(100,250,ErrorMessage = "Внеси валидна висина (во сантиметри).")]
        [Display(Name ="Висина (cm)")]
        public double Height { get; set; }

        [Required(ErrorMessage ="Телесната маса е задолжителна")]
        [Range(30,250,ErrorMessage = "Внеси валидна телесна маса (во килограми).")]
        [Display(Name ="Телесна маса (kg)")]
        public double Weight { get; set; }

        [Required(ErrorMessage ="Целта е задолжителна.")]
        [Display(Name ="Цел")]
        public Goal Goal { get; set; }

        [Required(ErrorMessage ="Полето за пол е задолжително.")]
        [Display(Name ="Пол")]
        public Gender Gender { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
