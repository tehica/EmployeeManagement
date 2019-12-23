using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();

            /*
                Na ovaj način provjeravamo u bazi imamo li povezane tablice kljucevima...
                ako imamo baza nam nece dopustiti da obrisemo redove u tablicama na koje se
                nadovezuju druge pod tablice

                primjer:
                po defaultu  Microsoft SQL Servera kada kreiramo tablice i povezemo ih kljucevima
                zadano je 'Cascading referential integrity contraint' (ON DELETE CASCADE) odnosno
                da kada pobrisemo red u parent tablici automatski ce se pobrisati i redovi u child
                tablicama koji su povezani s tim fk obrisanog reda

                u ovom slucaju s ovim update-om baze ne možemo pobrisati ROLU dok ne pobrižemo
                sve USERE iz te role i onda nakon što više nemamo ni jednog usera u roli možemo
                pobrisati i rolu
            */
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
        

    }
}
