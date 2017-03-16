using Microsoft.VisualStudio.TestTools.UnitTesting;
using Voxteneo.Core.Domains.LambdaSqlBuilder.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxteneo.Core.Domains.LambdaSqlBuilder.Adapter.Tests
{
    public class Profile
    {
        public string ProfileDescription { get; set; }
    }
    [TestClass()]
    public class UmbracoQueryAdapterTests
    {
        [TestMethod()]
        public void QueryStringTest()
        {
            const string productName = "Tofu";
            SqlLamBase._defaultAdapter = new UmbracoQueryAdapter();
            var query = new SqlLam<Profile>(p => p.ProfileDescription == productName);
            var sql = query.QueryString;
            Console.WriteLine();
        }
    }
}