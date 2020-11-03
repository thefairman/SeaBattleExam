using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeaBattleServer.GameLogic;

namespace GameLogic.Test
{
    [TestClass]
    public class TestGameField
    {
        [TestMethod]
        public void TestFieldSet_hasCollision_returnError()
        {
            SeaBattleField field = new SeaBattleField();
            int[,] fieldSet = { { 0,0,0,0,1,0,0,0,0,0},
                                { 0,0,0,0,1,0,0,0,0,0},
                                { 0,0,0,0,1,0,0,0,0,0},
                                { 0,0,0,0,0,1,0,0,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},};
            var res = field.SetField(fieldSet);
            Assert.IsTrue(res.Error);
        }

        [TestMethod]
        public void TestFieldSet_hasWrongWhipsCount_returnError()
        {
            SeaBattleField field = new SeaBattleField();
            int[,] fieldSet = { { 0,0,0,0,1,0,0,0,0,0},
                                { 0,0,0,0,1,0,0,0,0,1},
                                { 0,0,0,0,1,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,1,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 1,0,0,0,0,0,0,0,0,0},};
            var res = field.SetField(fieldSet);
            Assert.IsTrue(res.Error);
        }

        [TestMethod]
        public void TestFieldSet_EverythingOk_returnOk()
        {
            SeaBattleField field = new SeaBattleField();
            int[,] fieldSet = { { 1,0,0,0,1,0,0,0,0,0},
                                { 0,0,0,0,1,0,0,1,0,0},
                                { 0,0,1,0,1,0,0,1,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,1,1,1,1,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,1,0,0},
                                { 1,0,0,1,0,0,0,0,0,0},
                                { 1,0,0,0,0,0,0,0,0,0},
                                { 1,0,1,1,0,1,1,0,0,0},};
            var res = field.SetField(fieldSet);
            Assert.IsFalse(res.Error);
        }
    }
}
