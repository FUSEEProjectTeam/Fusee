//Use project level define(s) when referencing with Paket.
//#define CSX_IDENTITY_INTERNAL // Uncomment this to set visibility to internal.

using System;

namespace CSharpx
{
#if !CSX_IDENTITY_INTERNAL
    public
#endif
    sealed class Identity<T>
    {
        private readonly T value;

        public Identity(T value)
        {
            this.value = value;
        }

        public T Value
        {
            get { return this.value; }
        }
    }

#if !CSX_IDENTITY_INTERNAL
    public
#endif
    static class IdentityExtensions
    {
        public static Identity<T> ToIdentity<T>(this T value)
        {
            return new Identity<T>(value);
        }

        public static Identity<T2> Bind<T1, T2>(this Identity<T1> identity, Func<T1, Identity<T2>> func)
        {
            return func(identity.Value);
        }

        public static Identity<T2> Map<T1, T2>(this Identity<T1> identity, Func<T1, T2> func)
        {
            return func(identity.Value).ToIdentity();
        }

        public static Identity<TResult> Select<TSource, TResult>(
            this Identity<TSource> identity, Func<TSource, TResult> selector)
        {
            return selector(identity.Value).ToIdentity();
        }

        public static Identity<TResult> SelectMany<TSource, TValue, TResult>(
            this Identity<TSource> identity,
            Func<TSource, Identity<TValue>> valueSelector,
            Func<TSource, TValue, TResult> resultSelector)
        {
            return resultSelector(identity.Value, valueSelector(identity.Value).Value).ToIdentity();
        }
    }
}