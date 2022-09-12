using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using AcumaticaLibrary.AcumaticaEndPoint18;
using System.Net.Http;

namespace AcumaticaLibrary
{
    
    public class AcumaticaClient
    {
        bool Logging;
        bool Failed=false; 
        string Failure=""; //Error Message from Acumatica
        
        DefaultSoapClient soapClient;
        public AcumaticaClient(DefaultSoapClient _soapclient, bool willLog)
        {
            soapClient = _soapclient;
            Logging = willLog;
        }
        public void LogE(Exception e)
        {
            if (Logging) Console.WriteLine(e.GetType() + " " + DateTime.Now);
            if (e.GetType().Equals(typeof(System.ServiceModel.FaultException)) && e.Message.Contains("PX.Data.PXOuterException"))
            {
                string AcuErr;
                string[] separator = { "PX.Data" };
                string[] AcuErrors = e.Message.Split(separator, 20, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in AcuErrors)
                {
                    if (s.Contains("OuterException"))
                    {
                        AcuErr = s.Replace(".", "");
                        if (Logging) Console.WriteLine(AcuErr);
                        Failed = true;
                        Failure = AcuErr;
                    }
                }
            }
            else { if (Logging) Console.WriteLine(e.Message); }
        }
        public bool LogOff()
        {
            //Sign out of Acumatica ERP
            soapClient.Logout();
            if (Logging) { Console.WriteLine("Logged Off"); }
            return true;
        }
        public bool LogOn()
        {
            //Sign in to Acumatica ERP
            if (Logging) Console.WriteLine("Logging On, Stand by...");
            soapClient.Login
               (
               Properties.Settings1.Default.User,
               Properties.Settings1.Default.Password,
               Properties.Settings1.Default.Tenant,
               Properties.Settings1.Default.Company,
               Properties.Settings1.Default.Locale);
            if (Logging) Console.WriteLine("...Logged On");
            return true;
        }
        public bool ConvertBool(string Input)
        {
            if (Input.ToUpper() == "FALSE") return false;
            else return true;
        }
        public List<Account> ReadFile()
        {
            List<Account> accounts = new List<Account>();            
            string csvpath = @"C:\Users\tvars\Documents\Chart of Accounts.csv";
            using (var reader = new StreamReader(csvpath))
            {
                if(Logging) { Console.WriteLine("Parsing File"); }
                int x = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    //ignore titles
                    if (x == 0) { }
                    else
                    {
                        Account account = new Account();
                        account.AccountCD = new StringValue { Value = values[0] };
                        account.AccountClass = new StringValue { Value = values[1] };
                        account.Type = new StringValue { Value = values[2] };
                        account.Active = new BooleanValue { Value = ConvertBool(values[3]) };
                        account.Description = new StringValue { Value = values[4] };
                        account.PostOption = new StringValue { Value = values[5] };
                        accounts.Add(account);
                    }
                    x += 1;
                }
                return accounts;
            }
        }

        public bool LoadList(List<Account> accounts)
        {
            try
            {
                if (Logging) { Console.WriteLine("Loading File"); }
                //List<Account> accounts = new List<Account>();
                accounts = ReadFile();
                if (!LogOn()) { throw new Exception("Cant Log on"); }
                foreach (Account account in accounts)
                {
                    if (Logging) { Console.WriteLine("Loading Account " + account.AccountCD.Value.ToString()); }
                    Account newAcct = (Account)soapClient.Put(account);
                }
            }
            catch (Exception e)
            {
                LogE(e);
                return false;
            }
            finally
            {
                LogOff();
            }
            if (Failed == true) return false;
            else return true;
        }
        public bool LoadList()
        {
            try
            {
                if (Logging) { Console.WriteLine("Loading File"); }
                List<Account> accounts = new List<Account>();
                accounts = ReadFile();
                if (!LogOn()) { throw new Exception("Cant Log on"); }
                foreach (Account account in accounts)
                {
                    if (Logging) { Console.WriteLine("Loading Account "+account.AccountCD.Value.ToString()); }
                    Account newAcct = (Account)soapClient.Put(account);
                }                
            }
            catch (Exception e)
            {
                LogE(e);
                return false;
            }
            finally
            {
                LogOff();
            }
            if (Failed == true) return false;
            else return true;
        }

    }
}
