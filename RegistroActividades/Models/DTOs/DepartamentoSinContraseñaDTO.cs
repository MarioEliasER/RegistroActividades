namespace RegistroActividades.Models.DTOs
{
    public class DepartamentoSinContraseñaDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string? DepartamentoSuperior { get; set; }
    }
}
