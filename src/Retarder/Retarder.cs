namespace awes0mecoderz.Retarder
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    /// <summary>
    /// https://en.wikipedia.org/wiki/Leaky_bucket
    /// https://en.wikipedia.org/wiki/Generic_cell_rate_algorithm
    /// TODO Use float/double instead of int???
    /// TODO Use ticks instead millis???
    /// </summary>
    public class Retarder : IRetarder {

        #region Fields

        private readonly SemaphoreSlim semaphore;
        // Check HiPerfTimer???
        private readonly Timer timer;
        private readonly ConcurrentQueue<int> semaphoreExitTimesQueue;
        private int timeSpanInMillis;
        private int maxExecutionsPerTimeSpan;

        #endregion

        public Retarder(int timeSpanInMillis, int maxExecutionsPerTimeSpan)
        {
            // Check if arguments are valid
            if ((timeSpanInMillis <= 0) || (timeSpanInMillis >= int.MaxValue))
            {
                throw new ArgumentOutOfRangeException("timeSpan", "This parameter must be a positive integer!");
            }
            if ((maxExecutionsPerTimeSpan <= 0) || (maxExecutionsPerTimeSpan >= int.MaxValue))
            {
                throw new ArgumentOutOfRangeException("maxExecutionsPerTimeSpan", "This parameter must be a positive integer!");
            }

            // Store arguments as internal state
            this.timeSpanInMillis = timeSpanInMillis;
            this.maxExecutionsPerTimeSpan = maxExecutionsPerTimeSpan;

            // TODO Comment
            this.semaphore = new SemaphoreSlim(this.maxExecutionsPerTimeSpan, this.maxExecutionsPerTimeSpan);

            // TODO Comment
            this.semaphoreExitTimesQueue = new ConcurrentQueue<int>();

            // TODO Comment
            this.timer = new Timer(this.IsStillRetarded, null, this.timeSpanInMillis, Timeout.Infinite);
        }

        public void Retard()
        {
            if (this.semaphore.Wait(Timeout.Infinite))
            {
                this.semaphoreExitTimesQueue.Enqueue(Environment.TickCount + this.timeSpanInMillis);
            }
        }

        private void IsStillRetarded(object state)
        {
            // TODO Comment
            int semaphoreExitTime = 0;
            while (this.semaphoreExitTimesQueue.TryPeek(out semaphoreExitTime)
                && (Environment.TickCount >= semaphoreExitTime))
            {
                this.semaphore.Release();
                this.semaphoreExitTimesQueue.TryDequeue(out semaphoreExitTime);
            }

            // TODO Comment
            if (this.semaphoreExitTimesQueue.TryPeek(out semaphoreExitTime))
            {
                this.timer.Change((semaphoreExitTime - Environment.TickCount), Timeout.Infinite);
            }
            else
            {
                this.timer.Change(this.timeSpanInMillis, Timeout.Infinite);
            }
        }

    }

}