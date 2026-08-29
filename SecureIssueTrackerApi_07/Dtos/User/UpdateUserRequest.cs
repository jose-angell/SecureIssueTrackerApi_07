using SecureIssueTrackerApi_07.Domain;
using System.ComponentModel.DataAnnotations;

namespace SecureIssueTrackerApi_07.Dtos.User
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(300, ErrorMessage = "El nombre no puede tener mas de 300 caracteres")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [StringLength(200, ErrorMessage = "El correo no puede tener mas de 200 caracteres")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatorio.")]
        public string? PasswordHash { get; set; }

        [Required(ErrorMessage = "El Role es obligatorio.")]
        public UserRole? Role { get; set; }
    }
}
