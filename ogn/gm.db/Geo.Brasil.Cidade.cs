using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;

using gm.db.model;

namespace gm.Geo.Brasil
{
    public class Cidade
    {

        /// <summary>
        /// CRUD - Listar
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>        
        public List<RegistroGeoBrasilCidade> Listar(int id_cidade = 0,
                                                    string municipio = "",
                                                    short id_geo_micro = 0,
                                                    short id_geo_meso = 0,
                                                    string uf = "")
                                                    
        {
            try
            {
                var cnn = new gmEntities(true);
                var rst = (from r in cnn.geo_brasil_cidade
                           where r.categoria == "cidade"
                           select new
                           {
                               r.id_geo_cidade,
                               r.categoria,
                               r.uf,
                               r.municipio,
                               r.distrito,
                               r.populacao,
                               r.latitude,
                               r.longitude,
                               r.altitude,
                               r.cep,
                               r.poligono,
                               r.id_geo_meso,
                               r.id_geo_micro
                           });

                if (id_geo_micro != 0)
                {
                    rst = rst.Where("id_geo_micro = @0", id_geo_micro);
                }
                if (id_geo_meso != 0)
                {
                    rst = rst.Where("id_geo_meso = @0", id_geo_meso);
                }
                if (municipio != "")
                {
                    rst = rst.Where("municipio = @0", municipio);
                }
                if (uf != "")
                {
                    rst = rst.Where("uf = @0", uf);
                }

                List<RegistroGeoBrasilCidade> oRetorno = new List<RegistroGeoBrasilCidade>();

                if (rst.Any())
                {
                    foreach(var r in rst)
                    {
                        var oReg = new RegistroGeoBrasilCidade();

                        oReg.id_geo_cidade  = r.id_geo_cidade;
                        oReg.categoria      = r.categoria;
                        oReg.uf             = r.uf;
                        oReg.municipio      = r.municipio;
                        oReg.distrito       = r.distrito;
                        oReg.populacao      = r.populacao;
                        oReg.latitude       = r.latitude;
                        oReg.longitude      = r.longitude;
                        oReg.altitude       = r.altitude;
                        oReg.cep            = r.cep;
                        oReg.poligono       = r.poligono;
                        oReg.id_geo_meso    = r.id_geo_meso;
                        oReg.id_geo_micro   = r.id_geo_micro;

                        oRetorno.Add(oReg);

                    }
                }

                return oRetorno;

                //return new { Result = "OK", Records = rst, TotalRecordCount = nTotal };

            }
            catch (Exception ex)
            {
                string msgErro = ex.Message;

                try
                {
                    msgErro = ex.InnerException.Message;
                }
                catch (Exception) { }

                throw new System.InvalidOperationException(msgErro);
                //return new { Result = "ERROR", Message = msgErro };
            }
        }

    }
}
