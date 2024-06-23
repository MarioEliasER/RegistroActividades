using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Actividades>> GetAllActividadesPublicadasAsync(int? id, DateTime fecha)
        {
            List<Actividades> listactividades = new();

            List<Departamentos> hijos=new();

            var userActual = Context.Departamentos.Include(x=>x.InverseIdSuperiorNavigation).Include(x=>x.Actividades).Where(x => x.Id == id).First();

            foreach (var item in userActual.Actividades)
            {
                if (item.FechaActualizacion>fecha)
                {
                    listactividades.Add(item);
                }
            }
            hijos.AddRange(userActual.InverseIdSuperiorNavigation);
            while (hijos.Count != 0)
            {
                var hijes = hijos.ToList();
                hijos.Clear();
                foreach (var item in hijes)
                {
                    var dep = Context.Departamentos.Include(x=>x.Actividades).Include(x=>x.InverseIdSuperiorNavigation).First(x=>x.Id == item.Id);
                    foreach (var act in dep.Actividades)
                    {
                        if (act.FechaActualizacion > fecha)
                        {
                            listactividades.Add(act);
                        }
                    }
                    hijos.AddRange(dep.InverseIdSuperiorNavigation);
                }
            }
            return listactividades.Where(x=>x.FechaActualizacion>fecha &&( x.Estado == 1 || x.Estado == 2)).OrderByDescending(x=>x.FechaRealizacion);
        }

        public async Task<Actividades> GetIncludeDepa(int id)
        {
            return Context.Actividades.Include(x => x.IdDepartamentoNavigation).Where(x => x.Id == id).First();
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

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
