// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Async
{
    using System;
    using System.Disposables;
    using System.Threading.Tasks;

    /// <summary>
    /// Class that contains methods for dealing with tasks.
    /// </summary>
    public static class TaskHelpers
    {
        /// <summary>
        /// Executes a function after a task has completed, faulted, or has been canceled.
        /// </summary>
        public static Task<T> Finally<T>(this Task<T> task, Action @finally)
        {
            task.ContinueWith(_ => @finally());
            return task;
        }

        /// <summary>
        /// Executes a function after a task has completed.
        /// </summary>
        public static Task<TOut> Select<T, TOut>(this Task<T> task, Func<T, TOut> @then)
        {
            return task.ContinueWith(t => @then(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        /// <summary>
        /// Executes a function after a task has completed.
        /// </summary>
        public static Task<T> Do<T>(this Task<T> task, Action<T> @then)
        {
            return task.ContinueWith(t =>
                                         {
                                             @then(t.Result);
                                             return t.Result;
                                         },
                                         TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        /// <summary>
        /// Executes a function after a task has completed.
        /// </summary>
        public static Task<T> Do<T>(this Task<T> task, Action @then)
        {
            return task.Do(_ => then());
        }

        /// <summary>
        /// Safely uses a Disposable resource inside of a task.
        /// </summary>
        public static Task<TOut> Using<TIn, TOut>(TIn resource, Func<TIn, Task<TOut>> taskFunc)
            where TIn : IDisposable
        {
            using (var disposable = new RefCountDisposable(resource))
            {
                return taskFunc(resource).Finally(disposable.GetDisposable().Dispose);
            }
        }
    }
}