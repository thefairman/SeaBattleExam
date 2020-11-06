using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeaBattleServer.GameLogic;
using System.Diagnostics;

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

#if DEBUG
        [TestMethod]
        public void TestFieldShoot_KillAShip_returnKill()
        {
            SeaBattleField field = new SeaBattleField();
            int[,] fieldSet = { { 1,2,0,0,1,0,0,0,0,0},
                                { 0,0,0,0,1,0,0,1,0,0},
                                { 0,0,1,0,1,0,0,1,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,3,3,1,3,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,1,0,0},
                                { 1,0,0,1,0,0,0,0,0,0},
                                { 1,0,0,0,0,0,0,0,0,0},
                                { 1,0,1,1,0,1,1,0,0,0},};
            field.SetUnsafeMyField(fieldSet);
            var res = field.Shoot(0,0);
            var enemyField = field.GetFieldForEnemy();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Debug.Write((int)enemyField[i, j]);
                }
                Debug.WriteLine("");
            }
            Assert.AreEqual((ShootStatus)res.Object, ShootStatus.Kill);
        }

        [TestMethod]
        public void TestFieldShoot_KillAllShips_returnWin()
        {
            SeaBattleField field = new SeaBattleField();
            int[,] fieldSet = { { 1,2,0,0,4,0,0,0,0,0},
                                { 0,0,0,0,4,0,0,4,0,0},
                                { 0,0,4,0,4,0,0,4,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,4,4,4,4,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,0,0,0},
                                { 0,0,0,0,0,0,0,4,0,0},
                                { 4,0,0,4,0,0,0,0,0,0},
                                { 4,0,0,0,0,0,0,0,0,0},
                                { 4,0,4,4,0,4,4,0,0,0},};
            field.SetUnsafeMyField(fieldSet);
            var res = field.Shoot(0, 0);
            Assert.AreEqual((ShootStatus)res.Object, ShootStatus.Win);
        }
#endif
    }
}
