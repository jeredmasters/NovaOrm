using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using NovaOrm;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Test
{
    [TestClass]
    public class ResultTest
    {
        INovaDb _db;

        string _testTable = "TestTable3";

        INovaTable _sut;

        [TestInitialize]
        public void Init()
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.InitialCatalog = "TestDB";
            csb.Password = "password";
            csb.UserID = "sa";
            csb.DataSource = "localhost";

            _db = new NovaDb(csb.ToString());

            if (_db.TableExists(_testTable))
            {
                _db.Drop(_testTable).Execute();
            }

            _db.Create(_testTable)
                    .Column("id", "int IDENTITY(1,1)")
                    .Column("col1", "integer", false)
                    .Column("col2", "nvarchar(255)", false)
                    .Execute();

            _sut = _db.Table(_testTable, "id");
        }

        [TestMethod]
        public void CreateTable_Test()
        {
            var result = _db.TableExists(_testTable);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ToJson_Test()
        {
            _sut.Truncate().Execute();
            INovaResult result = _sut.Select().Result();

            string json = result.ToJson();

            Assert.IsTrue(json.Trim() == "[  ]");

            var entity = _sut.New();

            entity["col1"] = 3;
            entity["col2"] = "qwerty";

            entity.Save();

            _sut.Insert()
                .Column("col1", 7)
                .Column("col2", "asdfqwer")
                .Execute();

            json = _sut.Select().Order("col1").Result().ToJson();

            string json_shouldBe = "[ { \"id\" : 1,\"col1\" : 3,\"col2\" : \"qwerty\" }, { \"id\" : 2,\"col1\" : 7,\"col2\" : \"asdfqwer\" } ]";
            var jObj = JArray.Parse(json);
            var token = jObj.First;
            var col1 = token["col1"].Value<int>();
            var col2 = token["col2"].Value<string>();

            Assert.IsTrue(col1 == 3);
            Assert.IsTrue(col2 == "qwerty");
        }

        [TestMethod]
        public void ToCsv_Test()
        {
            _sut.Truncate().Execute();
            INovaResult result = _sut.Select().Result();

            string csv = result.ToCsv();

            Assert.IsTrue(csv.Trim() == "id,col1,col2");

            var entity = _sut.New();

            entity["col1"] = 3;
            entity["col2"] = "qwerty";

            entity.Save();

            _sut.Insert()
                .Column("col1", 7)
                .Column("col2", "asdfqwer")
                .Execute();

            csv = _sut.Select().Order("col1").Result().ToCsv();

            string[] parts = csv.Split('\n');

            Assert.IsTrue(parts.Count() > 1);

            string[] cells = parts[1].Split(',');

            Assert.IsTrue(cells.Count() > 0);

            Assert.IsTrue(cells[1] == "3");

            Assert.IsTrue(cells[2] == "\"qwerty\"");
        }
    }
}
