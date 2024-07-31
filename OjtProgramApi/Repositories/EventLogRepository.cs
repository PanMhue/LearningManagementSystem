using System.Text.Json;
using OjtProgramApi.DTO;
using OjtProgramApi.Models;

namespace OjtProgramApi.Repositories
{
    public class EventLogRepository : RepositoryBase<EventLog>, IEventLogRepository
    {
        public EventLogRepository(AppDB context)
            : base(context) { }

        public async Task InsertEventLog(
            string controllerName,
            string functionname,
            string logMessage
        )
        {
            EventLog eventLog = new EventLog
            {
                log_type = (int)LogType.Insert, // 3
                log_datetime = DateTime.Now,
                log_message = "Created a new data " + logMessage,
                error_message = "",
                form_name = functionname,
                source = controllerName
            };
            Create(eventLog);
            await Save();
        }

        public async Task UpdateEventLog(
            string controllerName,
            string functionname,
            string logMessage
        )
        {
            EventLog eventLog = new EventLog
            {
                log_type = (int)LogType.Update, // 4
                log_datetime = DateTime.Now,
                log_message = "Updated data: " + logMessage,
                error_message = "",
                form_name = functionname,
                source = controllerName
            };
            Create(eventLog);
            await Save();
        }

        public async Task ErrorEventLog(
            string controllerName,
            string functionname,
            string logMessage,
            string errorMessage
        )
        {
            EventLog eventLog = new EventLog
            {
                log_type = (int)LogType.Error, // 2
                log_datetime = DateTime.Now,
                log_message = logMessage,
                error_message = errorMessage,
                form_name = functionname,
                source = controllerName
            };
            Create(eventLog);
            await Save();
        }

        public async Task DeleteEventLog(
            string controllerName,
            string functionname,
            string logMessage
        )
        {
            EventLog eventLog = new EventLog
            {
                log_type = (int)LogType.Delete, // 5
                log_datetime = DateTime.Now,
                log_message = "Deleted data: " + logMessage,
                error_message = "",
                form_name = functionname,
                source = controllerName
            };
            Create(eventLog);
            await Save();
        }
    }
}
