using System;
using System.Collections.Generic;
using System.Linq;
using Starbucks.Actors;

namespace Starbucks.Wcf
{
    public class Sales : IBoundedContext
    {
        #region IBoundedContext Members

        public IEnumerable<Type> GetUseCases()
        {
            return typeof (Barista).Assembly.GetTypes()
                .Where(typeof (Command).IsAssignableFrom)
                .Where(IsExportable);
        }

        public string GetName()
        {
            return "Sales";
        }

        #endregion

        private static bool IsExportable(Type type)
        {
            return type.IsClass && false == type.IsAbstract;
            // or type.HasAttribute<Exportable>() or whatever
        }
    }
}