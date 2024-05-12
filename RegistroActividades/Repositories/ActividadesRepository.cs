using RegistroActividades.Models.Entities;

namespace RegistroActividades.Repositories
{
    public class ActividadesRepository
    {
        public ActividadesRepository(ItesrcneActividadesContext context)
        {
            Context = context;
        }

        public ItesrcneActividadesContext Context { get; }

        public IEnumerable<Actividades> GetAll()
        {
            return Context.Actividades.OrderBy(x => x.Titulo);
        }

        public Actividades? Get(int id)
        {
            return Context.Actividades.Find(id);
        }

        public void Insert(Actividades actividad)
        {
            Context.Actividades.Add(actividad);
            Context.SaveChanges();
        }

        public void Update(Actividades actividad)
        {
            Context.Actividades.Update(actividad);
            Context.SaveChanges();
        }

        public void Delete(Actividades actividad)
        {
            Context.Actividades.Remove(actividad);
            Context.SaveChanges();
        }
    }
}
