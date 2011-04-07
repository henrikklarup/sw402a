using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    class Visitor
    {
        public object visitMainBlock(Mainblock block, object arg)
        {
            visitBlock(block.block, null);
            return null;
        }

        public object visitBlock(Block block, object arg)
        {
            foreach (Command c in block.commands)
            { 
                c.visit(this, null);
            }
            return null;
        }
    }
}
