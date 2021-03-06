﻿using System;

namespace RefactorToMonadicCSharp
{
    /// <summary>
    /// Helps C# with type inference for lambdas
    /// </summary>
    public static class L {
        /// <summary>
        /// Helps C# with type inference for lambdas
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Func<A,B> F<A,B>(Func<A,B> f) {
            return f;
        }

        public static Func<A> F<A>(Func<A> f) {
            return f;
        }

        public static Func<A,B,C> F<A,B,C>(Func<A,B,C> f) {
            return f;
        }

    }
}
