using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Zmm;

public class ZmmTaskManager(IOptions<ZmmOptions> options) : ISingletonDependency
{
    private readonly List<Task> _tasks = [];
    private readonly SemaphoreSlim _asyncLock = new(1);

    public async Task WaitTaskAsync(Task task)
    {
        await _asyncLock.WaitAsync();
        try
        {
            _tasks.AddIfNotContains(task);
            _tasks.RemoveAll(u => u.IsCompleted || u.IsCanceled || u.IsFaulted);
            if (_tasks.Count > options.Value.MaxThreadCount)
            {
                await Task.WhenAny(_tasks);
            }
        }
        finally
        {
            _asyncLock.Release();
        }
    }
}