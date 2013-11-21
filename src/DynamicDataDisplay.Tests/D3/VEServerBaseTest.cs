//using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace DynamicDataDisplay.Test
//{
//    [TestClass]
//    public class VEServerBaseTest
//    {
//        private TestContext testContextInstance;

//        public TestContext TestContext
//        {
//            get
//            {
//                return testContextInstance;
//            }
//            set
//            {
//                testContextInstance = value;
//            }
//        }

//        public void CreateTileIndexStringTest()
//        {
//            VEServerBase server = new VERoadServer();
//            server.Test(0, 2, 2, "02");

//            server.Test(0, 0, 1, "2");
//            server.Test(1, 0, 1, "3");
//            server.Test(0, 1, 1, "0");
//            server.Test(1, 1, 1, "1");
//            server.Test(0, 0, 2, "22");
//            server.Test(1, 1, 2, "21");
//        }
//    }

//    internal static class VEServerBaseTestHelper
//    {
//        public static void Test(this VEServerBase server, int x, int y, int level, string expectedValue)
//        {
//            var actualValue = server.CallPrivateMethod<string>("CreateTileIndexString", new TileIndex(x, y, level));

//            Assert.AreEqual(expectedValue, actualValue);
//        }
//    }
//}
