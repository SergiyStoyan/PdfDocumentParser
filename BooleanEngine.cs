using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Cliver.PdfDocumentParser
{
    public class BooleanEngine
    {
        static public List<int> GetAnchorIds(string expression)
        {
            List<int> anchorIds = new List<int>();
            foreach (Match m in Regex.Matches(expression, @"\d+"))
                anchorIds.Add(int.Parse(m.Value));
            return anchorIds;
        }

        static public string CheckAndFormat(string expression, IEnumerable<int> anchorIds)
        {
            expression = Regex.Replace(expression, @"\d+", (Match m) =>
            { int ai = int.Parse(m.Value);
                if (!anchorIds.Contains(ai))
                    throw new Exception("Anchor[id=" + ai + "] does not exist.");
                return "T";
            });
            parseSubstituted(expression);
            expression = Regex.Replace(expression, @"\s", "", RegexOptions.Singleline);
            return Regex.Replace(expression, @"[^\d]|\d+", " ", RegexOptions.Singleline);
        }

        //Sample expression: "1 | (2 & 3)"
        static public bool Parse(string expression, Page p)
        {
            expression = Regex.Replace(expression, @"\d+", (Match m) =>
            {
                return p.GetAnchorPoint0(int.Parse(m.Value)) != null ? "T" : "F";
            });
            return parseSubstituted(expression);
        }

        static bool parseSubstituted(string expression)
        {
            BooleanEngine be = new BooleanEngine();
            be.expression = Regex.Replace(expression, @"\s", "", RegexOptions.Singleline);
            if (Regex.IsMatch(be.expression, @"[^TF\(\)\&\|\!]", RegexOptions.IgnoreCase))
                throw new Exception("Expression contains unacceptable symbols.");
            be.move2NextToken();
            bool r = be.parse();
            if(!be.isEOS)
                throw new Exception("Expression could not be parsed to the end.");
            return r;
        }
        int position = -1;
        string expression;
        char currentToken;
        bool isEOS { get { return currentToken == '_'; } }

        void move2NextToken()
        {
            position++;
            if (position >= expression.Length)
            {
                currentToken = '_';
                return;
            }
            currentToken = expression[position];
        }

        bool parse()
        {
            while (!isEOS)
            {
                var isNegated = false;
                while (currentToken == '!')
                {
                    isNegated = !isNegated;
                    move2NextToken();
                }

                var boolean = parseBoolean();
                if (isNegated)
                    boolean = !boolean;

                while (currentToken == '|' || currentToken == '&')
                {
                    var operand = currentToken;
                    move2NextToken();
                    if (isEOS)
                        throw new Exception("Missing expression after operand");
                    var nextBoolean = parseBoolean();

                    if (operand == '&')
                        boolean = boolean && nextBoolean;
                    else
                        boolean = boolean || nextBoolean;
                }
                return boolean;
            }
            throw new Exception("Empty expression");
        }

        private bool parseBoolean()
        {
            if (currentToken == 'T' || currentToken == 'F')
            {
                var current = currentToken;
                move2NextToken();
                return current == 'T';
            }
            if (currentToken == '(')
            {
                move2NextToken();
                var expInPars = parse();
                if (currentToken != ')')
                    throw new Exception("Closing parenthesis expected.");
                move2NextToken();
                return expInPars;
            }
            if (currentToken == ')')
                throw new Exception("Unexpected closing parenthesis.");

            // since its not a BooleanConstant or Expression in parenthesis, it must be a expression again
            var val = parse();
            return val;
        }
    }
}