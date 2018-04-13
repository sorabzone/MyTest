using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaySlipSimulator;

namespace PaySlipTest
{
    [TestClass]
    public class UnitTestSimulator
    {
        /// <summary>
        /// Positive test case
        /// </summary>
        [TestMethod]
        public void Simuator_Placed_Moved_Turned_Report()
        {
            var simulator = new Simulator();
            string[] commands = {"PLACE 1,2,EAST",
                                "MOVE",
                                "MOVE",
                                "LEFT",
                                "MOVE"};
            simulator.FeedCommands(commands);
            var result = simulator.ExecuteSingleCommand("REPORT");

            Assert.AreEqual("3,3,NORTH", result);
        }

        /// <summary>
        /// Positive test case
        /// </summary>
        [TestMethod]
        public void Simuator_Placed_Moved_Moved_Report()
        {
            var simulator = new Simulator();
            string[] commands = {"PLACE 1,2,EAST",
                                "MOVE",
                                "MOVE",
                                "MOVE",
                                "MOVE",
                                "MOVE",
                                "MOVE",
                                "LEFT",
                                "MOVE"};
            simulator.FeedCommands(commands);
            var result = simulator.ExecuteSingleCommand("REPORT");

            Assert.AreEqual("4,3,NORTH", result);
        }

        /// <summary>
        /// Test with out of bound PLACE command
        /// </summary>
        [TestMethod]
        public void Simuator_Placed_Moved_Invalid_Report()
        {
            var simulator = new Simulator();
            string[] commands = {"PLACE 1,2,EAST",
                                "MOVE",
                                "MOVE",
                                "LEFT",
                                "PLACE 5,7,EAST",
                                "MOVE"};
            simulator.FeedCommands(commands);
            var result = simulator.ExecuteSingleCommand("REPORT");

            Assert.AreEqual("3,3,NORTH", result);
        }

        /// <summary>
        /// Test begin with out of bound PLACE command
        /// </summary>
        [TestMethod]
        public void Simuator_Invalid_Move_Placed_Moved_Turned_Report()
        {
            var simulator = new Simulator();
            string[] commands = {"PLACE 5,7,EAST",
                                "MOVE",
                                "PLACE 1,2,EAST",
                                "MOVE",
                                "MOVE",
                                "LEFT",
                                "MOVE"};
            simulator.FeedCommands(commands);
            var result = simulator.ExecuteSingleCommand("REPORT");

            Assert.AreEqual("3,3,NORTH", result);
        }

        /// <summary>
        /// Test with invalid Place command, so no movement at all
        /// </summary>
        [TestMethod]
        public void Simuator_EmptySpace_Placed_Moved_Turned_Report()
        {
            var simulator = new Simulator();
            string[] commands = {"PLACE 1,2, EAST",
                                "MOVE",
                                "MOVE",
                                "LEFT",
                                "MOVE"};
            simulator.FeedCommands(commands);
            var result = simulator.ExecuteSingleCommand("REPORT");

            Assert.AreNotEqual("3,3,NORTH", result);
            Assert.AreEqual("", result);
        }

        /// <summary>
        /// Test with misspelled Place command, so no movement at all
        /// </summary>
        [TestMethod]
        public void Simuator_Spelling_Placed_Moved_Turned_Report()
        {
            var simulator = new Simulator();
            string[] commands = {"PLACED 1,2,EAST",
                                "MOVE",
                                "MOVE",
                                "LEFT",
                                "MOVE"};
            simulator.FeedCommands(commands);
            var result = simulator.ExecuteSingleCommand("REPORT");

            Assert.AreNotEqual("3,3,NORTH", result);
            Assert.AreEqual("", result);
        }

        /// <summary>
        /// Test with misspelled Move command
        /// </summary>
        [TestMethod]
        public void Simuator_Spelling2_Move_Placed_Moved_Turned_Report()
        {
            var simulator = new Simulator();
            string[] commands = {"PLACE 5,7,EAST",
                                "MOVE",
                                "PLACE 1,2,EAST",
                                "MOVE",
                                "MO VE",
                                "LEFT",
                                "MOVE"};
            simulator.FeedCommands(commands);
            var result = simulator.ExecuteSingleCommand("REPORT");

            Assert.AreEqual("2,3,NORTH", result);
        }

        /// <summary>
        /// Test with out of bound Place command
        /// Invalid Move command with empty space
        /// </summary>
        [TestMethod]
        public void Simuator_EmptySpace2_Move_Placed_Moved_Turned_Report()
        {
            var simulator = new Simulator();
            string[] commands = {"PLACE 5,7,EAST",
                                "MOVE",
                                "PLACE 1,2,EAST",
                                "MOVE ",
                                "MOVE",
                                "LEFT",
                                "MOVE"};
            simulator.FeedCommands(commands);
            var result = simulator.ExecuteSingleCommand("REPORT");

            Assert.AreEqual("2,3,NORTH", result);
        }

        /// <summary>
        /// Test with out of bound Place command and invalid Place command with extra space
        /// No movement at all
        /// </summary>
        [TestMethod]
        public void Simuator_EmptySpace3_Move_Placed_Moved_Turned_Report()
        {
            var simulator = new Simulator();
            string[] commands = {"PLACE 5,7,EAST",
                                "MOVE",
                                "PLACE    1,2,EAST",
                                "MOVE",
                                "MOVE",
                                "LEFT",
                                "MOVE"};
            simulator.FeedCommands(commands);
            var result = simulator.ExecuteSingleCommand("REPORT");

            Assert.AreNotEqual("3,3,NORTH", result);
            Assert.AreEqual("", result);
        }

        /// <summary>
        /// Ignored commands before a valid Place command
        /// </summary>
        [TestMethod]
        public void Simuator_Move_Placed_Moved_Turned_Report()
        {
            var simulator = new Simulator();
            string[] commands = {"MOVE",
                                "PLACE 5,7,EAST",
                                "MOVE",
                                "PLACE 1,2,EAST",
                                "MOVE",
                                "MOVE",
                                "LEFT",
                                "MOVE"};
            simulator.FeedCommands(commands);
            var result = simulator.ExecuteSingleCommand("REPORT");

            Assert.AreEqual("3,3,NORTH", result);
        }

        /// <summary>
        /// Test with out of bound Place command
        /// </summary>
        [TestMethod]
        public void Simuator_Move2_Placed_Moved_Turned_Report()
        {
            var simulator = new Simulator();
            string[] commands = {"PLACE 1,2,EAST",
                                "MOVE",
                                "PLACE 5,2,EAST",
                                "MOVE",
                                "MOVE",
                                "LEFT",
                                "MOVE"};
            simulator.FeedCommands(commands);
            var result = simulator.ExecuteSingleCommand("REPORT");

            Assert.AreEqual("4,3,NORTH", result);
            Assert.AreNotEqual("3,3,NORTH", result);
        }
    }
}
