namespace InstitutoTriboDeDavi.API.ViewModels.Create
{
    // Inscrição de push vinda do navegador (PushSubscription.toJSON()).
    public class InscreverPushViewModel
    {
        public string Endpoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }
    }

    // Cancelamento: basta o endpoint do dispositivo.
    public class DesinscreverPushViewModel
    {
        public string Endpoint { get; set; }
    }
}
