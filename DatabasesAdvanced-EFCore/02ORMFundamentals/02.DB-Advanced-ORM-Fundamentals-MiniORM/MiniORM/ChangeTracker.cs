namespace MiniORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    internal class ChangeTracker<TEntity>
        where TEntity : class, new()
    {
        private readonly List<TEntity> allEntities;

        private readonly List<TEntity> added;

        private readonly List<TEntity> removed;

        public ChangeTracker(IEnumerable<TEntity> entities)
        {
            this.added = new List<TEntity>();
            this.removed = new List<TEntity>();

            this.allEntities = CloneEntities(entities);
        }

        private static List<TEntity> CloneEntities(IEnumerable<TEntity> entities)
        {
            var clonedEntities = new List<TEntity>();

            var propertiesToClone = typeof(TEntity).GetProperties()
                .Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType))
                .ToArray();

            foreach (var entity in entities)
            {
                var clonedEntity = Activator.CreateInstance<TEntity>();

                foreach (var property in propertiesToClone)
                {
                    var value = property.GetValue(entity);
                    property.SetValue(clonedEntity, value);
                }

                clonedEntities.Add(clonedEntity);
            }

            return clonedEntities;
        }

        public IReadOnlyCollection<TEntity> AllEntities => this.allEntities.AsReadOnly();

        public IReadOnlyCollection<TEntity> Added => this.added.AsReadOnly();

        public IReadOnlyCollection<TEntity> Removed => this.removed.AsReadOnly();

        public void Add(TEntity item) => this.added.Add(item);

        public void Remove(TEntity item) => this.removed.Add(item);

        public IEnumerable<TEntity> GetModifiedEntities(DbSet<TEntity> dbSet)
        {
            var modifiedEntities = new List<TEntity>();

            var primaryKeys = typeof(TEntity).GetProperties()
                .Where(pi => pi.HasAttribute<KeyAttribute>())
                .ToArray();

            foreach (var proxyEntity in this.AllEntities)
            {
                var primaryKeyValues = GetPrimaryKeyValues(primaryKeys, proxyEntity).ToArray();

                var entity = dbSet.Entities
                    .Single(e => GetPrimaryKeyValues(primaryKeys, e).SequenceEqual(primaryKeyValues));

                var isModified = IsModified(proxyEntity, entity);
                if (isModified)
                {
                    modifiedEntities.Add(entity);
                }
            }

            return modifiedEntities;
        }

        private static bool IsModified(TEntity entity, TEntity proxyEntity)
        {
            var monitoredProperties = typeof(TEntity).GetProperties()
                .Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType));

            var modifiedProperties = monitoredProperties
                .Where(pi => !Equals(pi.GetValue(entity), pi.GetValue(proxyEntity)))
                .ToArray();

            var isModified = modifiedProperties.Any();

            return isModified;
        }

        private static IEnumerable<object> GetPrimaryKeyValues(IEnumerable<PropertyInfo> primaryKeys, TEntity entity)
        {
            return primaryKeys.Select(pk => pk.GetValue(entity));
        }
    }
}