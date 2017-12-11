using Messages.NServiceBus.Commands;
using Messages.ServiceBusRequest;

namespace AuthenticationService.Database
{
    interface IAuthenticationDatabase
    {
        ServiceBusResponse insertNewUserAccount(CreateAccount accountInfo);

        ServiceBusResponse isValidUserInfo(string username, string password);

    }
}
