using NServiceBus;

using System;

namespace Messages.NServiceBus.Commands
{
    /// <summary>
    /// Represents a request to search for companys who fit the description given in the delim
    /// </summary>
    [Serializable]
    public class SearchForCompany : ICommand
    {
        /// <summary>
        /// The information to filter companies with
        /// </summary>
        public string delim { get; set; }
    }
}
