/*
    The MIT License (MIT)

    Copyright (c) 2015  The Modular Project (https://bitbucket.org/juanshaf/modular)

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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modular
{
    /// <summary>
    /// ModuleResponse allows a module to identify whether a call to another module has been successful or not.
    /// </summary>
    public enum ModuleResponse
    {
        /// <summary>
        /// Should be returned from IModuleHost's RunModuleMethod method if a module is not found, otherwise the IModuleHost should return the response from the requested module.
        /// </summary>
        ModuleNotFound,

        /// <summary>
        /// Should be returned from a module's RunMethod method if the module is accepting the method call.
        /// </summary>
        MethodAccepted,

        /// <summary>
        /// Should be returned from a module's RunMethod method if the module is rejecting the method call.
        /// </summary>
        MethodNotFound,

        /// <summary>
        /// Should be returned in the module's RunMethod ModuleRunMethodCallback response field if the method call was successful.
        /// </summary>
        MethodSuccess,

        /// <summary>
        /// Should be returned in the module's RunMethod ModuleRunMethodCallback response field if the method call was unsuccessful.
        /// </summary>
        MethodFail,
    }
}
