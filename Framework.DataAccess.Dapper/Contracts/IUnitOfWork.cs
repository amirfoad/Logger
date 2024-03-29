﻿using Framework.DataAccess.Dapper.Contracts;

namespace Framework.DataAccess.Dapper
{
    public interface IUnitOfWork
    {
        void Dispose();
        IEnumerable<TEntity> ExecuteFunction<TEntity>(string name, string predicate, object data, int? commandTimeout = 0);
        Task<IEnumerable<TEntity>> ExecuteFunctionAsync<TEntity>(string name, string predicate, object data, int? commandTimeout = 0);
        TEntity ExecuteFunctionSingle<TEntity>(string name, string predicate, object data, int? commandTimeout = 0);
        Task<TEntity> ExecuteFunctionSingleAsync<TEntity>(string name, string predicate, object data, int? commandTimeout = 0);
        Task<IEnumerable<dynamic>> ExecuteQueryAsync(string query);
        int ExecuteStoredProcedure(string name, object data, int? commandTimeout = 0);
        IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string name, int? commandTimeout = 0);
        IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string name, object data, int? commandTimeout = 0);
        Task<int> ExecuteStoredProcedureAsync(string name, object data, int? commandTimeout = 0);
        Task<IEnumerable<TEntity>> ExecuteStoredProcedureAsync<TEntity>(string name, int? commandTimeout = 0);
        Task<IEnumerable<TEntity>> ExecuteStoredProcedureAsync<TEntity>(string name, object data, int? commandTimeout = 0);
        TEntity ExecuteStoredProcedureSingle<TEntity>(string name, int? commandTimeout = 0);
        TEntity ExecuteStoredProcedureSingle<TEntity>(string name, object data, int? commandTimeout = 0);
        Task<TEntity> ExecuteStoredProcedureSingleAsync<TEntity>(string name, int? commandTimeout = 0);
        Task<TEntity> ExecuteStoredProcedureSingleAsync<TEntity>(string name, object data, int? commandTimeout = 0);
        IRepository<T> GetDataRepository<T>() where T : class, new();
        IDataService<T> GetDataService<T>() where T : class;
    }
}