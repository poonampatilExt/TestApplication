using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Travel.Connectors.Hotel.Controllers;
using Xunit;

namespace Travel.Connectors.Hotel.Tests
{
    public class Class
    {
        [Fact]
        public void TestGetMethod()
        {
            ValuesController valuesController = new ValuesController();
            IEnumerable<string> result = valuesController.Get("test");

            Assert.Equal(result.Count(), 2);
        }
    }
}
