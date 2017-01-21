using GsmComm.GsmCommunication;
using GsmComm.PduConverter;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace AppSiacpiMovil.Host
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SiacpiMensaje" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SiacpiMensaje.svc or SiacpiMensaje.svc.cs at the Solution Explorer and start debugging.
    public class SiacpiMensaje : ISiacpiMensaje
    {
        #region ___ENVIO DE MENSAJE DE TEXTO A CELULARES___
        private GsmCommMain comm = null;
        public bool EnviarMensaje(string _pTo, string _pMsg)
        {
            bool result = true;
            SmsSubmitPdu pdu;
            if (comm == null)
            {
                if (!_fnInicializarServicio())
                {
                    result = false;
                }                

            }
            else if (!comm.IsOpen())
            {
                Cursor.Current = Cursors.WaitCursor;
                comm.Open();
                Cursor.Current = Cursors.Default;
            }

            if (result)
            {
                if (comm.IsConnected())
                {
                    Cursor.Current = Cursors.WaitCursor;
                    try
                    {
                      
                        pdu = new SmsSubmitPdu(_pMsg, _pTo);
                        comm.SendMessage(pdu);
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        _fnExceptionWrite(ex);
                        result = false;
                    }
                    comm.Close();
                }
                else
                {
                    result = false;
                }
            }
            Cursor.Current = Cursors.Default;
            comm = null;
            return result;
        }
        private bool _fnInicializarServicio()
        {
            string portName = GsmCommMain.DefaultPortName;
            int baudRate = GsmCommMain.DefaultBaudRate;
            int timeout = GsmCommMain.DefaultTimeout;
            _fnGetGetData(out portName, out baudRate, out timeout);
            bool retry;
            //do
            //{
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                comm = new GsmCommMain(portName, baudRate, timeout);
                Cursor.Current = Cursors.Default;
                Cursor.Current = Cursors.WaitCursor;
                comm.Open();
                Cursor.Current = Cursors.Default;
                retry = true;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                _fnExceptionWrite(ex);
                retry = false;
            }
            //}
            //while (retry);
            return retry;

        }

        private void _fnGetGetData(out string portName, out int baudRate, out int timeout)
        {
            XmlOperations _xmlOp = new XmlOperations();
            string _smsConfig = "MenssageConfig.xml";
            DataSet dsXmlMessages = _xmlOp.ReadXml(_smsConfig);
            portName = dsXmlMessages.Tables["ConfigSiacpiMovil"].Rows[0]["Puerto"].ToString();
            baudRate = Convert.ToInt32(dsXmlMessages.Tables["ConfigSiacpiMovil"].Rows[0]["Velocidad"].ToString());
            timeout = Convert.ToInt32(dsXmlMessages.Tables["ConfigSiacpiMovil"].Rows[0]["TimeOut"].ToString());
        }
        #endregion

        #region ___ENVIO DE CORREO ELECTRONICO_____
        public bool EnviarEmail(string _pEmailTo, string _pNombreTo, string _pNombreFrom, string _pTipoDocumento, string _pAsunto, string _pContenido)
        {
            try
            {
                string _pEmail, _pPassword, _pEmailServer;
                int _pEmailPuerto;
                string To = _pEmailTo;
                string Subject = _pAsunto;
                MailMessage mail;
                mail = new MailMessage();
                mail.To.Add(new MailAddress(To));
                fnGetCredentials(out _pEmail, out _pPassword, out _pEmailServer, out _pEmailPuerto);
                mail.From = new MailAddress(_pEmail, _pNombreFrom);
                mail.Subject = Subject;
                string filePath = _fnGetUrlTemplate();
                StreamReader reader = new StreamReader(filePath + "_template.htm");
                string body = reader.ReadToEnd();
                body = body.Replace("@Documento", _pTipoDocumento);
                body = body.Replace("@NombreTo", _pNombreTo);
                body = body.Replace("@NombreFrom", _pNombreFrom);
                body = body.Replace("@DetalleDocumento", _pContenido);
                body = body.Replace("@Ejercicio", Convert.ToString(DateTime.Now.Year));


                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpClient client = new SmtpClient(_pEmailServer, _pEmailPuerto);
                client.UseDefaultCredentials = true;
                using (client)
                {
                    client.Credentials = new System.Net.NetworkCredential(_pEmail, _pPassword);
                    client.EnableSsl = false;
                    client.Send(mail);
                }
                return true;
            }
            catch (Exception ex)
            {
                _fnExceptionWrite(ex);
                return false;
            }
        }

        private void fnGetCredentials(out string _pEmail, out string _pPassword, out string _pEmailServer, out int _pEmailPuerto)
        {
            XmlOperations _xmlOp = new XmlOperations();
            string _smsConfig = "MenssageConfig.xml";
            DataSet dsXmlMessages = _xmlOp.ReadXml(_smsConfig);
            _pEmail = dsXmlMessages.Tables["ConfigSiacpiMovil"].Rows[0]["Email"].ToString();
            _pPassword = dsXmlMessages.Tables["ConfigSiacpiMovil"].Rows[0]["Password"].ToString();
            _pEmailServer = dsXmlMessages.Tables["ConfigSiacpiMovil"].Rows[0]["EmailServer"].ToString();
            _pEmailPuerto = Convert.ToInt32(dsXmlMessages.Tables["ConfigSiacpiMovil"].Rows[0]["EmailPort"].ToString());
        }
        private string _fnGetUrlLog()
        {
            XmlOperations _xmlOp = new XmlOperations();
            string _smsConfig = "MenssageConfig.xml";
            DataSet dsXmlMessages = _xmlOp.ReadXml(_smsConfig);
            string UrlLog = dsXmlMessages.Tables["ConfigSiacpiMovil"].Rows[0]["UrlLog"].ToString();
            return UrlLog;
        }
        private string _fnGetUrlTemplate()
        {
            XmlOperations _xmlOp = new XmlOperations();
            string _smsConfig = "MenssageConfig.xml";
            DataSet dsXmlMessages = _xmlOp.ReadXml(_smsConfig);
            string UrlTemplate = dsXmlMessages.Tables["ConfigSiacpiMovil"].Rows[0]["UrlTemplate"].ToString();
            return UrlTemplate;
        }
        #endregion

        #region ____LOG DE ERRORES____
        public void _fnExceptionWrite(Exception e)
        {
            string UrlLog = _fnGetUrlLog();

            if (!(Directory.Exists(UrlLog)))
            {
                Directory.CreateDirectory(UrlLog);
            }
            using (StreamWriter sw = System.IO.File.AppendText(Path.Combine(UrlLog, String.Format("{0:yyyy-MM-dd}", DateTime.Today) + ".txt")))
            {
                sw.WriteLine("--------- " + Convert.ToString(DateTime.Now) + " --------- --------- --------- --------- ---------");
                if (e != null)
                {
                    sw.WriteLine("Message: " + e.Message);
                    sw.WriteLine("Stack Trace: ");
                    sw.WriteLine(e.StackTrace);
                    sw.WriteLine("");
                }
                sw.Close();
            }
        }
        #endregion
    }
}
