using System;
using System.Threading.Tasks;

namespace MarginTrading.NotificationGenerator.Core.Helpers
{
    public class TaskBatchRunner
    {
        private readonly System.Threading.SemaphoreSlim _semaphoreSlim;

        /// <summary>
        /// Set levelOfParallelism from 1 to 128
        /// </summary>
        /// <param name="levelOfParallelism"></param>
        public TaskBatchRunner(int levelOfParallelism = 10)
        {
            if (levelOfParallelism < 1)
                levelOfParallelism = 1;
            if (levelOfParallelism > 128)
                levelOfParallelism = 128;
            
            _semaphoreSlim = new System.Threading.SemaphoreSlim(levelOfParallelism, levelOfParallelism);
        }

        public async Task Run(Func<Task> handler)
        {
            await _semaphoreSlim.WaitAsync();
            
            try
            {
                await handler();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}