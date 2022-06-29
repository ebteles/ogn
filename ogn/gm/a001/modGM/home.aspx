<%@ Page Title="" Language="C#" MasterPageFile="~/mp.Master" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="gm.a001.modGM.home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <link href="../../Script/gm/gm.css" rel="stylesheet" />
 
    <style type="text/css">
        .gmnoprint a, .gmnoprint span {     
            display: none;
        }

        .map_box_info {
            position: fixed;
            width: 300px;
            height: 430px;
            z-index: 999;
            cursor: pointer;
            right: 5px;
            top: 160px;
            padding-top: 20px;
            overflow: hidden;
            border: 1px solid #F1F1F1;
            background-color: rgba(248,248,248,0.8) !important;
            text-align: left;
            color: black;
            -moz-box-shadow: 0 0 10px #505050;
            -webkit-box-shadow: 0 0 10px #505050;
            box-shadow: 0 0 10px #505050;
            padding: 0px 0px 0px 0px;
        }

            .map_box_info span {
                display: block;
                width: 100%;
                height: 30px;
                font-size: 18px;
                padding: 7px 5px 0px 5px;
                background-color: #2A6BA6;
                color: #FFFFFF;
            }

            .map_box_info div {
                padding: 0px 5px 0px 5px;
                height: 390px;
                overflow-y: scroll;
            }

        .map_rd_opc_analise {
            width: 170px;
            font-weight: normal !important;
            font-size: 16px;
        }

        .map_rd_tip_selecao {
            width: 97px;
            font-weight: normal !important;
            font-size: 8px;
        }

        #txtLocalizar {
            z-index: 1;
            position: absolute;
            width: 320px;
            height: 38px;
            overflow: auto;
            top: 25px;
            left: 90px;
            border: 1px solid #F1F1F1;
            background-color: rgba(248,248,248,0.8);
            -webkit-box-shadow: 0 0 10px #505050;
            box-shadow: 0 0 10px #505050;
            padding: 0px 0px 0px 0px;
            border-radius: 3px;
        }

        .map_poly_ul {
            padding: 3px;
            margin: 0px;
            font-size: 14px;
            list-style: none;
        }

            .map_poly_ul li {
                height: 30px;
                padding-top: 7px;
                border-bottom: 1px solid #2A6BA6;
            }

                .map_poly_ul li:hover {
                    background-color: #FFFF00;
                }
    </style>
    <script src="../../Script/gm/gm.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script src="https://maps.googleapis.com/maps/api/js?v=3.exp&libraries=places&sensor=false"></script>
    <script src="../../Script/geoxml3/geoxml3.js"></script>
    <script src="../../Script/geoxml3/ProjectedOverlay.js"></script>

    <div id="divMenuConfiguracao" class="boxMenuFixo sb-toggle-right" title="Configurações e Filtros" onclick="window.scrollTo(0, 0);">
        <i id="iMenuRight" class="fa fa-gear fa-2x sb-toggle-right" style="color: gray" onclick="window.scrollTo(0, 0);"></i>
    </div>

    <div id="divParametrosGerais" style="display: none" runat="server"></div>

    <%--Body do Formulário --%>
    <input type="text" id="txtLocalizar" placeholder="Pesquisar localidade no mapa" />

    <div id="divCorpo" class="xpaginaBody" style="width: 100%; margin-top: -10px">
        <div id="map"></div>
    </div>

    <div id="divLstPoligonosContainer" class="ui-widget-content map_box_info" style="display: none">
        <span id="lstPoligonosTitulo" onclick="oGeo.zoomMapa()"></span>
        <div id="lstPoligonosItens"></div>
    </div>

    <script type="text/javascript">

        // inicializar variáveis globais
        var oParametros = JSON.parse($('#body_divParametrosGerais').html());
        oGeo = new GMGeo();

        $(document).ready(function () {

            $('#spanTitulo').html('Exploração Visual de Dados em Cartas Geográficas');

            oGeo.inicializar();

            $("#divOpcAnalise").buttonset();
            $("#divTipoMapSelecao").buttonset();

            $(function () {
                $("#divLstPoligonosContainer").draggable();
            });

        });

        // função de complementação (pode ou não ser implementada pelo usuário)
        // id = P (dentro de um polígono)
        //      M (em uma área do mapa)
        function mapaOnClick(id) {
            if (id == 'M') {
                oGeo.loadKML();
            }
        }

        /*
            Aplicação de Filtros (não necessariamente reload de página)
        */
        function paginaReload(filtroExcluir) {

            var filtro = getFiltros(filtroExcluir, true);

            if (filtro.hierarquia_incompleta != '') {
                myAlert(filtro.hierarquia_incompleta);
                return;
            }

            aguarde('true');

            var radio = $('input[name=rdOpcAnalise]:checked').val();

            var url = '/gm/ws/gmGeo.ashx'; //var url = getPrefixoURL() + 'ws/allupaGeo.ashx';
            var dados;
            var myFuncOK;

            switch (radio) {

                case 'varejo':

                    dados = {
                        method: 'ListaLojas',
                        data_movimento: filtro.data,
                        codigo_rede: filtro.rede,
                        codigo_loja: filtro.loja,
                        codigo_abastecedor: filtro.abastecedor,
                        codigo_hierarquia_nivel: filtro.hierarquia_nivel,
                        codigo_hierarquia_item: filtro.hierarquia_item,
                        codigo_produto: filtro.produto
                    };

                    myFuncOK = function (d) {
                        if (d.a_msg == 'OK') {
                            aguarde(false);
                            console.log('d.marcas', d.marcas);
                            oGeo.adicionarMarca(d.marcas, 'all');
                        }
                        else {
                            aguarde(false);
                            myAlert(d.a_msg);
                        }
                    }

                    execAjaxJsonP(url, dados, myFuncOK);

                    break;

                case 'ipc':
                    break;

                case 'tv':
                    break;
            }

            aguarde(false);
        }

        //
        // Habilitar / Desabilitar seleção de áreas no mapa
        //
        function auxHabilitarSelecao() {
            oGeoOpcoesAnalise.habilitarSelecao = document.getElementById('chkHabilitarSelecao').checked;
            $('#divTipoMapSelecao').css('display', (oGeoOpcoesAnalise.habilitarSelecao ? 'block' : 'none'));
            radioTipoSelecao();
        }

    </script>

    <div id="location_setter"></div>

</asp:Content>


