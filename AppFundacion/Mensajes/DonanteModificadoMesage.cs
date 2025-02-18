using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AppFundacion.Mensajes
{
    public class DonanteModificadoMessage : ValueChangedMessage<bool>
    {
        public DonanteModificadoMessage(bool value) : base(value)
        {
        }
    }
}
