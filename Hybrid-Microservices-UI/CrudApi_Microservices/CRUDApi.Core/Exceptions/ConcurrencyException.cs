using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.Exceptions
{
    public class ConcurrencyException : Exception
    {
        //[DataMember]
        public enum enmExceptionType
        {
            Deleted,
            Modified
        };

        public string ExceptionType { get; set; }


        public object EntityObject { get; set; }
        public ConcurrencyException()
        {
            //ExceptionType=exceptiontype.ToString();
        }
        public ConcurrencyException(string message, enmExceptionType exceptiontype)
            : base(message)
        {
            ExceptionType = exceptiontype.ToString();
        }
        //public ConcurrencyException(string message, enmExceptionType exceptiontype, Exception inner)
        //    : base(message, inner)
        //{
        //    ExceptionType = exceptiontype.ToString();
        //}
    }
}
