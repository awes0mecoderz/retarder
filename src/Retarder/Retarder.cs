namespace awes0mecoderz.Retarder
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    public class Retarder : IRetarder {

        #region Fields

        private int timePeriodInMillis;

        private int maxExecutionsPerTimePeriod;

        private SemaphoreSlim semaphore;

        private ConcurrentQueue<int> spReleaseTimeQueue;

        private Timer timer;

        #endregion

        public Retarder(int timePeriodInMillis, int maxExecutionsPerTimePeriod)
        {
            // Validate arguments
            if (timePeriodInMillis <= 0) {
                throw new ArgumentOutOfRangeException("timePeriodInMillis", "This parameter must be a positive integer!");
            }

            if (maxExecutionsPerTimePeriod <= 0) {
                throw new ArgumentOutOfRangeException("maxExecutionsPerTimePeriod", "This parameter must be a positive integer!");
            }

            // Store the arguments as internal state
            this.timePeriodInMillis = timePeriodInMillis;
            this.maxExecutionsPerTimePeriod = maxExecutionsPerTimePeriod;

            // Configure semaphore and limit the maximum number of concurrent executions
            this.semaphore = new SemaphoreSlim(this.maxExecutionsPerTimePeriod, this.maxExecutionsPerTimePeriod);

            // Setup queue that will store the list of next semaphore release times
            this.spReleaseTimeQueue = new ConcurrentQueue<int>();

            // Everytime that the timer countdown reaches "0" "EvaluateRetardation" method will be called.
            // Timer starts right after this statement has been executed, but waits for "HangOn" method call
            // to set up the callback method cyclic countdown
            this.timer = new Timer(this.EvaluateRetardation, null, 1, Timeout.Infinite);
        }

        // "TimerCallback method" used by timer
        private void EvaluateRetardation(object state) {
            // Release one thread from semaphore each time that a "scheduled release time" has passed
            int semaphoreExitTime = 0;
            while (this.spReleaseTimeQueue.TryPeek(out semaphoreExitTime) && (Environment.TickCount >= semaphoreExitTime))
            {
                this.semaphore.Release();
                this.spReleaseTimeQueue.TryDequeue(out semaphoreExitTime);
            }

            // If the queue has values...
            if (this.spReleaseTimeQueue.TryPeek(out semaphoreExitTime))
            {
                // Adjust the timer for the next "scheduled release time"
                this.timer.Change((semaphoreExitTime - Environment.TickCount), Timeout.Infinite);
            }
            else
            {
                // If not... wait for "HangOn" method call to set up the callback cyclic countdown again
                this.timer.Change(this.timePeriodInMillis, Timeout.Infinite);
            }
        }

        // This method is responsible for block the thread until there is an available oportunity to execute the next statement
        // of code that calls this method
        public bool HangOn()
        {
            // Calls the "HangOn(int)" signature with an infinite timeout
            // Returns true if the current thread was signaled by the semaphore, false otherwise
            return this.HangOn(Timeout.Infinite);
        }

        // This method is responsible for block the thread until there is an available oportunity to execute the next statement
        // of code that calls this method or a timeout defined by you has been reached
        public bool HangOn(int timeoutInMillis)
        {
            // Validate argument
            if (timePeriodInMillis <= 0) {
                throw new ArgumentOutOfRangeException("timePeriodInMillis", "This parameter must be a positive integer!");
            }

            // Waits until there is an available oportunity to put the current thread inside semaphore or
            // a timeout defined has been reached
            var threadEntered = this.semaphore.Wait(timeoutInMillis);

            // If the timeout has not been reached, we will define the earliest moment that the semaphore will
            // release the current thread
            if (threadEntered) {
                this.spReleaseTimeQueue.Enqueue(Environment.TickCount + this.timePeriodInMillis);
            }

            // Returns true if the current thread was signaled by the semaphore, false otherwise
            return threadEntered;
        }
    }
}