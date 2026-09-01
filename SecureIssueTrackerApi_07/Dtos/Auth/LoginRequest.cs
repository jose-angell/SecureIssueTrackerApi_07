using System.ComponentModel.DataAnnotations;

namespace SecureIssueTrackerApi_07.Dtos.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [StringLength(200, ErrorMessage = "El correo no puede tener mas de 200 caracteres")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatorio.")]
        public string? Password { get; set; }
    }
}
