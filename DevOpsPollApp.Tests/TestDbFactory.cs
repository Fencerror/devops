using DevOpsPollApp.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DevOpsPollApp.Tests
{
    public sealed class TestDbFactory
    {
        public SqliteConnection Connection { get; }
        public ApplicationDbContext Db { get; }

        public TestDbFactory()
        {
            Connection = new SqliteConnection("Data Source=:memory:;Cache=Shared");
            Connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(Connection)
                .Options;

            Db = new ApplicationDbContext(options);
            Db.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Db.Dispose();
            Connection.Dispose();
        }
    }
}
