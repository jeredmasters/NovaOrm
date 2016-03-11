using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using NovaOrm;
using System.Data;

namespace Test
{
    [TestClass]
    public class EntityTest
    {
        INovaDb _sut;

        string _testTable = "TestTable2";

        INovaTable _table;

        [TestInitialize]
        public void Init()
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.InitialCatalog = "TestDB";
            csb.Password = "password";
            csb.UserID = "sa";
            csb.DataSource = "localhost";

            _sut = new NovaDb(csb.ToString());

            if (_sut.TableExists(_testTable))
            {
                _sut.Drop(_testTable).Execute();
            }

            _sut.Create(_testTable)
                    .Column("id", "int IDENTITY(1,1)")
                    .Column("col1", "integer", false)
                    .Column("col2", "nvarchar(255)", false)
                    .Execute();

            _table = _sut.Table(_testTable, "id");
        }

        [TestMethod]
        public void CreateTable_Test()
        {
            var result = _sut.TableExists(_testTable);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CreateEntity_Test()
        {
            int testInt = 6;
            string testString = "test string";

            NovaEntity entity = _table.New();
            entity["col1"] = testInt;
            entity["col2"] = testString;

            object id = entity.Save();

            NovaEntity newentity = _table.Find(id);

            Assert.IsTrue(newentity["col1"].GetType() == typeof(int));
            Assert.IsTrue(testInt == (int)newentity["col1"]);

            Assert.IsTrue(newentity["col2"].GetType() == typeof(string));
            Assert.IsTrue(testString == (string)newentity["col2"]);
        }

        [TestMethod]
        public void DeleteEntity_Test()
        {
            NovaEntity entity = _table.New();
            entity["col1"] = 5;
            entity["col2"] = "five";

            object id = entity.Save();

            Assert.IsTrue(_table.Find(id) != null);

            entity.Delete();

            Assert.IsTrue(_table.Find(id) == null);
        }

    }
}
