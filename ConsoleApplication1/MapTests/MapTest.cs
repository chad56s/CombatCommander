using CombatCommander;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace MapTests
{
    
    
    /// <summary>
    ///This is a test class for MapTest and is intended
    ///to contain all MapTest Unit Tests
    ///</summary>
	[TestClass()]
	public class MapTest {


		private TestContext testContextInstance;

		private static Map myMap;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext {
			get {
				return testContextInstance;
			}
			set {
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			MapTest.myMap = new Map();

			XDocument map1 = XDocument.Load("C:\\users\\a3skszz\\Documents\\Chad\\CCE\\map1.xml");

			MapTest.myMap.Load(map1);
		}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{

		//	this.myMap = new Map();

		//	XDocument map1 = XDocument.Load("C:\\users\\a3skszz\\Documents\\Chad\\CCE\\map1.xml");

		//	this.myMap.Load(map1);
		//}
		
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///A test for Map Constructor
		///</summary>
		[TestMethod()]
		public void MapConstructorTest() {
			
			Assert.IsInstanceOfType(myMap, typeof(Map));
		}

		/// <summary>
		///A test for GetHex
		///</summary>
		[TestMethod()]
		public void GetHexTest() {
			Map target = myMap;

			int col = 0; // TODO: Initialize to an appropriate value
			int row = 0; // TODO: Initialize to an appropriate value
			Map.Hex actual;
			actual = target.GetHex(col, row);
			
			Assert.AreEqual("A1", actual.Name);
			Assert.AreEqual(TERRAIN.WOODS, actual.Terrain);
		}

		/// <summary>
		///A test for GetHex
		///</summary>
		[TestMethod()]
		public void GetHexTest1() {
			Map target = myMap;
			Map.Hex h = myMap.GetHex(3,5);
			Map.Hex expected, actual;
			COMPASS direction;

			direction = COMPASS.NORTH; // TODO: Initialize to an appropriate value
			expected = myMap.GetHex(3,4); // TODO: Initialize to an appropriate value
			actual = target.GetHex(h, direction);
			Assert.AreEqual(expected, actual);

			direction = COMPASS.NORTHEAST; // TODO: Initialize to an appropriate value
			expected = myMap.GetHex(4,5); // TODO: Initialize to an appropriate value
			actual = target.GetHex(h, direction);
			Assert.AreEqual(expected, actual);

			direction = COMPASS.SOUTHEAST; // TODO: Initialize to an appropriate value
			expected = myMap.GetHex(4,6); // TODO: Initialize to an appropriate value
			actual = target.GetHex(h, direction);
			Assert.AreEqual(expected, actual);


			direction = COMPASS.SOUTH; // TODO: Initialize to an appropriate value
			expected = myMap.GetHex(3,6); // TODO: Initialize to an appropriate value
			actual = target.GetHex(h, direction);
			Assert.AreEqual(expected, actual);


			direction = COMPASS.SOUTHWEST; // TODO: Initialize to an appropriate value
			expected = myMap.GetHex(2,6); // TODO: Initialize to an appropriate value
			actual = target.GetHex(h, direction);
			Assert.AreEqual(expected, actual);


			direction = COMPASS.NORTHWEST; // TODO: Initialize to an appropriate value
			expected = myMap.GetHex(2,5); // TODO: Initialize to an appropriate value
			actual = target.GetHex(h, direction);
			Assert.AreEqual(expected, actual);


		}

		/// <summary>
		///A test for GetHex
		///</summary>
		[TestMethod()]
		public void GetHexTest2() {
			Map target = myMap; // TODO: Initialize to an appropriate value
			string name = "N7"; // TODO: Initialize to an appropriate value
			Map.Hex expected = myMap.GetHex(13,6); // TODO: Initialize to an appropriate value
			Map.Hex actual;
			actual = target.GetHex(name);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Load
		///</summary>
		[TestMethod()]
		public void LoadTest() {
			Map target = new Map(); // TODO: Initialize to an appropriate value
			XDocument xml = null; // TODO: Initialize to an appropriate value
			target.Load(xml);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for SetFeatures
		///</summary>
		[TestMethod()]
		[DeploymentItem("ConsoleApplication1.exe")]
		public void SetFeaturesTest() {
			Map_Accessor target = new Map_Accessor(); // TODO: Initialize to an appropriate value
			Map.Hex h = null; // TODO: Initialize to an appropriate value
			string f = string.Empty; // TODO: Initialize to an appropriate value
			XElement hNode = null; // TODO: Initialize to an appropriate value
			target.SetFeatures(h, f, hNode);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Distance
		///</summary>
		[TestMethod()]
		public void DistanceTest() {
			Map target = MapTest.myMap; // TODO: Initialize to an appropriate value
			Map.Hex h1 = target.GetHex("O10"); // TODO: Initialize to an appropriate value
			Map.Hex h2 = target.GetHex("F1"); // TODO: Initialize to an appropriate value
			int expected = 13; // TODO: Initialize to an appropriate value
			int actual;
			actual = Map.Distance(h1, h2);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for SetFeatures
		///</summary>
		[TestMethod()]
		[DeploymentItem("ConsoleApplication1.exe")]
		public void SetFeaturesTest1() {
			Map_Accessor target = new Map_Accessor(); // TODO: Initialize to an appropriate value
			Map.Hex h = null; // TODO: Initialize to an appropriate value
			string f = string.Empty; // TODO: Initialize to an appropriate value
			XElement hNode = null; // TODO: Initialize to an appropriate value
			target.SetFeatures(h, f, hNode);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}
	}
}
