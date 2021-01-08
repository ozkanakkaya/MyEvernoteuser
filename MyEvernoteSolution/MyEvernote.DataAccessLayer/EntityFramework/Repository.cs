using MyEvernote.Common;
using MyEvernote.Core.DataAccess;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class Repository<T> :RepositoryBase, IDataAccess<T> where T : class // T class (new lenen bir nesne)olmak zorundadır.
        //RepositoryBase den miras ayarak "context" oluşur. Sonuç olarak Singleton Pattern ile bir database oluşturulmuş olur ve tüm metodlarda bu db kullanılır.
    {
        //private DatabaseContext db;

        private DbSet<T> _objectSet;

        public Repository()
        {
            _objectSet = context.Set<T>();
        }

        public List<T> List()
        {
            return _objectSet.ToList();
        }

        public IQueryable<T> ListQueryable()//database de sorgulama yapacak
        {
            return _objectSet.AsQueryable<T>();// IQueryable dönecek. OrderByladığımızda ne zaman to list metodunu çağırırsak o zaman sql e giriyor olacak.
        }

        public List<T> List(Expression<Func<T, bool>> where)
        {
            return _objectSet.Where(where).ToList();
        }

        public int Insert(T obj)
        {
            _objectSet.Add(obj);

            if (obj is MyEntityBase)
            {
                MyEntityBase o = obj as MyEntityBase;
                DateTime dateTimeNow = DateTime.Now;

                o.CreatedOn = dateTimeNow;
                o.ModifiedOn = dateTimeNow;
                o.ModifiedUsername = App.Common.GetCurrentUsername(); // TODO : işlem yapan kullanıcı adı yazılmalı...
            }

            return Save();
        }

        public int Update(T obj)
        {
            if (obj is MyEntityBase)
            {
                MyEntityBase o = obj as MyEntityBase;

                o.ModifiedOn = DateTime.Now;
                o.ModifiedUsername = App.Common.GetCurrentUsername(); // TODO : işlem yapan kullanıcı adı yazılmalı...
            }
            return Save();
        }

        public int Delete(T obj)
        {
            //if (obj is MyEntityBase)
            //{
            //    MyEntityBase o = obj as MyEntityBase;

            //    o.ModifiedOn = DateTime.Now;
            //    o.ModifiedUsername = App.Common.GetUsername(); // TODO : işlem yapan kullanıcı adı yazılmalı...
            //
            //Silme işlemini yapmayıp MyEntityBase de "IsRemoved" adında bir property oluşturup buna false vererek hala sistemde tutabilir ve listelemek istediğimizde de IsRomoved=true/false olan kayıtları çağırabiliriz.
            //}

            _objectSet.Remove(obj);
            return Save();
        }

        public int Save()
        {
            return context.SaveChanges();
        }

        public T Find(Expression<Func<T, bool>> where)//geriye tek bir tür döndürür yukarıdaki listeden farklı olarak
        {
            return _objectSet.FirstOrDefault(where);
        }

    }
}
