using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AppSiacpiMovil.Host
{
    public class XmlOperations
    {
        public DataSet ReadXml(string pNameXml)
        {

            string PathXMLFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            PathXMLFile += "\\" + pNameXml;

            DataSet ds = new DataSet();

            try
            {
                ds.ReadXml(PathXMLFile);
                return ds;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error crítico: " + ex.Message);
                return null;
            }


        }        
    }
}