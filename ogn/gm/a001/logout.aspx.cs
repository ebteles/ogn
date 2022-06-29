using System;
using System.Web;

namespace gm.a001
{
    public partial class logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string mensagem = "";
                
                try
                {
                    mensagem = Request.QueryString["mensagem"].ToString();
                }
                catch (Exception) { }

                RegistrarLogout();

                //limpar variáveis de sessão
                Session.RemoveAll();
                Session.Clear();

                // posicionar-se na página de login
                //Response.Redirect("login.aspx?mensagem=" + mensagem, false);
                Response.Redirect("default.aspx", false);

                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }

        /// <summary>
        /// Registrar Logout em Tabela
        /// </summary>
        private void RegistrarLogout()
        {
            short id_usuario = 0;

            try
            {
                id_usuario = short.Parse(Session["seg_id_usuario"].ToString());
            }
            catch (Exception) { }
            finally
            {
                if (id_usuario > 0)
                {
                    string nome_sistema = "gm";

                    var obj = new Allupa.Seguranca.LoginLogoutDAL();
                    obj.Atualizar(id_usuario, nome_sistema);
                    obj = null;
                }
            }
        }

    }
}

