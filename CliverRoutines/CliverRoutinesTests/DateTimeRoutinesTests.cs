using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Cliver;

namespace CliverRoutinesTests
{
    [TestClass]
    public class DateTimeRoutinesTests
    {
        class DTTest
        {
            public string Test;
            public DateTime Answer;
            public DTTest(string test, DateTime answer)
            {
                Test = test;
                Answer = answer;
            }
        }

        [TestMethod]
        public void TestDate()
        {
            foreach (DTTest test in get_tests1())
            {
                DateTimeRoutines.ParsedDateTime t;
                if (test.Test.TryParseDate(DateTimeRoutines.DateTimeFormat.USA_DATE, out t))
                    Assert.AreEqual(test.Answer.Date, t.DateTime.Date);
                else
                {
                    if (DateTime.Now.Year != test.Answer.Year || DateTime.Now.Month != test.Answer.Month || DateTime.Now.Day != test.Answer.Day)
                        Assert.Fail("Date not found in " + test.Test);
                }
            }
        }

        [TestMethod]
        public void TestDateTime()
        {
            foreach (DTTest test in get_tests2())
            {
                DateTimeRoutines.ParsedDateTime t;
                if (test.Test.TryParseDateOrTime(DateTimeRoutines.DateTimeFormat.USA_DATE, out t))
                {
                    if (
                        (!t.IsUtcOffsetFound || test.Answer.Kind != DateTimeKind.Utc || t.UtcDateTime != test.Answer) 
                        && (t.IsUtcOffsetFound || t.DateTime != test.Answer)
                        )
                        Assert.Fail(t.DateTime.ToString() + " <> " + test.Answer.ToString() + " and " + t.UtcDateTime.ToString() + " <> " + test.Answer.ToString());
                }
                else
                    Assert.Fail("DateTime not found in " + test.Test);
            }
        }

        [TestMethod]
        public void TestTime()
        {
            foreach (DTTest test in get_tests2())
            {
                DateTimeRoutines.ParsedDateTime t;
                if (test.Test.TryParseDateOrTime(DateTimeRoutines.DateTimeFormat.USA_DATE, out t))
                {
                    if (
                        (!t.IsUtcOffsetFound || test.Answer.Kind != DateTimeKind.Utc || t.UtcDateTime.TimeOfDay != test.Answer.TimeOfDay)
                        && (t.IsUtcOffsetFound || t.DateTime.TimeOfDay != test.Answer.TimeOfDay)
                        )
                        Assert.Fail(t.DateTime.TimeOfDay.ToString() + " <> " + test.Answer.TimeOfDay.ToString() + " and " + t.UtcDateTime.TimeOfDay.ToString() + " <> " + test.Answer.TimeOfDay.ToString());
                }
                else
                    if (0 != test.Answer.Hour || 0 != test.Answer.Minute || 0 != test.Answer.Second)
                    Assert.Fail("Time not found in " + test.Test);
            }
        }

        List<DTTest> get_tests2()
        {
            List<DTTest> tests = get_tests1();
            tests.Add(new DTTest(@"Your program recognizes string : 21 Jun 2010 04:20:19 -0430 blah blah", new DateTime(2010, 6, 20, 23, 50, 19, DateTimeKind.Utc)));
            tests.Add(new DTTest(@"Your program recognizes string : 21 Jun 2010 04:20:19 UTC blah blah", new DateTime(2010, 6, 21, 4, 20, 19, DateTimeKind.Utc)));
            tests.Add(new DTTest(@"Your program recognizes string : 21 Jun 2010 04:20:19 EST blah blah", new DateTime(2010, 6, 20, 23, 20, 19, DateTimeKind.Utc)));
            return tests;
        }

        List<DTTest> get_tests1()
        {
            List<DTTest> tests = new List<DTTest>();
            tests.Add(new DTTest(@"Member since:  	10-Feb-2008", new DateTime(2008, 2, 10, 0, 0, 0)));
            tests.Add(new DTTest(@"Last Update: 18:16 11 Feb '08 ", new DateTime(2008, 2, 11, 18, 16, 0)));
            tests.Add(new DTTest(@"date	Tue, Feb 10, 2008 at 11:06 AM", new DateTime(2008, 2, 10, 11, 06, 0)));
            tests.Add(new DTTest(@"see at 12/31/2007 14:16:32", new DateTime(2007, 12, 31, 14, 16, 32)));
            tests.Add(new DTTest(@"sack finish 14:16:32 November 15 2008, 1-144 app", new DateTime(2008, 11, 15, 14, 16, 32)));
            tests.Add(new DTTest(@"Genesis Message - Wed 04 Feb 08 - 19:40", new DateTime(2008, 2, 4, 19, 40, 0)));
            tests.Add(new DTTest(@"The day 07/31/07 14:16:32 is ", new DateTime(2007, 7, 31, 14, 16, 32)));
            tests.Add(new DTTest(@"Shipping is on us until December 24, 2008 within the U.S. ", new DateTime(2008, 12, 24, 0, 0, 0)));
            tests.Add(new DTTest(@" 2008 within the U.S. at 14:16:32", new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 16, 32)));
            tests.Add(new DTTest(@"5th November, 1994, 8:15:30 pm", new DateTime(1994, 11, 5, 20, 15, 30)));
            tests.Add(new DTTest(@"7 boxes January 31 , 14:16:32.", new DateTime(DateTime.Now.Year, 1, 31, 14, 16, 32)));
            tests.Add(new DTTest(@"the blue sky of Sept  30th  2008 14:16:32", new DateTime(2008, 9, 30, 14, 16, 32)));
            tests.Add(new DTTest(@" e.g. 1997-07-16T19:20:30+01:00", new DateTime(1997, 7, 16, 19, 20, 30)));
            tests.Add(new DTTest(@"Apr 1st, 2008 14:16:32 tufa 6767", new DateTime(2008, 4, 1, 14, 16, 32)));
            tests.Add(new DTTest(@"wait for 07/31/07 14:16:32", new DateTime(2007, 7, 31, 14, 16, 32)));
            tests.Add(new DTTest(@"later 12.31.08 and before 1.01.09", new DateTime(2008, 12, 31, 0, 0, 0)));
            tests.Add(new DTTest(@"Expires: Sept  30th  2008 14:16:32", new DateTime(2008, 9, 30, 14, 16, 32)));
            tests.Add(new DTTest(@"Offer expires Apr 1st, 2007, 14:16:32", new DateTime(2007, 4, 1, 14, 16, 32)));
            tests.Add(new DTTest(@"Expires  14:16:32 January 31.", new DateTime(DateTime.Now.Year, 1, 31, 14, 16, 32)));
            tests.Add(new DTTest(@"Expires  14:16:32 January 31-st.", new DateTime(DateTime.Now.Year, 1, 31, 14, 16, 32)));
            tests.Add(new DTTest(@"Expires 23rd January 2010.", new DateTime(2010, 1, 23, 0, 0, 0)));
            tests.Add(new DTTest(@"Expires January 22nd, 2010.", new DateTime(2010, 1, 22, 0, 0, 0)));
            tests.Add(new DTTest(@"Expires DEC 22, 2010.", new DateTime(2010, 12, 22, 0, 0, 0)));
            tests.Add(new DTTest(@"Version: 1.0.0.692 6/1/2010 2:28:04 AM ", new DateTime(2010, 6, 1, 2, 28, 4)));
            tests.Add(new DTTest(@"Version: 1.0.0.692 04/21/11 12:30am ", new DateTime(2011, 4, 21, 00, 30, 00)));
            tests.Add(new DTTest(@"Version: 1.0.0.692 04/21/11 12:30pm ", new DateTime(2011, 4, 21, 12, 30, 00)));
            tests.Add(new DTTest(@"Version: Thu Aug 06 22:32:15 MDT 2009 ", new DateTime(2009, 8, 6, 22, 32, 15)));
            return tests;
        }
    }
}
