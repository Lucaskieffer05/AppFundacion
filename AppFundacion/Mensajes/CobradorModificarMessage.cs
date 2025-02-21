using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AppFundacion.Mensajes
{
    public class CobradorModificarMessage : ValueChangedMessage<bool>
    {
        public CobradorModificarMessage(bool value) : base(value)
        {
        }
    }
}
