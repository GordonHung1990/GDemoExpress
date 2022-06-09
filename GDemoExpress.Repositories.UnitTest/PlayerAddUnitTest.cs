using GDemoExpress.Core;
using GDemoExpress.Core.Models;
using GDemoExpress.DataBase.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NETCore.Encrypt.Extensions;
using NSubstitute;

namespace GDemoExpress.Repositories.UnitTest
{
    [TestClass]
    public class PlayerAddUnitTest
    {
        [TestMethod]
        public async Task Player_Added_Successfully()
        {
            var fakeLogger = NullLogger<PlayerRepository>.Instance;
            var options = new DbContextOptionsBuilder<DboContext>()
               .UseInMemoryDatabase(databaseName: "dbo")
               .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
               .Options;
            var context = new DboContext(options);
            _ = context.Database.EnsureDeleted();

            string account = "gordon";
            string password = "aa123456";
            string nickName = "Gordon";


            var demoExpressOptions = new DemoExpressOptions()
            {
                AdminUser = "admin",
                AdminPassword = "admin123456",
                AdminCryptographyKey = "QWERTasdfgZXCVB",
                PlayerPassword = "1qaz2wsx",
                PlayerCryptographyKey = "qwertASDFGzxcvb"
            };

            var repository = new PlayerRepository(
                context,
                fakeLogger,
                Options.Create(demoExpressOptions));

            var data = new PlayerAdd(
                 Account: account,
                 Password: password,
                 NickName: nickName);

            var playeId = await repository.AddAsync(data).ConfigureAwait(false);

            var actual = await repository.GetAsync(playeId).ConfigureAwait(false);
            Assert.IsNotNull(actual);
            Assert.AreEqual(account, actual.Account);
            Assert.AreEqual(nickName, actual.NickName);
        }
    }
}