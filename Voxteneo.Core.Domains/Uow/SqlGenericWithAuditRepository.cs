using System;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Reflection;
using Voxteneo.Core.Domains.Arguments;
using Voxteneo.Core.Domains.Contracts;
using Voxteneo.WebComponents.Logger;

namespace Voxteneo.Core.Domains.Uow
{
    public class SqlGenericWithAuditRepository<Class, TAuditTrail> : SqlGenericRepository<Class>,
        IGenericWithAuditRepository<Class> where Class : class
        where TAuditTrail : class, IAuditTrailEntity
    {
        private readonly DbSet<TAuditTrail> _dbSetAuditTrail;


        public SqlGenericWithAuditRepository(DbContext context, ILogger logger) : base(context, logger)
        {
            _dbSetAuditTrail = context.Set<TAuditTrail>();
        }

        /// <summary>
        ///     update data
        /// </summary>
        /// <param name="entity"></param>
        public override void Update(Class entity)
        {
            var entryObject = GetEntryWrapperFromEntity(entity);
            var edmEntityType = GetEntityKeys(entity, entryObject);
            var id = GetIdFromEntity(entity, entryObject, edmEntityType);
            var oldEntity = GetOldEntityFromRepository(entity, id);
            foreach (var property in entity.GetType().GetProperties())
            {
                var canContinue = CheckData(entity, property);
                if (canContinue)
                {
                    if (property.GetValue(entity) == null) continue;
                    if (!CompareOldWithNewValue(entity, oldEntity, property))
                    {
                        var audit = IniDataAudit(property);
                        audit = SetValueAudit(entity, id, oldEntity, property, audit);
                        var args = InitTheArgumentForUpdate(entity, audit);
                        OnUpdate(args);
                        AddAuditData(audit, args);
                    }
                }
            }
            base.Update(entity);
        }

        /// <summary>
        ///     check data is part of entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private static bool CheckData(Class entity, PropertyInfo property)
        {
            //  var checkEntity = CheckIfTheValueIsEntity(entity, property);
            var isEnumerable = IsEnumerable(entity, property);
            //checkEntity == null
            return isEnumerable;
        }

        /// <summary>
        ///     set value for audit
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="oldEntity"></param>
        /// <param name="property"></param>
        /// <param name="audit"></param>
        /// <returns></returns>
        private static TAuditTrail SetValueAudit(Class entity, EntityKeyMember id, Class oldEntity,
            PropertyInfo property, TAuditTrail audit)
        {
            SetRelationId(entity, id, ref audit);
            SetOldValue(oldEntity, property, ref audit);
            SetNewValue(entity, property, ref audit);
            return audit;
        }

        /// <summary>
        ///     initialize first data from entity audit
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static TAuditTrail IniDataAudit(PropertyInfo property)
        {
            var audit = Activator.CreateInstance<TAuditTrail>();
            audit.RelationType = 0;
            audit.ActionType = 1;

            audit.ActionDate = DateTime.Now;
            audit.Field = property.Name;
            audit.Username = Constants.DashValue;
            return audit;
        }

        /// <summary>
        ///     add data to repository
        /// </summary>
        /// <param name="audit"></param>
        /// <param name="args"></param>
        private void AddAuditData(TAuditTrail audit, UpdateArgs<Class> args)
        {
            if (!args.Handle)
            {
                _dbSetAuditTrail.Add(audit);
            }
        }

        private static void SetNewValue(Class entity, PropertyInfo property, ref TAuditTrail audit)
        {
            if (property.GetValue(entity) == null)
                audit.NewValue = Constants.DashValue;
            else
            {
                var entityWrapper = property.GetValue(entity)
                        .GetType()
                        .GetField("_entityWrapper",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                            BindingFlags.CreateInstance);
                if (entityWrapper != null)
                {
                    var model = property.GetValue(entity);
                    var propertyName = model.GetType().GetProperty("Name");
                    audit.NewValue = propertyName != null ? propertyName.GetValue(model).ToString() : property.GetValue(entity).ToString();
                }
                else
                    audit.NewValue = property.GetValue(entity).ToString();
            }
        }

        private static void SetOldValue(Class oldEntity, PropertyInfo property, ref TAuditTrail audit)
        {
            if (property.GetValue(oldEntity) == null)
                audit.OldValue = Constants.DashValue;
            else
            {
                var entityWrapper = property.GetValue(oldEntity)
                        .GetType()
                        .GetField("_entityWrapper",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                            BindingFlags.CreateInstance);
                if (entityWrapper != null)
                {
                    var model = property.GetValue(oldEntity);
                    var propertyName = model.GetType().GetProperty("Name");
                    audit.OldValue = propertyName != null ? propertyName.GetValue(model).ToString() : property.GetValue(oldEntity).ToString();
                }
                else
                    audit.OldValue = property.GetValue(oldEntity).ToString();
            }
        }

        private static void SetRelationId(Class entity, EntityKeyMember id, ref TAuditTrail audit)
        {
            var idEntity = 0;
            int.TryParse(entity.GetType().GetProperty(id.Key).GetValue(entity).ToString(), out idEntity);
            audit.RelationId = idEntity;
        }

        private static bool CompareOldWithNewValue(Class entity, Class oldEntity, PropertyInfo property)
        {
            var entityWrapper = property.GetValue(entity)
                       .GetType()
                       .GetField("_entityWrapper",
                           BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                           BindingFlags.CreateInstance);
            if (entityWrapper != null)
            {
                var model = property.GetValue(entity);
                var propertyName = model.GetType().GetProperty("Name");
                if (propertyName != null)
                {
                    var newValue = "";
                    var oldValue = "";
                    if (property.GetValue(entity) != null)
                    {
                        if (propertyName.GetValue(property.GetValue(entity)) != null)
                            newValue = propertyName.GetValue(property.GetValue(entity)).ToString();
                    }
                    if (property.GetValue(oldEntity) != null)
                    {
                        if (propertyName.GetValue(property.GetValue(oldEntity)) != null)
                            oldValue = propertyName.GetValue(property.GetValue(oldEntity)).ToString();
                    }
                    return newValue.Equals(oldValue);
                }
            }
            return property.GetValue(entity).Equals(property.GetValue(oldEntity));
        }

        private static bool IsEnumerable(Class entity, PropertyInfo property)
        {
            return !(property.GetValue(entity) is IEnumerable) || property.GetValue(entity) is string;
        }

        private static object CheckIfTheValueIsEntity(Class entity, PropertyInfo property)
        {
            return property.GetValue(entity) == null
                ? new object()
                : property.GetValue(entity)
                    .GetType()
                    .GetField("_entityWrapper",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                        BindingFlags.CreateInstance);
        }

        private Class GetOldEntityFromRepository(Class entity, EntityKeyMember id)
        {
            return
                new SqlGenericRepository<Class>((DbContext)Activator.CreateInstance(Context.GetType()), Logger).GetByID
                    (entity.GetType().GetProperty(id.Key).GetValue(entity));
        }

        private static EntityKeyMember GetIdFromEntity(Class entity, FieldInfo entryObject, PropertyInfo edmEntityType)
        {
            return (edmEntityType.GetValue(entryObject.GetValue(entity)).GetType()
                .GetProperty("EntityKeyValues"
                    , BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance)
                .GetValue(edmEntityType.GetValue(entryObject.GetValue(entity))) as EntityKeyMember[])[0];
        }

        private static PropertyInfo GetEntityKeys(Class entity, FieldInfo entryObject)
        {
            return entryObject.GetValue(entity)
                .GetType()
                .GetProperty("EntityKey",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance);
        }

        private static FieldInfo GetEntryWrapperFromEntity(Class entity)
        {
            return entity.GetType()
                .GetField("_entityWrapper",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance);
        }

        private static UpdateArgs<Class> InitTheArgumentForUpdate(Class entity, TAuditTrail audit)
        {
            var args = new UpdateArgs<Class>
            {
                Entity = entity,
                Handle = false,
                Audit = audit
            };

            return args;
        }
    }
}