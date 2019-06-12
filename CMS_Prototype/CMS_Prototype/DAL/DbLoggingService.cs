using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Unity.DAL
{
    public class DbLoggingService : ILoggingDataProvider
    {
        public async Task LogError(DateTime date, Guid? requestId, Exception exception)
        {
            var logs = new List<Log>();

            if (exception is DbEntityValidationException)
            {
                var efException = (DbEntityValidationException)exception;
                foreach (var efError in efException.EntityValidationErrors)
                {
                    logs.Add(new Log
                    {
                        GlobalRequestId = requestId,
                        Date = date,
                        Message = string.Join("; ", efError.ValidationErrors.Select(e => e.ErrorMessage)),
                        StackTrace = exception.StackTrace,
                        Type = (int)LogType.Error
                    });
                }
            }

            logs.Add(new Log()
            {
                GlobalRequestId = requestId,
                Date = date,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                Type = (int)LogType.Error
            });

            using (var db = new UnityContext())
            {
                db.Logs.AddRange(logs);
                await db.SaveChangesAsync();
            }
        }

        public async Task LogInfo(DateTime date, Guid? requestId, string message)
        {
            Log log = new Log()
            {
                GlobalRequestId = requestId,
                Date = date,
                Message = message,
                Type = (int)LogType.Info
            };

            using (var db = new UnityContext())
            {
                db.Logs.Add(log);
                await db.SaveChangesAsync();
            }
        }
    }
}