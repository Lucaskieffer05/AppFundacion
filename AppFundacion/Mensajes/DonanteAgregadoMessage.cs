using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFundacion.Mensajes
{
    public class DonanteAgregadoMessage : ValueChangedMessage<bool>
    {
        public DonanteAgregadoMessage(bool value) : base(value)
        {
        }
    }
}
