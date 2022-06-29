/*
 * Teles: Conexão Dinâmica com banco de dados Geográfico
 */

namespace gm.db.model
{
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class gmEntities : DbContext
    {
        private static string _conexao = ConfigurationManager.AppSettings["gmEntities"];

        public gmEntities(bool appSetting) : base(_conexao)
        {
        }
    }

}

