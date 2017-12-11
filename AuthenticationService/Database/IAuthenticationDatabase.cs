using Messages.Database;
using Messages.NServiceBus.Commands;
using Messages.ServiceBusRequest;

namespace AuthenticationService.Database
{
    interface IAuthenticationDatabase : IAbstractDatabase
    {
        ServiceBusResponse insertNewUserAccount(CreateAccount accountInfo);

        ServiceBusResponse isValidUserInfo(string username, string password);

    }
}