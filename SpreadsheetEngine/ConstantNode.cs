//-----------------------------------------------------------------------
// <copyright file="ConstantNode.cs" company="Hernan Nunez-Ortega">
//     Company copyright tag.
// </copyright>
//-----------------------------------------------------------------------

namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ConstantNode : Node
    {
        public ConstantNode(double number)
        {
            value = number;
        }
    }
}
