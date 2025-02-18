using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AppFundacion.Mensajes
{
    public class DonanteAgregadoMessage : ValueChangedMessage<bool>
    {
        public DonanteAgregadoMessage(bool value) : base(value)
        {
        }
    }
}
