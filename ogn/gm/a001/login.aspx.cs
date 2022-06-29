using System;
using System.Web;
using System.Web.Services;
using System.Web.UI;

namespace gm.a001
{
    public partial class login : System.Web.UI.Page
    {
        // parâmetros enviados via GET
        private string gmModulo = "";
        private string gmMensagem = "";

        protected void Page_Load(object sender, EventArgs e)
        {

            if (IsPostBack) return;

            try
            {
                gmModulo = Request.QueryString["modulo"].ToString();
            }
            catch (Exception) { };

            try
            {
                gmMensagem = Request.QueryString["mensagem"].ToString();
            }
            catch (Exception) { };

            string urlOrigem = HttpContext.Current.Request.RawUrl == null ? "" : HttpContext.Current.Request.RawUrl;

            divParametros.InnerHtml = string.Format("<div id='paramModulo'>{0}</div>" +
                                                    "<div id='paramMensagem'>{1}</div>" +
                                                    "<div id='paramOrigem'>{2}</div>"
                                                    , gmModulo
                                                    , gmMensagem
                                                    , urlOrigem);

        }

        #region Alterar Senha

        /// <summary>
        /// Alterar Senha
        /// </summary>
        /// <param name="id_usuario"></param>
        /// <param name="senha_old"></param>
        /// <param name="senha_new"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public static object AlterarSenha(string senha_old, string senha_new)
        {
            try
            {
                short id_usuario = short.Parse(HttpContext.Current.Session["seg_id_usuario"].ToString());
                var seg = new Allupa.Seguranca.UsuarioDAL();
                var obj = seg.AlterarSenha(id_usuario, senha_old, senha_new);
                return obj;
            }
            catch (Exception ex)
            {
                return new { Result = "ERROR", Message = ex.Message };
            }
        }
        #endregion

        #region VALIDAÇÃO
        /// <summary>
        /// Validação de dados de acesso ao módulo rb_be
        /// </summary>
        /// <param name="u">Usuário</param>
        /// <param name="s">Senha</param>
        /// <param name="m">Módulo gm</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public static object ValidaDados(string u, string s, string m)
        {
            string dados = "";

            try
            {
                u = u.Trim().ToLower();
                s = s.Trim().ToLower();

                if (u.Length == 0) throw new System.InvalidOperationException("Usuário não informado.");
                if (s.Length == 0) throw new System.InvalidOperationException("Senha não informada.");

                // validar usuário e senha
                var oLogin = new gm.GlobalLogin(); // Login();
                {

                    var obj = new Allupa.Seguranca.PermissaoAcessoDAL();
                    var oSessao = new Allupa.Seguranca.VariaveisDeSessao();

                    string msg = oLogin.ValidaUsuarioSenha(m, u, s);

                    if (msg == "OK")
                    {

                        oSessao.seg_timeout = "20";

                        // informações que interessam apenas à redes
                        string seg_redes = "";
                        try
                        {
                            seg_redes = obj.ListarRedes(u); // lista de códigos de redes ou "all"
                            seg_redes = oLogin.FiltrarRedesAtivas(seg_redes);

                            oSessao.seg_redes = seg_redes;
                            oSessao.seg_nome_rede = oLogin.GetNomeRede(seg_redes);

                        }
                        catch (Exception) { }

                        oSessao.seg_usuario = u.ToLower();
                        oSessao.seg_modulo = m.ToLower();
                        oSessao.seg_modulos_permitidos = obj.ListarModulos(u, m);
                        oSessao.seg_id_usuario = obj.GetIdUsuario(u);
                        oSessao.seg_id_grupos = obj.GetIdUsuarioGrupos(u);
                        oSessao.administrador = obj.Administrador(u);

                        string plataforma_padrao = obj.GetPlataformaUsuario(u);
                        plataforma_padrao = plataforma_padrao == "" ? oLogin.DetectarPlataforma() : plataforma_padrao;
                        oSessao.seg_plataforma = plataforma_padrao;
                        oSessao.seg_tipo_usuario = obj.GetInformacoesDoUsuario(u)[0];
                        obj = null;

                        var oMenu = new Allupa.Seguranca.FuncionalidadeDAL();
                        oSessao.seg_menu = oMenu.ListarMenu(m, u);
                        if (oSessao.seg_menu.IndexOf("#raiz#") >= 0)
                        {
                            var raiz = (HttpContext.Current.Handler as Page).ResolveUrl("~/");
                            oSessao.seg_menu = oSessao.seg_menu.Replace("#raiz#", raiz);
                        }

                        oMenu = null;

                        oLogin.CriarVariaveisDeSessao(oSessao);
                        oLogin = null;

                    }
                    else
                    {
                        oLogin = null;
                        throw new System.InvalidOperationException(msg);
                    }

                }

                return new { Message = "OK", Dados = dados };

            }
            catch (Exception ex)
            {
                return new { Message = ex.Message, Dados = "" };
            }
        }

        #endregion

    }
}

