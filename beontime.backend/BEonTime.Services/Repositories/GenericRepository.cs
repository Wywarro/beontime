using BEonTime.Data;
using BEonTime.Data.Attributes;
using BEonTime.Data.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BEonTime.Services.Repositories
{
    public abstract class GenericRepository<TDocument> : IGenericRepository<TDocument>
            where TDocument : IDocument
    {
        private protected readonly IMongoCollection<TDocument> collection;

        public GenericRepository(IAppDbContext context)
        {
            collection = context.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        }

        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                   typeof(BsonCollectionAttribute),
                   true)
               .FirstOrDefault())?.CollectionName;
        }

        public virtual IQueryable<TDocument> AsQueryable()
        {
            return collection.AsQueryable();
        }

        public virtual IEnumerable<TDocument> FilterBy(
            Expression<Func<TDocument, bool>> filterExpression)
        {
            return collection.Find(filterExpression).ToEnumerable();
        }

        public Task<List<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            return Task.Run(() => collection.Find(filterExpression).ToListAsync());
        }

        public virtual IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression)
        {
            return collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
        }

        public virtual TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression)
        {
            return collection.Find(filterExpression).FirstOrDefault();
        }

        public virtual Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            return Task.Run(() => collection.Find(filterExpression).FirstOrDefaultAsync());
        }

        public virtual TDocument FindById(string id)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
            return collection.Find(filter).FirstOrDefault();
        }

        public virtual Task<TDocument> FindByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
                return collection.Find(filter).FirstOrDefaultAsync();
            });
        }


        public virtual void InsertOne(TDocument document)
        {
            collection.InsertOne(document);
        }

        public virtual Task InsertOneAsync(TDocument document)
        {
            if (document == null)
                throw new ArgumentNullException($"{nameof(TDocument)} object is null");

            return Task.Run(() => collection.InsertOneAsync(document));
        }

        public void InsertMany(ICollection<TDocument> documents)
        {
            collection.InsertMany(documents);
        }


        public virtual async Task InsertManyAsync(ICollection<TDocument> documents)
        {
            await collection.InsertManyAsync(documents);
        }

        public void ReplaceOne(TDocument document)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            collection.FindOneAndReplace(filter, document);
        }

        public virtual async Task ReplaceOneAsync(TDocument document)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            await collection.FindOneAndReplaceAsync(filter, document);
        }

        public void DeleteOne(Expression<Func<TDocument, bool>> filterExpression)
        {
            collection.FindOneAndDelete(filterExpression);
        }

        public Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            return Task.Run(() => collection.FindOneAndDeleteAsync(filterExpression));
        }

        public void DeleteById(string id)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
            collection.FindOneAndDelete(filter);
        }

        public Task DeleteByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
                collection.FindOneAndDeleteAsync(filter);
            });
        }

        public void DeleteMany(Expression<Func<TDocument, bool>> filterExpression)
        {
            collection.DeleteMany(filterExpression);
        }

        public Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            return Task.Run(() => collection.DeleteManyAsync(filterExpression));
        }
    }
}
