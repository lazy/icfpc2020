﻿using System;
using System.Linq;
using Xunit;
using app;
using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Helpers;

namespace Test
{
    public class InterpreterTests
    {
        [Fact]
        public void TestMul()
        {
            EvalAssert("8", "ap ap mul 4 2");
            EvalAssert("12", "ap ap mul 3 4");
            EvalAssert("-6", "ap ap mul 3 -2");
        }

        [Fact]
        public void TestList()
        {
            EvalAssert("nil", "( )");
            EvalAssert("(0 nil)", "( 0 )");
            EvalAssert("(0 (1 nil))", "( 0 , 1 )");
            EvalAssert("(0 (1 (2 nil)))", "( 0 , 1 , 2 )");
            EvalAssert("(0 (1 (2 (5 nil))))","( 0 , 1 , 2 , 5 )");
        }

        private void EvalAssert(string expected, string program)
        {
            var env = new Env();
            var result = env.Eval(program);
            Assert.Equal(expected, result.ToString());
        }
    }
}
