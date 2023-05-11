﻿/*
    The MIT License

    Copyright (c) 2021 MathEval.org

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/
using System;
using static org.matheval.Common.Afe_Common;

namespace org.matheval.Operators
{
    /// <summary>
    /// Create interface IOperator
    /// </summary>
    public interface IOperator
    {
        /// <summary>
        /// Get Prec
        /// </summary>
        /// <returns>Value is type int</returns>
        int GetPrec();

        /// <summary>
        /// Get Op
        /// </summary>
        /// <returns>Value is type string</returns>
        string GetOp();

        /// <summary>
        /// Get Ass
        /// </summary>
        /// <returns>Value is type enum Assoc</returns>
        Assoc GetAss();

        /// <summary>
        /// Calculate
        /// </summary>
        /// <returns>Value is type Object result Calculate</returns>
        object Calculate(object left, object right, ExpressionContext dc);

        /// <summary>
        /// Validate
        /// </summary>
        /// <returns>Value is System.Type</returns>
        Type Validate(Type typeLeft, Type typeRight);
    }
}
