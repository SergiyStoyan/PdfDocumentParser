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
        //Sample: "1 | (2 & 3)"
        static public bool Parse(string expression)
        {
            BooleanEngine be = new BooleanEngine();
            expression = Regex.Replace(expression, @"\d+", (Match m) => { return int.Parse(m.Value) <= 4 ? "T" : "F"; });
            be.expression = Regex.Replace(expression, @"\s", "", RegexOptions.Singleline);
            be.move2NextToken();
            return be.parse();
        }
        int position = -1;
        string expression;
        char currentToken;
        bool isEOS = true;

        void move2NextToken()
        {
            position++;
            if (position >= expression.Length)
            {
                isEOS = true;
                return;
            }
            currentToken = expression[position];
            isEOS = false;
        }

        bool parse()
        {
            while (!isEOS)
            {
                var isNegated = currentToken == '!';
                if (isNegated)
                    move2NextToken();

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