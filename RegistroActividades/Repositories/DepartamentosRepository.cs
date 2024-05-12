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
    }
}
