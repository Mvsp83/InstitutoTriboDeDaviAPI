namespace InstitutoTriboDeDavi.Infrastructure
{
    /// Horário de Brasília independente do relógio do servidor —
    /// em nuvem o padrão é UTC, o que deslocaria o sync noturno e os
    /// registros de histórico em 3 horas.
    public static class FusoBrasil
    {
        private static readonly TimeZoneInfo _fuso = ObterFuso();

        public static DateTime Agora =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _fuso);

        private static TimeZoneInfo ObterFuso()
        {
            // Id IANA (Linux/containers); fallback para o id do Windows;
            // em último caso, o fuso local do servidor
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                }
                catch (TimeZoneNotFoundException)
                {
                    return TimeZoneInfo.Local;
                }
            }
        }
    }
}
