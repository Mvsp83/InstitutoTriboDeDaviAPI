namespace InstitutoTriboDeDavi.System.DTO
{
    public class UsuarioDTO
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
