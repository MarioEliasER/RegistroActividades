using Microsoft.EntityFrameworkCore;
using RegistroActividades.Models.Entities;

namespace RegistroActividades.Repositories
{
    public class DepartamentosRepository
    {
        public DepartamentosRepository(ItesrcneActividadesContext context)
        {
            Context = context;
        }

        public ItesrcneActividadesContext Context { get; }

        public IEnumerable<Departamentos> GetAll()
        {
            return Context.Departamentos.OrderBy(x => x.Nombre);
        }

        public Departamentos? Get(int id)
        {
            return Context.Departamentos.Find(id);
        }

        public void Insert(Departamentos departamento)
        {
            Context.Departamentos.Add(departamento);
            Context.SaveChanges();
        }

        public void Update(Departamentos actividad)
        {
            Context.Departamentos.Update(actividad);
            Context.SaveChanges();
        }

        public void Delete(Departamentos actividad)
        {
            Context.Departamentos.Remove(actividad);
            Context.SaveChanges();
        }

        public IEnumerable<Departamentos> GetSubdepartamentos(int id)
        {
            return Context.Departamentos.Include(x => x.IdSuperiorNavigation).Where(x => x.IdSuperior == id);
        }

        public async Task<Departamentos?> GetIncludeActividades(int id)
        {
            return Context.Departamentos.Include(x => x.Actividades).Include(x => x.InverseIdSuperiorNavigation).FirstOrDefault(x => x.Id == id);
        }

        public void DeleteDepartment(int id)
        {
            var depa = Context.Departamentos.Include(x => x.Actividades).Include(x => x.InverseIdSuperiorNavigation).FirstOrDefault(x => x.Id == id);
            if (depa != null)
            {
                foreach (var item in depa.Actividades.ToList())
                {
                    var entity = Context.Actividades.Find(item.Id);
                    if (entity != null)
                    {
                        Context.Remove(entity);
                        Context.SaveChanges();
                    }
                }
                foreach (var item in depa.InverseIdSuperiorNavigation.ToList())
                {
                    var entity = Context.Departamentos.Find(item.Id);
                    if (entity != null)
                    {
                        entity.IdSuperior = depa.IdSuperior;
                        Context.Update(entity);
                        Context.SaveChanges();
                    }
                }
                var departamento = Context.Departamentos.Find(id);
                if (departamento != null)
                {
                    Context.Remove(departamento);
                }
                Context.SaveChanges();

            }
        }
    }
}
