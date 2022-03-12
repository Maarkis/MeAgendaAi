namespace MeAgendaAi.Domains.RequestAndResponse
{
    public class PhysicalPersonResponse
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Name { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string CPF { get; set; } = default!;
    }
}