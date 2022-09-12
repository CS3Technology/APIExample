using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcumaticaLibrary;
using AcumaticaLibrary.AcumaticaEndPoint18;
using SourceLibrary;

namespace SimpleUI
{
    class UIController
    {
        void Output(string message)
        {
            Console.WriteLine(message);
        }
        string GetInput()
        {
            string message;
            message = Console.ReadLine().ToUpper();
            return message;
        }

        List<Account> ConvertCSVtoAccounts(List<SourceModel> csvList)
        {
            var AccountList = new List<Account>();
            foreach(SourceModel source in csvList)
            {
                var account = new Account();
                account.AccountCD = new StringValue { Value = source.Account };
                account.AccountClass = new StringValue { Value = source.AccountClass };
                account.Type = new StringValue { Value = source.Type };
                account.Description = new StringValue { Value = source.Description };
                account.Active = new BooleanValue { Value = source.Active };
                account.PostOption = new StringValue { Value = source.PostOption };
                AccountList.Add(account);
            }
            return AccountList;
        }
        void Act(string Input, AcumaticaClient client)
        {
            if (Input.Length == 0) { Input = "H"; }
            switch (Input.Substring(0, 1))
            {
                case "L":
                    SourceModel a = new SourceModel();
                    List<SourceModel> csvList = a.ReadFile();
                    List<Account> Loadlist = ConvertCSVtoAccounts(csvList);
                    client.LoadList(Loadlist);
                    break;
                case "H":
                    Console.WriteLine("X - Exit, H- Help, L- Load List to Acumatica");
                    break;
                case "X":
                    break;
            }
        }

        public UIController()
        {
            using (AcumaticaLibrary.AcumaticaEndPoint18.DefaultSoapClient soap = new AcumaticaLibrary.AcumaticaEndPoint18.DefaultSoapClient())
            {
                AcumaticaClient client = new AcumaticaClient(soap,true);
                string Value;
                do
                {
                    Output("press X to continue or H for Help");
                    Value = GetInput();
                    Act(Value,client); 
                } while (Value != "X");
            }
        }
    }
}
