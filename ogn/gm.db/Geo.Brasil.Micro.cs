using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;

using gm.db.model;

namespace gm.Geo.Brasil
{
    public class Micro
    {

        /// <summary>
        /// CRUD - Listar
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>        
        public List<RegistroGeoBrasilMicro> Listar(short id_geo_micro = 0,
                                                   string micro_regiao = "",
                                                   short id_geo_meso = 0,
                                                   string uf = "",
                                                   string municipio = "")
        {
            try
            {
                var cnn = new gmEntities(true);
                var rst = from r in cnn.geo_brasil_micro
                          select new
                          {
                              r.id_geo_micro,
                              r.id_geo_meso,
                              r.micro_regiao,
                              r.geo_brasil_meso.uf,
                              r.poligono
                          };

                if (municipio != "" && uf != "" && id_geo_meso == 0)
                {
                    id_geo_micro = (short)cnn.geo_brasil_cidade
                                            .FirstOrDefault(c => c.uf == uf &&
                                                                 c.municipio == municipio &&
                                                                 c.categoria == "cidade")
                                            .id_geo_micro;
                }

                if (id_geo_micro != 0)
                {
                    rst = rst.Where("id_geo_micro = @0", id_geo_micro);
                }
                if (id_geo_meso != 0)
                {
                    rst = rst.Where("id_geo_meso = @0", id_geo_meso);
                }
                if (micro_regiao.Trim() != "")
                {
                    rst = rst.Where("micro_regiao.Contains(@0)", micro_regiao.Trim());
                }
                if (uf != "")
                {
                    rst = rst.Where("uf = @0", uf);
                }

                List<RegistroGeoBrasilMicro> oRetorno = new List<RegistroGeoBrasilMicro>();

                if (rst.Any())
                {
                    foreach (var r in rst)
                    {
                        var oReg = new RegistroGeoBrasilMicro();

                        oReg.id_geo_micro   = r.id_geo_micro;
                        oReg.id_geo_meso    = r.id_geo_meso;
                        oReg.uf             = r.uf;
                        oReg.micro_regiao   = r.micro_regiao;
                        oReg.poligono       = r.poligono;

                        oRetorno.Add(oReg);
                    }
                }

                return oRetorno;

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
            }
        }

    }
}
