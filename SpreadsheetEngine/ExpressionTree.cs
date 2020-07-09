//-----------------------------------------------------------------------
// <copyright file="ExpressionTree.cs" company="Hernan Nunez-Ortega">
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

    /// <summary>
    /// Implements an arithmetic expression parser that builds a tree for the expression.
    /// </summary>
    public class ExpressionTree : Node
    {
        /// <summary>
        /// array holding operators
        /// </summary>
        public readonly static char[] operators = { '+', '-', '*', '/' };
        private string _exprString;
        private double _result;
        

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadsheetEngine.ExpressionTree"/> class.  
        /// </summary>
        /// <param name="expression"> a string of and expression </param>
        public ExpressionTree(string expression)
        {
            // Getting rid of spaces from the expression
            expression = expression.Replace(" ", string.Empty);
            this.variableDict = new Dictionary<string, double>();
            this.root = Compile(expression);
        }

        /// <summary>
        /// sets the specified variable within the ExpressionTree variables dictionary 
        /// </summary>
        /// <param name="variableName"> string for the variable name </param>
        /// <param name="variableValue"> double variable value </param>
        public void SetVariable(string variableName, double variableValue)
        {
            this.variableDict[variableName] = variableValue;
        }

        /// <summary>
        /// places the keys of the dictionary into an array
        /// </summary>
        /// <returns> an array of keys </returns>
        public string[] GetAllVariables()
        {
            return variableDict.Keys.ToArray();
        }

        public double Evaluate()
        {
            return this.Evaluate(this.root);
        }

        /// <summary>
        /// Evaluates the expression based on the operator
        /// </summary>
        /// <param name="n"> a node </param>
        /// <returns> returns the result of the expression </returns>
        private double Evaluate(Node n)
        {
            if (n == null)
            {
                return -1;
            }

            ConstantNode constantN = n as ConstantNode;
            if (constantN != null)
            {
                return constantN.getValue();
            }

            VariableNode variableN = n as VariableNode;
            if (variableN != null)
            {
                return variableDict[n.getName()];
            }

            OperatorNode opN = n as OperatorNode;
            if (opN != null)
            {
                if (opN.getName() == "+")
                {
                    return this.Evaluate(opN.Left) + this.Evaluate(opN.Right);
                }

                else if (opN.getName() == "-")
                {
                    return this.Evaluate(opN.Left) - this.Evaluate(opN.Right);
                }

                else if (opN.getName() == "*")
                {
                    return this.Evaluate(opN.Left) * this.Evaluate(opN.Right);
                }

                else if (opN.getName() == "/")
                {
                    return this.Evaluate(opN.Left) / this.Evaluate(opN.Right);
                }
            }

            return 0.0;
         } // end of evaluate method
    }
}
