﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLab.Assets.EFUpdateWrapper
{

    public static class ContextExtender
    {
        public static int SaveChanges(this DbContext context ,DirectUpdateMode updatemode = DirectUpdateMode.PerEntity)
        {
            IDirectUpdateContext directContext = context as IDirectUpdateContext;

            if (directContext != null)
            {
                directContext.CurrentSaveOperationMode = updatemode;
                IEnumerable<DbEntityEntry> allEntries = context.ChangeTracker.Entries();
                if (updatemode != DirectUpdateMode.PerEntity)
                {
                    bool allow = true;
                    if (updatemode == DirectUpdateMode.AllowAll)
                        allow = true;
                    if (updatemode == DirectUpdateMode.PreventAll)
                        allow = false;
                    foreach (DbEntityEntry entry in allEntries)
                    {
                        IUpdatableEntity updatableEntity = entry.Entity as IUpdatableEntity;
                        if (updatableEntity != null)
                            updatableEntity.AllowDirectUpdate = allow;
                    }
                }

            }

            int result = context.SaveChanges();
            return result;
            
        }

        public static bool GetLoadedEntityIfAny<TEntity>(this DbContext context, ref TEntity entity)
            where TEntity : class
        {
            ObjectStateEntry entry;
            IObjectContextAdapter adapter = context as IObjectContextAdapter;
            bool shouldAttach = false;

            string entitySetName = context.GetEntitySetName<TEntity>();

            EntityKey key = adapter.ObjectContext.CreateEntityKey(entitySetName, entity);
            if (adapter.ObjectContext.ObjectStateManager.
                TryGetObjectStateEntry(key, out entry))
            {
                if (entry.State == EntityState.Detached)
                {
                    //if the entity is already loaded but has been detatched
                    shouldAttach = true;
                }
                //if the object originally exist on the context return it
                entity = (TEntity)entry.Entity;
            }
            else
            {
                //object does not exist on the context
                shouldAttach = true;
            }
            return !shouldAttach;
        }

        public static DbEntityValidationResult RemoveEFFalseAlarms(this DbContext context, DbEntityValidationResult result,
            DbEntityEntry entityEntry, IDictionary<object, object> items)
        {

            IDirectUpdateContext directContext = context as IDirectUpdateContext;

            if (directContext != null)
            {
                DirectUpdateMode? mode = directContext.CurrentSaveOperationMode;

                if (mode.HasValue && mode.Value != DirectUpdateMode.PreventAll)
                {
                    bool canDoDirectUpdate = false;
                    IUpdatableEntity entityAsUpdatable = entityEntry.Entity as IUpdatableEntity;
                    if (entityAsUpdatable != null)
                    {
                        canDoDirectUpdate = entityAsUpdatable.AllowDirectUpdate;
                    }
                    else
                    {
                        canDoDirectUpdate = (mode.Value == DirectUpdateMode.AllowAll);
                    }

                    if (canDoDirectUpdate)
                    {
                        List<DbValidationError> errorsToIgnore = new List<DbValidationError>();
                        foreach (DbValidationError error in result.ValidationErrors)
                        {
                            if (entityEntry.State == EntityState.Modified)
                            {
                                DbMemberEntry member = entityEntry.Member(error.PropertyName);
                                DbPropertyEntry property = member as DbPropertyEntry;
                                if (property != null)
                                {
                                    if (!property.IsModified)
                                    {
                                        errorsToIgnore.Add(error);
                                    }
                                }
                            }
                        }

                        errorsToIgnore.ForEach(e => result.ValidationErrors.Remove(e));
                    }
                }
            }

            return result;
        }

        public static string GetEntitySetName<T>(this DbContext context)
         where T : class
        {
            string className = typeof(T).Name;
            IObjectContextAdapter adapter = context as IObjectContextAdapter;

            //get context defaultmeta container
            EntityContainer container = adapter.ObjectContext.MetadataWorkspace.
                GetEntityContainer(adapter.ObjectContext.DefaultContainerName, 
                DataSpace.CSpace);

            string entitySetName = container.BaseEntitySets.
                First(setMetaData => setMetaData.ElementType.Name == className).Name;
            return entitySetName;
        }
    }
}