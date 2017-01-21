using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace AppSiacpiMovil.Host
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISiacpiMensaje" in both code and config file together.
    [ServiceContract]
    public interface ISiacpiMensaje
    {
        [OperationContract]
        bool EnviarMensaje(string _pTo, string _pMsg);

        [OperationContract]
        bool EnviarEmail(string _pEmailTo, string _pNombreTo, string _pNombreFrom, string _pTipoDocumento, string _pAsunto, string _pContenido);

    }
}
