﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.BusinessRules
{
    public interface IRule
    {
        string Name { get; }

        Task ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken);
    }
}