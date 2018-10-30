using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using KiiCorp.Cloud.Storage;
using UnityEngine;

namespace KiiCorp.Cloud.Unity
{
    /// <summary>
    /// Main looper.
    /// </summary>
    internal class WWWRequestLooper
    {
        // Default timeout is 30 seconds.
        internal static int ApiTimeout = 30;
        private readonly static ReaderWriterLockSlim requestQueueLock;
        private readonly static Queue<Action> requestQueue;
        
        static WWWRequestLooper ()
        {
            WWWRequestLooper.requestQueueLock = new ReaderWriterLockSlim();
            WWWRequestLooper.requestQueue = new Queue<Action>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="KiiCorp.Cloud.Storage.WWWRequestLooper"/> class.
        /// </summary>
        public WWWRequestLooper ()
        {
        }
        /// <summary>
        /// Registers the network request.
        /// </summary>
        /// <param name="www">Www.</param>
        /// <param name="progressCallback">Callback for progrees.</param>
        /// <param name="action">Action.</param>
        internal static void RegisterNetworkRequest(WWW www, ProgressCallbackHelper progressCallback, Func<WWW, ProgressCallbackHelper, bool> action)
        {
            WWWRequestLooper.RunOnMainThread(() => {
                bool isDone = action(www, progressCallback);
                if (!isDone)
                {
                    WWWRequestLooper.RegisterNetworkRequest(www, progressCallback, action);
                }
            });
        }
        /// <summary>
        /// Runs the on main thread.
        /// </summary>
        /// <param name="action">Action.</param>
        internal static void RunOnMainThread(Action action)
        {
            if (WWWRequestLooper.requestQueueLock.IsWriteLockHeld)
            {
                WWWRequestLooper.requestQueue.Enqueue(action);
                return;
            }
            WWWRequestLooper.requestQueueLock.EnterWriteLock();
            try
            {
                WWWRequestLooper.requestQueue.Enqueue(action);
            }
            finally
            {
                WWWRequestLooper.requestQueueLock.ExitWriteLock();
            }
        }
        /// <summary>
        /// Runs the loop.
        /// </summary>
        /// <returns>The loop.</returns>
        internal static IEnumerator RunLoop()
        {
            while (true)
            {
                WWWRequestLooper.requestQueueLock.EnterUpgradeableReadLock();
                try
                {
                    int count = WWWRequestLooper.requestQueue.Count;
                    if (count > 0)
                    {
                        WWWRequestLooper.requestQueueLock.EnterWriteLock();
                        try
                        {
                            while (count > 0)
                            {
                                try
                                {
                                    WWWRequestLooper.requestQueue.Dequeue()();
                                }
                                catch (Exception ignore)
                                {
                                    if (Kii.Logger != null)
                                    {
                                        Kii.Logger.Debug("Error:", ignore);
                                    }
                                    Debug.Log(ignore.ToString());
                                }
                                count--;
                            }
                        }
                        finally
                        {
                            WWWRequestLooper.requestQueueLock.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    WWWRequestLooper.requestQueueLock.ExitUpgradeableReadLock();
                }
                yield return null;
            }
        }
    }
}

