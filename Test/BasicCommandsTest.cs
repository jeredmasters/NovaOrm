using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using NovaOrm;
using System.Data;

namespace Test
{
    [TestClass]
    public class BasicCommandsTest
    {
        NovaDb _sut;

        string _testTable = "TestTable";

        [TestInitialize]
        public void Init()
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.InitialCatalog = "TestDB";
            csb.Password = "password";
            csb.UserID = "sa";
            csb.DataSource = "localhost";

            _sut = new NovaDb(csb.ToString());
        }

        [TestMethod]
        public void CreateTable_Test()
        {
            if (!_sut.TableExists(_testTable))
            {
                var query = _sut.Create(_testTable).Column("col1", "integer", false);
                query.Execute();
                var result = _sut.TableExists(_testTable);


                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void Instert_Test()
        {
            _sut.Insert(_testTable).Column("col1", 5).Execute();

            var query = _sut.Select(_testTable).First();

            Assert.IsTrue((int)query["col1"] == 5);

            _sut.Insert(_testTable).Column("col1", 4).Execute();
        }

        [TestMethod]
        public void DeleteRow_Test()
        {
            _sut.Insert(_testTable).Column("col1", 6).Execute();

            var command = _sut.Delete(_testTable).Where("col1", 6);
            int count1 = command.Count();

            command.Execute();

            int count2 = command.Count();

            Assert.IsTrue(count1 > count2);
        }

        [TestMethod]
        public void DropTable_Test()
        {
            var command = _sut.Drop(_testTable);
            command.Execute();
        }
    }
}
