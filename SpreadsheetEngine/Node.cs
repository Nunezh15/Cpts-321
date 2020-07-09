//-----------------------------------------------------------------------
// <copyright file="Node.cs" company="Hernan Nunez-Ortega">
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
    ///  
    /// </summary>
    public abstract class Node 
    {
        public string name;
        protected double value;

        public Node Left;
        public Node Right;

        public Node root = null;
        public Dictionary<string, double> variableDict = new Dictionary<string, double>();
        bool invalidInput = false;

        public string getName()
        {
            return name;
        }

        public double getValue()
        {
            return value;
        }

        public void setValue(double number)
        {
            value = number;
        }

        /// <summary>
        /// Go through expression and find the next operator.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int LocateOperator(string expression)
        {
            for (int i = 0; i < expression.Length; i++)
            {
                if ((expression[i] == '*') || (expression[i] == '/') || (expression[i] == '+') || (expression[i] == '-'))
                {
                    return i;
                }
            }
            return -1; //No operator.
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Node Compile(string s)
        {
            
            int index = 0;
            int parenthesisCounter = 0;
            char[] ops = ExpressionTree.operators;

            if ((s == "") || (s == null)) 
            {
                return null;
            }

            if (s[0] == '(')
            {
                //going through each character in the string
                for (int i = 0; i < s.Length; i++)
                {
                    // encounter left parenthesis
                    if (s[i] == '(')
                    {
                        parenthesisCounter++;
                    }
                    else if (s[i] == ')')
                    {
                        parenthesisCounter--;
                        if (parenthesisCounter == 0)
                        {
                            if (s.Length - 1 != i)
                            {
                                break;
                            }
                            else
                            {
                                return Compile(s.Substring(1, s.Length - 2));
                            }
                        }
                    }
                }
            }
            
            foreach (char op in ops)
                {
                    Node n = Compile(s, op);
                    if (invalidInput == true)
                    {
                        break;
                    }
                    if (n != null)
                    {
                        return n;
                    }
                }

            double constantNumber;

            if (double.TryParse(s, out constantNumber)) 
            {
                return new ConstantNode(constantNumber);
            }

            variableDict[s] = 0;

            index = LocateOperator(s);
            int constantNum;

            if (index == -1)
            {
                if (int.TryParse(s, out constantNum))
                {
                    return new ConstantNode(constantNum);
                }
                else
                {
                    return new VariableNode(s);
                }
            }

            Left = Compile(s.Substring(0, index));
            Right = Compile(s.Substring(index + 1));

            return new OperatorNode(s[index].ToString(), Left, Right);          
        } // End of compile method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        private Node Compile(string expr, char op)
        {
            int parenthesisCounter = 0;
            bool quit = false;
            bool rightAssociative = false;
            
            int i = 0;
            i = expr.Length - 1; 

            while (!quit)
            {
                if (expr[i] == '(')
                {
                    if (rightAssociative)
                    {
                        parenthesisCounter--;
                    }
                    else
                    {
                        parenthesisCounter++;
                    }
                }
                else if (expr[i] == ')')
                {
                    if (rightAssociative)
                    {
                        parenthesisCounter++;
                    }
                    else
                    {
                        parenthesisCounter--;
                    }
                }

                //Not inside a parenthesis
                if (parenthesisCounter == 0 && expr[i] == op)
                {
                    return new OperatorNode(op.ToString(), Compile(expr.Substring(0, i)), Compile(expr.Substring(i + 1)));
                }

                //right associative 
                if ( rightAssociative)
                {
                    if (i == expr.Length - 1)
                    {
                        quit = true;
                    }
                    i++;
                }

                else
                {
                    if (i == 0)
                    {
                        quit = true;
                    }
                    i--;
                }
            }
            // if parenthesis are not matching up properly
            if (parenthesisCounter != 0)
            {
                Console.WriteLine("Parenthesis are not balanced");
                invalidInput = true;
            }
            return null;
        }//end second compile method
    }
}
