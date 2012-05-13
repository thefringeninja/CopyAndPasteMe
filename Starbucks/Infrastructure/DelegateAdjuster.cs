using System;
using System.Linq.Expressions;

namespace Starbucks.Infrastructure
{
    public class DelegateAdjuster
    {
        public static Action<BaseT> CastArgument<BaseT, DerivedT>(Expression<Action<DerivedT>> source)
            where DerivedT : BaseT
        {
            if (typeof (DerivedT)
                == typeof (BaseT))
            {
                return (Action<BaseT>) ((Delegate) source.Compile());
            }
            ParameterExpression sourceParameter = Expression.Parameter(typeof (BaseT), "source");
            Expression<Action<BaseT>> result = Expression.Lambda<Action<BaseT>>(
                Expression.Invoke(
                    source,
                    Expression.Convert(sourceParameter, typeof (DerivedT))),
                sourceParameter);
            return result.Compile();
        }
    }
}