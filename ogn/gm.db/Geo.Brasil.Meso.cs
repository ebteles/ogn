using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;

using gm.db.model;

namespace gm.Geo.Brasil
{
    public class Meso
    {

        /// <summary>
        /// CRUD - Listar
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>        
        public List<RegistroGeoBrasilMeso> Listar(short id_geo_meso = 0,
                                                  string meso_regiao = "",
                                                  string uf = "",
                                                  string municipio = "" )
        {
            try
            {
                var cnn = new gmEntities(true);
                var rst = from r in cnn.geo_brasil_meso
                          select new
                          {
                              r.id_geo_meso,
                              r.meso_regiao,
                              r.uf,
                              r.poligono
                          };

                if(municipio != ""  && uf != "" && id_geo_meso == 0)
                {
                    id_geo_meso = (short)cnn.geo_brasil_cidade
                                            .FirstOrDefault(c => c.uf == uf && 
                                                                 c.municipio == municipio && 
                                                                 c.categoria == "cidade")
                                            .id_geo_meso;
                }
                
                if (id_geo_meso != 0)
                {
                    rst = rst.Where("id_geo_meso = @0", id_geo_meso);
                }
                if (meso_regiao.Trim() != "")
                {
                    rst = rst.Where("meso_regiao.Contains(@0)", meso_regiao.Trim());
                }
                if (uf != "")
                {
                    rst = rst.Where("uf = @0", uf);
                }

                List<RegistroGeoBrasilMeso> oRetorno = new List<RegistroGeoBrasilMeso>();
                
                if (rst.Any())
                {
                    foreach (var r in rst)
                    {
                        var oReg = new RegistroGeoBrasilMeso();

                        oReg.id_geo_meso    = r.id_geo_meso;
                        oReg.uf             = r.uf;
                        oReg.meso_regiao    = r.meso_regiao;
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
