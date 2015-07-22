using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Linqpad.Server
{
    public class ServerResponse<T>
    {
        public ServerResponse()
        {
            this.Exceptions = new List<Exception>();
        }
        public T Data { get; set; }

        public void AddException(Exception ex)
        {
            if (ex != null)
            {
                this.Exceptions.Add(ex);
            }
        }
        public IEnumerable<Exception> GetExceptions()
        {
            return this.Exceptions;
        }
        private List<Exception> Exceptions { get; set; }
    }
}
