using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using MySLOTree.Models;

namespace MySLOTree.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleNewsDBAttribute : ActionFilterAttribute
    {
        private static NewsDBInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class NewsDBInitializer
        {
            public NewsDBInitializer()
            {
                Database.SetInitializer<NewsContext>(null);

                try
                {
                    using (var context = new NewsContext())
                    {
                        if (!context.Database.Exists())
                        {
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The news database could not be initialized.", ex);
                }
            }
        }
    }
}
