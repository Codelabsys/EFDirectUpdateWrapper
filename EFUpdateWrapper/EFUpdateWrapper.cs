using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodeLab.Assets.EFUpdateWrapper
{
    public class EntityUpdateWrapper<TEntity>
           where TEntity : class, new()
    {

        DbContext context = null;
        DbEntityEntry<TEntity> entry = null;
        protected DbSet<TEntity> EntitySet = null;
        IEnumerable<string> allProps = null;

        public EntityUpdateWrapper(TEntity entity, DbContext dbcontext, DbSet<TEntity> dbEntitySet)
        {

            EntitySet = dbEntitySet;
            context = dbcontext;

            if (!context.GetLoadedEntityIfAny(ref entity))
            {
                //now safe to attach since it is either not loaded , or detached
                EntitySet.Attach(entity);
            }
            entry = context.Entry(entity);
            allProps = entry.CurrentValues.PropertyNames;

            //set all properties to not modified so that 
            //they will not be included in the generated update statement
            foreach (string prop in allProps)
            {
                entry.Property(prop).IsModified = false;
            }
            entry.State = EntityState.Unchanged;

        }

        public void Update<TProperty>(Expression<Func<TEntity, TProperty>> propSelector, TProperty value)
        {
            entry.Property(propSelector).CurrentValue = value;
            entry.Property(propSelector).IsModified = true;
        }

        //public void Save()
        //{ 
        //    try
        //    {
        //        context.SaveChanges();
                
        //    }
        //    catch (DbEntityValidationException ex)
        //    {

        //    }
        //}

    }
}
