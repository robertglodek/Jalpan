﻿using Jalpan.Contexts.Accessors;
using Jalpan.Contexts.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Contexts;

public static class Extensions
{
    public static IJalpanBuilder AddContexts(this IJalpanBuilder builder)
    {
        builder.Services.AddSingleton<IContextProvider, ContextProvider>();
        builder.Services.AddSingleton<IContextAccessor, ContextAccessor>();

        return builder;
    }

    public static IJalpanBuilder AddDataContexts<T>(this IJalpanBuilder builder)
    {
        builder.Services.AddSingleton<IDataContextProvider<T>, DataContextProvider<T>>();
        builder.Services.AddSingleton<IDataContextAccessor<T>, DataContextAccessor<T>>();
        return builder;
    }
}