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

        private ConcurrentQueue<int> checkTimesQueue;

        private Timer checkTimer;

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

            // Configure semaphore and limit the number of concurrent executions
            this.semaphore = new SemaphoreSlim(this.maxExecutionsPerTimePeriod, this.maxExecutionsPerTimePeriod);

            // TODO Comment
            this.checkTimesQueue = new ConcurrentQueue<int>();

            // TODO Comment
            this.checkTimer = new Timer(this.EvaluateRetardation, null, 1, Timeout.Infinite);
        }

        private void EvaluateRetardation(object state) {
            // TODO Comment
            int semaphoreExitTime = 0;
            while (this.checkTimesQueue.TryPeek(out semaphoreExitTime) && (Environment.TickCount >= semaphoreExitTime))
            {
                this.semaphore.Release();
                this.checkTimesQueue.TryDequeue(out semaphoreExitTime);
            }

            // TODO Comment
            if (this.checkTimesQueue.TryPeek(out semaphoreExitTime))
            {
                this.checkTimer.Change((semaphoreExitTime - Environment.TickCount), Timeout.Infinite);
            }
            else
            {
                this.checkTimer.Change(this.timePeriodInMillis, Timeout.Infinite);
            }
        }

        public bool HangOn()
        {
            // TODO Comment
            return this.HangOn(Timeout.Infinite);
        }

        public bool HangOn(int timeoutInMillis)
        {
            // Validate argument
            if (timePeriodInMillis <= 0) {
                throw new ArgumentOutOfRangeException("timePeriodInMillis", "This parameter must be a positive integer!");
            }

            // TODO Comment
            var threadEntered = this.semaphore.Wait(timeoutInMillis);

            if (threadEntered) {
                this.checkTimesQueue.Enqueue(Environment.TickCount + this.timePeriodInMillis);
            }

            return threadEntered;
        }
    }

}