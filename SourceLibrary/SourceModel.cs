using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace SourceLibrary
{
    //Model for the CSV Import File for Accounts
    public class SourceModel
    {
        public string Account { get; set; }
        public string AccountClass { get; set; }
        public bool Active { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string PostOption { get; set; }
        public bool ConvertBool(string Input)
        {
            if (Input.ToUpper() == "FALSE") return false;
            else return true;
        }
        public List<SourceModel> ReadFile()
        {
              List<SourceModel> csvList = new List<SourceModel>();
                string csvpath = @"C:\Users\tvars\Documents\Chart of Accounts.csv";
                using (var reader = new StreamReader(csvpath))
                {
                    //if (Logging) { Console.WriteLine("Parsing File"); }
                    int x = 0;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        //ignore titles
                        if (x == 0) { }
                        else
                        {
                            SourceModel account = new SourceModel();
                            account.Account = values[0];
                            account.AccountClass = values[1];
                            account.Type = values[2];
                            account.Active = ConvertBool(values[3]);
                            account.Description = values[4];
                            account.PostOption = values[5];
                            csvList.Add(account);
                        }
                        x += 1;
                    }
                    return csvList;
                }
        }
    }
}

