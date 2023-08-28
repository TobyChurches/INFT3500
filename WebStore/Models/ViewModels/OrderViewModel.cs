using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.ViewModels
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Please enter a Address.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please enter a Postcode.")]
        [Range(0000, 9999, ErrorMessage = "Postcode must be 4 digits.")]
        public int Postcode { get; set; }
        [Required(ErrorMessage = "Please enter a Suburb.")]
        public string Suburb { get; set; }
        [Required(ErrorMessage = "Please enter a State.")]
        public string State { get; set; }
    }
}
