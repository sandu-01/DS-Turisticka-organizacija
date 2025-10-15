using System;
using System.Collections.Generic;
using System.Data.Common;
using TurističkaOrganizacija.Application;
using TurističkaOrganizacija.Backend;
using TurističkaOrganizacija.Domain;

namespace TurističkaOrganizacija.Infrastructure.Repositories.SqlClient
{
    public class PackageRepositorySql : IPackageRepository
    {
        public TravelPackage GetById(int id)
        {
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM paketi WHERE idPaketa=@id";
                    AddParam(cmd, "@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        return MapPackage(reader);
                    }
                }
            }
        }

        public IEnumerable<TravelPackage> GetAll(PackageType? type = null)
        {
            var list = new List<TravelPackage>();
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    if (type.HasValue)
                    {
                        cmd.CommandText = "SELECT * FROM paketi WHERE vrstaPak=@t";
                        AddParam(cmd, "@t", ToDbType(type.Value));
                    }
                    else
                    {
                        cmd.CommandText = "SELECT * FROM paketi";
                    }
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read()) list.Add(MapPackage(r));
                    }
                }
            }
            return list;
        }

        public void Add(TravelPackage package)
        {
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO paketi(nazivPak,cena,vrstaPak,destinacija,tipPrevoza,vrstaSmestaja,dodatneAktivnosti,vodic,trajanje,brod,ruta,datumPolaska,tipKabine) " +
                        "VALUES(@naziv,@cena,@vrsta,@dest,@prevoz,@smestaj,@aktiv,@vodic,@trajanje,@brod,@ruta,@datum,@kabina)";
                    BindParams(cmd, package);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(TravelPackage package)
        {
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        "UPDATE paketi SET nazivPak=@naziv,cena=@cena,vrstaPak=@vrsta,destinacija=@dest,tipPrevoza=@prevoz,vrstaSmestaja=@smestaj,dodatneAktivnosti=@aktiv,vodic=@vodic,trajanje=@trajanje,brod=@brod,ruta=@ruta,datumPolaska=@datum,tipKabine=@kabina WHERE idPaketa=@id";
                    BindParams(cmd, package);
                    AddParam(cmd, "@id", package.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM paketi WHERE idPaketa=@id";
                    AddParam(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void BindParams(DbCommand cmd, TravelPackage package)
        {
            AddParam(cmd, "@naziv", package.Name);
            AddParam(cmd, "@cena", package.Price);
            AddParam(cmd, "@vrsta", ToDbType(package.Type));
            AddParam(cmd, "@dest", package.Destination);
            AddParam(cmd, "@prevoz", package.TransportType);
            AddParam(cmd, "@smestaj", package.AccommodationType);
            var mountainPackage = package as MountainPackage;
            string aktivnosti = (mountainPackage != null && mountainPackage.Activities != null) ? string.Join(",", mountainPackage.Activities) : null;
            AddParam(cmd, "@aktiv", aktivnosti);
            AddParam(cmd, "@vodic", string.IsNullOrEmpty(package.GuideName) ? null : "da");
            // DB column 'trajanje' is TIME; map days -> TimeSpan
            AddParam(cmd, "@trajanje", package.DurationDays.HasValue ? (object)TimeSpan.FromDays(package.DurationDays.Value) : DBNull.Value);
            AddParam(cmd, "@brod", package.ShipName);
            AddParam(cmd, "@ruta", package.Route);
            AddParam(cmd, "@datum", package.DepartureDate.HasValue ? (object)package.DepartureDate.Value.Date : DBNull.Value);
            AddParam(cmd, "@kabina", package.CabinType);
        }

        private static string ToDbType(PackageType type)
        {
            switch (type)
            {
                case PackageType.Sea: return "more";
                case PackageType.Mountain: return "planina";
                case PackageType.Excursion: return "ekskurzija";
                case PackageType.Cruise: return "krstarenje";
                default: return null;
            }
        }

        private static TravelPackage MapPackage(DbDataReader r)
        {
            string vrsta = r["vrstaPak"].ToString();
            TravelPackage p;
            switch (vrsta)
            {
                case "more": p = new SeaPackage(); break;
                case "planina": p = new MountainPackage(); break;
                case "ekskurzija": p = new ExcursionPackage(); break;
                case "krstarenje": p = new CruisePackage(); break;
                default: p = new SeaPackage(); break;
            }
            p.Id = Convert.ToInt32(r["idPaketa"]);
            p.Name = r["nazivPak"].ToString();
            p.Price = Convert.ToDecimal(r["cena"]);
            p.Destination = r["destinacija"] as string;
            p.TransportType = r["tipPrevoza"] as string;
            p.AccommodationType = r["vrstaSmestaja"] as string;
            var mountainPackage = p as MountainPackage;
            if (mountainPackage != null && r["dodatneAktivnosti"] != DBNull.Value)
            {
                string s = r["dodatneAktivnosti"].ToString();
                mountainPackage.Activities = new List<string>(s.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries));
            }
            p.GuideName = (r["vodic"] != DBNull.Value && r["vodic"].ToString() == "da") ? "Vodic" : null;
            if (r["trajanje"] != DBNull.Value)
            {
                p.DurationDays = Convert.ToInt32(r["trajanje"]); ;
            }
            p.ShipName = r["brod"] as string;
            p.Route = r["ruta"] as string;
            if (r["datumPolaska"] != DBNull.Value)
                p.DepartureDate = Convert.ToDateTime(r["datumPolaska"]);
            p.CabinType = r["tipKabine"] as string;
            return p;
        }

        private static void AddParam(DbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}


