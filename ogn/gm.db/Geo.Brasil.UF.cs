using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;

using gm.db.model;

namespace gm.Geo.Brasil
{
    public class UF
    {

        /// <summary>
        /// CRUD - Listar
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>        
        public List<RegistroGeoBrasilUF> Listar(string uf)
        {
            try
            {
                var cnn = new gmEntities(true);
                var rst = from r in cnn.geo_brasil_uf
                          select new
                          {
                              r.uf,
                              r.descricao,
                              r.poligono
                          };


                if (uf != "")
                {
                    rst = rst.Where("uf = @0", uf);
                }

                List<RegistroGeoBrasilUF> oRetorno = new List<RegistroGeoBrasilUF>();

                if (rst.Any())
                {
                    foreach (var r in rst)
                    {
                        var oReg = new RegistroGeoBrasilUF();

                        oReg.uf = r.uf;
                        oReg.descricao = r.descricao;
                        oReg.poligono = r.poligono;
                      
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
