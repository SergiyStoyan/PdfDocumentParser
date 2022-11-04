//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cliver.PdfDocumentParser
{
    internal class BooleanEngine
    {
        static public IEnumerable<int> GetAnchorIds(string expression)
        {
            if (expression == null)
                return new List<int>();
            List<int> anchorIds = new List<int>();
            foreach (Match m in Regex.Matches(expression, @"\d+"))
                anchorIds.Add(int.Parse(m.Value));
            return anchorIds.Distinct();
        }

        static public string GetFormatted(string expression)
        {
            if (expression == null)
                return null;
            expression = Regex.Replace(expression, @"\s", "", RegexOptions.Singleline);
            return Regex.Replace(expression, @"[\&\|]", " $0 ");
        }

        static public void Check(string expression, IEnumerable<int> anchorIds)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new Exception("Expression is empty.");
            {
                Match m = Regex.Match(expression, @"[^\s\d\(\)\&\|\!TF]");
                if (m.Success)
                    throw new Exception("Expression contains unacceptable symbol: '" + m.Value + "'. Expected symbols: <anchor id>, '!', '&', '|', '(', ')', 'T', 'F'");
            }
            expression = Regex.Replace(expression, @"\d+", (Match m) =>
            {
                int ai = int.Parse(m.Value);
                if (!anchorIds.Contains(ai))
                    throw new Exception("Anchor[id=" + ai + "] does not exist.");
                return "T";
            });
            parseWithSubstitutedAnchorIds(expression);
        }

        //Sample expression: "1 | (2 & 3)"
        static public bool Parse(string expression, Page p)
        {
            expression = Regex.Replace(expression, @"\d+", (Match m) =>
            {
                Page.AnchorActualInfo aai = p.GetAnchorActualInfo(int.Parse(m.Value));
                return aai.Found ? "T" : "F";
            });
            return parseWithSubstitutedAnchorIds(expression);
        }

        static bool parseWithSubstitutedAnchorIds(string expression)
        {
            BooleanEngine be = new BooleanEngine();
            be.expression = Regex.Replace(expression, @"\s", "", RegexOptions.Singleline);
            be.move2NextToken(false);
            return be.parse(true);
        }
        string expression;

        void move2NextToken(bool endOfExpressionIsPossible)
        {
            position++;
            if (position >= expression.Length)
            {
                if (!endOfExpressionIsPossible)
                    throw new Exception("Unexpected end of expression");
                currentToken = '_';
                return;
            }
            currentToken = expression[position];
        }
        int position = -1;
        char currentToken;
        bool isEndOfExpression { get { return currentToken == '_'; } }

        private bool parse(bool globalScope)
        {
            bool? value = null;
            char operand = ' ';
            while ((globalScope && !isEndOfExpression) || (!globalScope && currentToken != ')'))
            {
                if (value != null)
                {
                    if (currentToken == '&' || currentToken == '|')
                        operand = currentToken;
                    else
                    {
                        if (globalScope)
                            throw new Exception("End of expression or operand '&' or '|' is expected instead of '" + currentToken + "'");
                        throw new Exception("Closing parenthesis or operand '&' or '|' is expected instead of '" + currentToken + "'");
                    }
                    move2NextToken(false);
                }

                bool isNegated = false;
                while (currentToken == '!')
                {
                    isNegated = !isNegated;
                    move2NextToken(false);
                }

                bool value2;
                if (currentToken == 'T' || currentToken == 'F')
                    value2 = currentToken == 'T';
                else if (currentToken == '(')
                {
                    move2NextToken(false);
                    value2 = parse(false);
                    if (currentToken != ')')
                        throw new Exception("Closing parenthesis is expected instead of '" + currentToken + "'");
                }
                else
                {
                    if (isEndOfExpression)
                        throw new Exception("Unexpected end of expression.");
                    throw new Exception("Unexpected symbol: '" + currentToken + "'");
                }
                if (isNegated)
                    value2 = !value2;

                if (value == null)
                    value = value2;
                else
                {
                    if (operand == '&')
                        value = (bool)value && value2;
                    else
                        value = (bool)value || value2;
                }

                move2NextToken(true);
            }
            if (value == null)
                if (globalScope)
                    throw new Exception("Expression is empty.");
                else
                    throw new Exception("Some parentheses are empty.");
            return (bool)value;
        }
    }
}