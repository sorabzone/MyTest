using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaySlipEngine;
using PaySlipEngine.Common;

namespace PaySlipTest
{
    [TestClass]
    public class UnitTestPaySlip
    {
        /// <summary>
        /// Test without valid Place command
        /// </summary>
        [TestMethod]
        public void PaySlip_Not_Placed_And_Moved()
        {
            var paySlip = new PaySlipEngine.NSWPaySlipEngine();
            paySlip.Move();
            var result = paySlip.Report();
            Assert.IsTrue(string.IsNullOrEmpty(result));
        }

        /// <summary>
        /// Test with out of bound Place command
        /// </summary>
        [TestMethod]
        public void PaySlip_PlacedOutside()
        {
            var paySlip = new PaySlipEngine.NSWPaySlipEngine();
            paySlip.Place(2, 5, Direction.EAST);
            paySlip.Move();
            var result = paySlip.Report();
            Assert.IsTrue(string.IsNullOrEmpty(result));
        }

        /// <summary>
        /// Test with 2 Place commands
        /// </summary>
        [TestMethod]
        public void PaySlip_PlacedInside_PlacedOutside()
        {
            var paySlip = new PaySlipEngine.NSWPaySlipEngine();
            paySlip.Place(2, 2, Direction.EAST);
            paySlip.Place(6, 2, Direction.EAST);
            var result = paySlip.Report();
            Assert.AreEqual("2,2,EAST", paySlip.Report());
        }

        /// <summary>
        /// Positive test case
        /// </summary>
        [TestMethod]
        public void PaySlip_Placed_Moved_Turned_Report()
        {
            var paySlip = new PaySlipEngine.NSWPaySlipEngine();
            paySlip.Place(1, 2, Direction.EAST);
            paySlip.Move();
            paySlip.Move();
            paySlip.Left();
            paySlip.Move();
            Assert.AreEqual("3,3,NORTH", paySlip.Report());
        }
    }
}
