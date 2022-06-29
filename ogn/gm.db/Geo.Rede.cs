using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;

using Allupa.Sistema.Banco;

namespace gm.Geo
{
    public class Rede
    {
        /// <summary>
        /// Retorna Informações Geográfica das Lojas de uma determinada rede
        /// </summary>
        /// <returns></returns>
        public List<RegistroGoogleMapsMarcas>ListaLojas(short codigo_rede = 0,
                                                        string codigo_loja = "")
        {
            try
            {
                //var cnn = new gmEntities(codigo_rede);
                var cnn = new sistemaEntities(true);
                var rst = from r in cnn.rede_loja
                          join p in cnn.parametro_tipo_loja on r.tipo_loja equals p.tipo_loja
                          select new
                          {
                              r.codigo_rede,
                              r.codigo_loja,
                              r.nome_loja,
                              r.endereco_loja,
                              r.cep_loja,
                              r.latitude,
                              r.longitude,
                              r.tipo_loja,
                              p.descricao_tipo_loja
                          };

                if (codigo_rede != 0)
                {
                    rst = rst.Where("codigo_rede = @0", codigo_rede);
                }
                if (codigo_loja.Trim() != "")
                {
                    rst = rst.Where("codigo_loja = @0", codigo_loja.Trim());
                }

                List<RegistroGoogleMapsMarcas> oRetorno = new List<RegistroGoogleMapsMarcas>();

                if (rst.Any())
                {
                    foreach (var r in rst)
                    {
                        var oReg = new RegistroGoogleMapsMarcas();

                        object oComplemento = new {
                            codigo_rede = r.codigo_rede,
                            codigo_loja = r.codigo_loja,
                            tipo_loja   = r.descricao_tipo_loja
                        };

                        oReg.id_marca = r.nome_loja;
                        oReg.latitude = r.latitude;
                        oReg.longitude = r.longitude;
                        oReg.icone = r.tipo_loja == 0
                                        ? "gm_pdv_32_verde"
                                        : r.tipo_loja == 1
                                            ? "gm_pdv_32_branco"
                                            : "gm_pdv_32_vermelho";
                        oReg.complemento = oComplemento;
                  
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
                
            }
        }
    }
}
