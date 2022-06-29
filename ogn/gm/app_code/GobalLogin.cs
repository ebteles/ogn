using System;
using System.Web;


/// <summary>
/// Summary description for login
/// </summary>
/// 

namespace gm
{
    public class GlobalLogin
    {

        public void CriarVariaveisDeSessao(Allupa.Seguranca.VariaveisDeSessao oSessao)
        {

            HttpContext.Current.Session["seg_erro"] = oSessao.seg_erro;
            HttpContext.Current.Session["seg_id_grupos"] = oSessao.seg_id_grupos;
            HttpContext.Current.Session["seg_id_usuario"] = oSessao.seg_id_usuario;
            HttpContext.Current.Session["seg_menu"] = oSessao.seg_menu;
            HttpContext.Current.Session["seg_modulo"] = oSessao.seg_modulo;
            HttpContext.Current.Session["seg_modulos_permitidos"] = oSessao.seg_modulos_permitidos;
            HttpContext.Current.Session["seg_nome_rede"] = oSessao.seg_nome_rede;
            HttpContext.Current.Session["seg_plataforma"] = oSessao.seg_plataforma;
            HttpContext.Current.Session["seg_redes"] = oSessao.seg_redes;
            HttpContext.Current.Session["seg_timeout"] = oSessao.seg_timeout;
            HttpContext.Current.Session["seg_tipo_usuario"] = oSessao.seg_tipo_usuario;
            HttpContext.Current.Session["seg_usuario"] = oSessao.seg_usuario;
            HttpContext.Current.Session["seg_administrador"] = oSessao.administrador;

            HttpContext.Current.Session["seg_filtro_data_movimento"] = oSessao.filtro_data_movimento;
            HttpContext.Current.Session["seg_filtro_codigo_rede"] = oSessao.filtro_codigo_rede;
            HttpContext.Current.Session["seg_filtro_codigo_loja"] = oSessao.filtro_codigo_loja;
            HttpContext.Current.Session["seg_filtro_codigo_abastecedor"] = oSessao.filtro_codigo_abastecedor;

        }

        public string ListarMenu(string sistema = "",
                                 string nome_usuario = "")
        {
            var html = "";

            if (nome_usuario == "")
            {
                html = HttpContext.Current.Session["seg_menu"].ToString(); // "<nav style='height:100px;z-index: 1000;'><ul><li100>&nbsp;</li></ul></nav>";
            }
            else
            {
                sistema += HttpContext.Current.Session["seg_plataforma"].ToString() == "M"
                           ? "_mobile"
                           : "";

                var obj = new Allupa.Seguranca.FuncionalidadeDAL();
                html = obj.ListarMenu(sistema, nome_usuario);
                obj = null;

            }

            return html;

        }

        /// <summary>
        /// Identifica a plataforma operacional do usuÃ¡rio
        /// </summary>
        /// <returns>D ou M para Desktop e Mobile</returns>
        public string DetectarPlataforma()
        {
            string plataforma = "D"; // desktop

            HttpRequest _request = HttpContext.Current.Request;
            if (_request.Browser.IsMobileDevice)
            {
                plataforma = "M";
            }
            return plataforma;
        }

        /// <summary>
        /// Validar usuário e senha, passando informações de navegação do usuário para gravação
        /// da trilha de auditoria de acesso.
        /// </summary>
        /// <param name="sistema"></param>
        /// <param name="usuario"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        public string ValidaUsuarioSenha(string sistema, string usuario, string senha)
        {
            string numero_ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            string navegador = HttpContext.Current.Request.Browser.Browser;
            string navegador_versao = HttpContext.Current.Request.Browser.Version;
            string plataforma = HttpContext.Current.Request.Browser.Platform;

            var obj = new Allupa.Seguranca.PermissaoAcessoDAL();

            string mensagem = obj.ValidarLogin(sistema,
                                               usuario,
                                               senha,
                                               numero_ip,
                                               navegador,
                                               navegador_versao,
                                               plataforma);
            return mensagem;
        }

        /// <summary>
        /// Filtragem de redes. Retorna string contendo código das redes ativas
        /// </summary>
        /// <param name="listaRedes"></param>
        /// <returns></returns>
        public string FiltrarRedesAtivas(string listaRedes)
        {
            string redesAtivas = "";
            //string[] redes = listaRedes.Split(',');

            //Allupa.Sistema.Parametro.RedeDAL o = new Allupa.Sistema.Parametro.RedeDAL();
            //List<Allupa.Sistema.RegistroDescricaoValorString> oLista = o.ListarRedesAtivas();

            //foreach (string rede in redes)
            //{
            //    if (oLista.Any(l => l.Valor == rede))
            //    {
            //        redesAtivas += "," + rede;
            //    }
            //}

            //if (redesAtivas.Length > 0)
            //{
            //    redesAtivas = redesAtivas.Substring(1);
            //}

            return redesAtivas;
        }

        /// <summary>
        /// Obter nome da rede. Para usuários internos com acesso a mais de uma rede, assume nome da rede de código -1
        /// </summary>
        /// <param name="seg_redes">Lista de códigos de rede</param>
        /// <returns></returns>
        public string GetNomeRede(string seg_redes)
        {
            //try
            //{
            //    if (seg_redes.Trim() == "")
            //    {
            //        string msgErro = "Usuário/Grupo sem permissões de " +
            //                         "acesso a dados de rede. É preciso " +
            //                         "associar o grupo de acesso a uma ou " +
            //                         "mais redes de varejo.";
            //        throw new System.InvalidOperationException(msgErro);
            //    }
            //    if (seg_redes.IndexOf(",") > 0)
            //    {
            //        seg_redes = "-1";
            //    }
            //    short codigo_rede = short.Parse(seg_redes);

            //    var obj = new Allupa.Sistema.Cadastro.RedeDAL();
            //    string nome_rede = obj.ListarNomeRede(codigo_rede);
            //    obj = null;

            //    return nome_rede;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return "";
        }
    }
}