using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DancePro.Services
{
    /// <summary>
    /// This service is called when an exception is thrown and caught by the application.
    /// It allows the logging and error recalling of issues from an error log.
    /// </summary>
    public class ErrorService
    {
        private string FilePath;
        private const string ErrorLogFileName = "ErrorLog.txt";
        List<Error> Errors = new List<Error>();

        public List<Error> GetErrors()
        {

            foreach(Error e in Errors)
            {
                Console.WriteLine(e);
            }
            return Errors;
        }

        public ErrorService(string _ErrorLogDir)
        {
            FilePath = _ErrorLogDir + ErrorLogFileName;
            Init();
        }

        //Setup of service
        public void Init()
        {

            try
            {

                if (System.IO.File.Exists(FilePath))
                {
                    string jsonData = System.IO.File.ReadAllText(FilePath);
                    Errors = JsonConvert.DeserializeObject<List<Error>>(jsonData);
                }

            }
            catch(Exception e)
            {
                LogError(e);
            }


        }


        public void LogError(Exception e)
        {
            Error er = new Error();
            er.Date = DateTime.Now;
            er.Type = Error.ExceptionTypes.Exception;
            er.Message = e.Message;
            er.Source = e.Source;

            Errors.Add(er);

            System.IO.File.WriteAllText(FilePath, JsonConvert.SerializeObject(Errors));
        }

        public void LogError(string message)
        {
            Error er = new Error();
            er.Date = DateTime.Now;
            er.Type = Error.ExceptionTypes.Error;
            er.Message = message;
            er.Source = "Null";

            Errors.Add(er);

            System.IO.File.WriteAllText(FilePath, JsonConvert.SerializeObject(Errors));
        }
    }


    public class Error
    {

        public enum ExceptionTypes
        {
            Exception,
            Error,
            Warning,
            Log
        }


        public DateTime Date;
        public ExceptionTypes Type;
        public string Message;
        public string Source;


        public override string ToString()
        {
            return "[" + Date.ToShortDateString() + "] - " + Message;
        }

    }
}
