using System.Threading.Tasks;
using LeagueHistory.Core;
using LeagueHistory.Core.Architecture;
using LeagueHistory.Core.Architecture.Implementations;
using NUnit.Framework;

namespace LeagueHistory.Tests
{
    public class AuthenticationTest
    {
        [Test]
        public async Task Authenticate()
        {
            var auth = new LeagueAuthenticator();
            var account = new LeagueAccount(new LeagueCredentials("username", "password"));
            var result = await auth.Authenticate(account);
            Assert.AreEqual(Result.Invalid,result);
        }
    }
}