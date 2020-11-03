using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattleServer.GameLogic
{
    public class GameAnswer
    {
        public object Object { get; }
        public bool Error { get; }
        public GameAnswer(object @object, bool error)
        {
            Object = @object;
            Error = error;
        }
    }
}
