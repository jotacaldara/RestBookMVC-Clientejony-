using System.ComponentModel.DataAnnotations;

namespace RestBookMVC__Cliente_.DTOs
{
    public class LoginDTO
        {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Insira um e-mail válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        }
    }
