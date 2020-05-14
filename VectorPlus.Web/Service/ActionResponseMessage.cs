using System;
using System.Collections.Generic;
using System.Linq;

namespace VectorPlus.Web.Service
{
    public enum ActionResponseState
    {
        Success,
        Failure,
        Information,
        Warning,
        NOP
    }

    public class ActionResponseMessage
    {
        public ActionResponseState State { get; set; }
        public string Message { get; set; }

        public Exception Exception { get; set; }

        public static ActionResponseMessage Success(string message)
        {
            return new ActionResponseMessage()
            {
                State = ActionResponseState.Success,
                Message = message
            };
        }

        public static ActionResponseMessage Failure(Exception e, string message = null)
        {
            return new ActionResponseMessage()
            {
                State = ActionResponseState.Failure,
                Exception = e,
                Message = message ?? e.Message
            };
        }

        public static ActionResponseMessage Failure(string message)
        {
            return new ActionResponseMessage()
            {
                State = ActionResponseState.Failure,
                Message = message
            };
        }

        public static ActionResponseMessage Failure(List<string> messages)
        {
            return new ActionResponseMessage()
            {
                State = ActionResponseState.Failure,
                Message = "<ul>" + string.Join("", messages.Select(m => "<li>" + m + "</li>")) + "</ul>"
            };
        }

        public static ActionResponseMessage NOP
        {
            get
            {
                return new ActionResponseMessage()
                {
                    State = ActionResponseState.NOP,
                    Message = "No actions."
                };
            }
        }
    }

}
