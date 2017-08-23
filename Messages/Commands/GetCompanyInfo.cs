using NServiceBus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Commands
{
    /// <summary>
    /// Represents a request for information about a specific company
    /// </summary>
    public class GetCompanyInfo : ICommand
    {
        /// <summary>
        /// The name of the company to get information on
        /// </summary>
        public string companyName { get; set; }
    }
}
