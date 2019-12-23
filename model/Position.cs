using System;
using System.Collections.Generic;
namespace model
{
    public class Position { 

        private int x;
        private int y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        
        }

       


        public override string ToString()
        {
            return "x: " + x + " y:" + y;

        }


        public int getX() { return x; }
        public int getY() { return y; }

        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   x == position.x &&
                   y == position.y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

       
    }

}