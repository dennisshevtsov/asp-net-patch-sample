﻿// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

namespace AspNetPatchSample.Data.Test
{
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;

  public abstract class DataTestBase
  {
#pragma warning disable CS8618
    private DbContext _dbContext;
    private IServiceScope _serviceScope;
#pragma warning restore CS8618

    protected DbContext DbContext => _dbContext;

    [TestInitialize]
    public void Initialize()
    {
      var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                    .Build();

      _serviceScope = new ServiceCollection().SetUpInfrastructure(configuration)
                                             .BuildServiceProvider()
                                             .CreateScope();

      _dbContext = _serviceScope.ServiceProvider.GetRequiredService<DbContext>();
      _dbContext.Database.EnsureCreated();

      Initialize(_serviceScope.ServiceProvider);
    }

    protected abstract void Initialize(IServiceProvider provider);

    [TestCleanup]
    public void Cleanup()
    {
      _dbContext?.Database?.EnsureDeleted();
      _serviceScope?.Dispose();
    }
  }
}
