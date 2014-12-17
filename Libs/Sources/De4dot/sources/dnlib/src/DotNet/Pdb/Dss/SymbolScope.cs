﻿/*
    Copyright (C) 2012-2014 de4dot@gmail.com

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the
    "Software"), to deal in the Software without restriction, including
    without limitation the rights to use, copy, modify, merge, publish,
    distribute, sublicense, and/or sell copies of the Software, and to
    permit persons to whom the Software is furnished to do so, subject to
    the following conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
    IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
    CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
    SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Diagnostics.SymbolStore;

namespace dnlib.DotNet.Pdb.Dss {
	sealed class SymbolScope : ISymbolScope {
		readonly ISymUnmanagedScope scope;

		public SymbolScope(ISymUnmanagedScope scope) {
			this.scope = scope;
		}

		public int EndOffset {
			get {
				uint result;
				scope.GetEndOffset(out result);
				return (int)result;
			}
		}

		public ISymbolMethod Method {
			get {
				ISymUnmanagedMethod method;
				scope.GetMethod(out method);
				return method == null ? null : new SymbolMethod(method);
			}
		}

		public ISymbolScope Parent {
			get {
				ISymUnmanagedScope parentScope;
				scope.GetParent(out parentScope);
				return parentScope == null ? null : new SymbolScope(parentScope);
			}
		}

		public int StartOffset {
			get {
				uint result;
				scope.GetStartOffset(out result);
				return (int)result;
			}
		}

		public ISymbolScope[] GetChildren() {
			uint numScopes;
			scope.GetChildren(0, out numScopes, null);
			var unScopes = new ISymUnmanagedScope[numScopes];
			scope.GetChildren((uint)unScopes.Length, out numScopes, unScopes);
			var scopes = new ISymbolScope[numScopes];
			for (uint i = 0; i < numScopes; i++)
				scopes[i] = new SymbolScope(unScopes[i]);
			return scopes;
		}

		public ISymbolVariable[] GetLocals() {
			uint numVars;
			scope.GetLocals(0, out numVars, null);
			var unVars = new ISymUnmanagedVariable[numVars];
			scope.GetLocals((uint)unVars.Length, out numVars, unVars);
			var vars = new ISymbolVariable[numVars];
			for (uint i = 0; i < numVars; i++)
				vars[i] = new SymbolVariable(unVars[i]);
			return vars;
		}

		public ISymbolNamespace[] GetNamespaces() {
			uint numNss;
			scope.GetNamespaces(0, out numNss, null);
			var unNss = new ISymUnmanagedNamespace[numNss];
			scope.GetNamespaces((uint)unNss.Length, out numNss, unNss);
			var nss = new ISymbolNamespace[numNss];
			for (uint i = 0; i < numNss; i++)
				nss[i] = new SymbolNamespace(unNss[i]);
			return nss;
		}
	}
}
