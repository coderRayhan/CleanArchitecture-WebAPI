using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Abstractions.Contracts;
public interface ICacheInvalidatorCommand : ICommand, IBaseCacheInvalidatorCommand
{
}

public interface ICacheInvalidatorCommand<TResponse> : ICommand<TResponse>, IBaseCacheInvalidatorCommand
{
}

public interface IBaseCacheInvalidatorCommand
{
    string[] CacheKeys { get; }
}