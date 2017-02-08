using System;
using System.Collections.Generic;
using System.Net;
using HueLib2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using HueLib2;

namespace HueLib2Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string data = "[\n  {\n    \"error\": {\n      \"type\": 6,\n      \"address\": \"/groups/9/bri\",\n      \"description\": \"parameter, bri, not available\"\n    }\n  },\n  {\n    \"error\": {\n      \"type\": 6,\n      \"address\": \"/groups/9/sat\",\n      \"description\": \"parameter, sat, not available\"\n    }\n  }\n]";
            List<Error> el = Serializer.DeserializeToObject<List<Error>>(data);

        }
    }
}
