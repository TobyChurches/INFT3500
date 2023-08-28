using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.ViewModels
{
    public class EditAccountViewModel
    {
        [Required(ErrorMessage = "Please Enter a Username.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please Enter an Email.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Enter a Name.")]
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
