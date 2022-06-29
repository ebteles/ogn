using System;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Procedimentos de inicialização de páginas. Executados em todas as páginas.
/// </summary>
/// 
namespace gm
{
    public class GlobalPages
    {
        public GlobalPages()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public class PageParametros
        {
            [ScriptIgnore]
            public DateTime data_movimento { get; set; }
            public string data_movimento_txt { get; set; }
            public string nome_dia_semana { get; set; }
            public short codigo_rede { get; set; }
            public string codigo_loja { get; set; }
            public string codigo_abastecedor { get; set; }
            public string codigo_hierarquia_nivel { get; set; }
            public string codigo_hierarquia_item { get; set; }
            public string codigo_produto { get; set; }
            public string periodo_oficial_inicio { get; set; }
            public string periodo_oficial_fim { get; set; }
            public string navegador { get; set; }
            public string lista_redes { get; set; }
            public string lista_lojas { get; set; }
            public string lista_abastecedores { get; set; }
            public string lista_nivel_hierarquia_produto { get; set; }
            public string lista_hierarquia_produto { get; set; }
            public string outros { get; set; }
            public string outros2 { get; set; }
            public string outros3 { get; set; }
            public PageParametros()
            {
                data_movimento = DateTime.MinValue;
                data_movimento_txt = "";
                nome_dia_semana = "";
                codigo_rede = 0;
                codigo_loja = "";
                codigo_abastecedor = "";
                codigo_hierarquia_nivel = "";
                codigo_hierarquia_item = "";
                codigo_produto = "";
                periodo_oficial_inicio = "";
                periodo_oficial_fim = "";
                navegador = "";
                lista_redes = "";
                lista_lojas = "";
                lista_abastecedores = "";
                lista_nivel_hierarquia_produto = "";
                lista_hierarquia_produto = "";
                outros = "";
                outros2 = "";
                outros3 = "";
            }
        }

        public string SerializePageParametros(gm.GlobalPages.PageParametros oParametros)
        {
            string retorno = new JavaScriptSerializer().Serialize(oParametros);
            return retorno;
        }

        /// <summary>
        /// Inicialização de variáveis (sessão)
        /// </summary>
        public void InicializarVariaveis(ref gm.GlobalPages.PageParametros oParametros)
        {
            short codigo_rede = 0;
            string codigo_loja = "";
            string codigo_abastecedor = "";
            string codigo_hierarquia_nivel = "";
            string codigo_hierarquia_item = "";
            string codigo_produto = "";

            DateTime data_movimento = DateTime.MinValue;
            DateTime periodo_oficial_inicio = DateTime.MinValue;
            DateTime periodo_oficial_fim = DateTime.MinValue;

            string msgErro = "";
            string data = null;

            var oContexto = HttpContext.Current.Request;

            // recuperar parâmetros passados via querystring:
            try
            {

                data = oContexto.QueryString["data_movimento"].ToString();

                if (data.Length == 10)
                {
                    data_movimento = new DateTime(
                        int.Parse(data.Substring(6, 4)),
                        int.Parse(data.Substring(3, 2)),
                        int.Parse(data.Substring(0, 2))
                      );
                }

                codigo_rede = short.Parse(oContexto.QueryString["codigo_rede"].ToString());
                codigo_loja = oContexto.QueryString["codigo_loja"].ToString();
                codigo_abastecedor = oContexto.QueryString["codigo_abastecedor"].ToString();

                // parâmetros opcionais em algumas páginas
                try {
                    codigo_hierarquia_nivel = oContexto.QueryString["codigo_hierarquia_nivel"].ToString();
                    codigo_hierarquia_item = oContexto.QueryString["codigo_hierarquia_item"].ToString();
                    codigo_produto = oContexto.QueryString["codigo_produto"].ToString();
                }
                catch (Exception) { }//

                InicializarPeriodo(codigo_rede, ref periodo_oficial_inicio, ref periodo_oficial_fim, ref data_movimento);

            }

            // recuperar parâmetros de sessão ou inicializar parâmetros:
            catch (Exception)
            {

                // recuperar de variáveis de sessão:
                try
                {

                    if (data == "") // reinicialiar (gerar erro)
                    {
                        throw new System.InvalidOperationException("forçar reinicialização");
                    }
                    data = HttpContext.Current.Session["seg_filtro_data_movimento"].ToString();
                    data_movimento = new DateTime(
                        int.Parse(data.Substring(6, 4)),
                        int.Parse(data.Substring(3, 2)),
                        int.Parse(data.Substring(0, 2))
                    );

                    codigo_rede = short.Parse(HttpContext.Current.Session["seg_filtro_codigo_rede"].ToString());
                    codigo_loja = HttpContext.Current.Session["seg_filtro_codigo_loja"].ToString();
                    codigo_abastecedor = HttpContext.Current.Session["seg_filtro_codigo_abastecedor"].ToString();
                    
                    InicializarPeriodo(codigo_rede, ref periodo_oficial_inicio, ref periodo_oficial_fim, ref data_movimento);

                }
                catch (Exception)
                {
                    try
                    {
                        string redes = HttpContext.Current.Session["seg_redes"].ToString();
                        codigo_rede = short.Parse(redes.Split(',')[0]);

                        InicializarPeriodo(codigo_rede, ref periodo_oficial_inicio, ref periodo_oficial_fim, ref data_movimento);
                    }
                    catch (Exception)
                    {
                        msgErro = "Não foi possível recuperar o Código da Rede. Processo Interrompido";
                        throw new System.InvalidOperationException(msgErro);
                    }
                }
            }

            // data de movimento
            if (data_movimento == DateTime.MinValue)
            {
                data_movimento = periodo_oficial_fim;
            }

            // filtros
            HttpContext.Current.Session["seg_filtro_data_movimento"] = data_movimento;
            HttpContext.Current.Session["seg_filtro_codigo_rede"] = codigo_rede;
            HttpContext.Current.Session["seg_filtro_codigo_loja"] = codigo_loja;
            HttpContext.Current.Session["seg_filtro_codigo_abastecedor"] = codigo_abastecedor;

            // retorno
            oParametros.data_movimento = data_movimento;
            oParametros.data_movimento_txt = string.Format("{0:dd/MM/yyyy}", data_movimento);
            oParametros.nome_dia_semana = GetNomeDiaSemana(data_movimento);
            oParametros.codigo_rede = codigo_rede; // codigo_rede.ToString();
            oParametros.codigo_loja = codigo_loja;
            oParametros.codigo_abastecedor = codigo_abastecedor;
            oParametros.codigo_hierarquia_nivel = codigo_hierarquia_nivel;
            oParametros.codigo_hierarquia_item = codigo_hierarquia_item;
            oParametros.codigo_produto = codigo_produto;
            oParametros.periodo_oficial_inicio = string.Format("{0:dd/MM/yyyy}", periodo_oficial_inicio);
            oParametros.periodo_oficial_fim = string.Format("{0:dd/MM/yyyy}", periodo_oficial_fim);
            oParametros.navegador = oContexto.Browser.Browser;

            //listas de redes, lojas e abastecedores
            //var o = new Allupa.Sistema.Filtro();

            //string lista_redes = HttpContext.Current.Session["seg_redes"].ToString();
            //if (lista_redes != "")
            //{
            //    lista_redes = o.FiltroRedeLista(lista_redes);
            //}
            //string lista_lojas = o.FiltroRedeLojaLista(codigo_rede, 0);
            //string lista_abastecedores = o.FiltroRedeAbastecedorLista(codigo_rede);
            //string lista_nivel_hierarquia_produto = o.FiltroRedeHierarquiaProdutoNiveisHTML(codigo_rede, "cboFiltroHierarquiaNivel");

            string lista_redes = "";
            string lista_lojas = "";
            string lista_abastecedores = "";
            string lista_nivel_hierarquia_produto = "";

            oParametros.lista_redes = lista_redes;
            oParametros.lista_lojas = lista_lojas;
            oParametros.lista_nivel_hierarquia_produto = lista_nivel_hierarquia_produto.Replace("class=\"lupa2\">", "style=\"width:179px;color:#707070\" onchange=\"carregarFiltroHierarquiaProdutoItem()\"><option value=\"0\"></option>");
            oParametros.lista_abastecedores = lista_abastecedores;

        }

        /// <summary>
        /// Período oficial de 7 semanas, com base no último processamento de dados para a rede
        /// </summary>
        /// <param name="codigo_rede"></param>
        /// <param name="periodo_oficial_inicio"></param>
        /// <param name="periodo_oficial_fim"></param>
        private void InicializarPeriodo(short codigo_rede,
                                        ref DateTime periodo_oficial_inicio,
                                        ref DateTime periodo_oficial_fim,
                                        ref DateTime data_movimento)
        {
            //var cnn = new Allupa.Sistema.Painel.EtlRedeDAL();
            //DateTime data_movimento_oficial = cnn.GetDataMovimento(codigo_rede, "P2");
            //cnn = null;

            //periodo_oficial_inicio = data_movimento_oficial.AddDays(-48);
            //periodo_oficial_fim = data_movimento_oficial;

            //// quando ocorre mudança de rede, a data deve ser revista, a menos que, por coincidência, seja uma
            //// data válida!
            //try
            //{
            //    if (data_movimento < periodo_oficial_inicio || data_movimento > periodo_oficial_fim)
            //        data_movimento = periodo_oficial_fim;
            //}
            //catch (Exception) { }

        }

        private string GetNomeDiaSemana(DateTime data)
        {
            int nDiaSemana = (int)data.DayOfWeek;
            string[] arrDias = { "Domingo", "Segunda-Feira", "Terça-Feira", "Quarta-Feira", "Quinta-Feira", "Sexta-Feira", "Sábado" };
            return arrDias[nDiaSemana];
        }

    }
}