<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="gm.a001.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <link href="../Script/gm/gm.css" rel="stylesheet" />
    <link href="../Script/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
    <script src="../Script/jquery/jquery-1.11.3.min.js"></script>
    <style type="text/css">
        #divCabecalhoContainer {
            width: 100%;
            height: 60px;
            background-color: #2A6BA6;
        }

        #divLogoContainer {
            width: 100%;
            height: 60px;
        }

        #divLogoAllupa {
            width: 20%;
            height: inherit;
            float: left;
            margin-left: 5%;
        }

        #divBodyContainer {
            max-width: 1024px;
            margin: 0 auto;
        }

        #divLeft {
            position: relative;
            padding-top: 4%;
            padding-left: 8%;
            float: left;
            width: 50%;
            height: 100%;
            margin-bottom: -4%;
            margin-right: -8%;
        }

        #divRight {
            position: relative;
            width: 40%;
            left: 5%;
            padding-top: 9%;
            padding-left: 5%;
            float: left;
        }

        .ui-dialog, .ui-button {
            -moz-border-radius: 5px;
            -webkit-border-radius: 5px;
            -khtml-border-radius: 5px;
            border-radius: 5px;
            outline: none;
        }
        
        .mensagem {
            width: 209px;
            color: red;
            font-size: 12px;
            position: relative;
            left: 50%;
            margin-left: -150px;
            width: 300px;
            text-align:center;
        }

        /* iní­cio login */
        #divLogin{
            position: relative;
            left: 50%;
            margin-top: 120px;
            margin-left: -150px;
            width: 300px;
        }

            #divLogin h3 {
                margin: 0;
                background-color: #2A6BA6;
                border-radius: 5px 5px 0 0;
                color: #fff;
                font-size: 24px;
                padding: 20px;
                text-align: center;
                text-transform: uppercase;
            }

            #divLogin fieldset {
                border: none;
                margin: 0;
                background: #fff;
                border-radius: 0 0 5px 5px;
                padding: 19px;
                position: relative;
                border: 1px solid gray;
            }

                #divLogin fieldset:before {
                    background-color: #fff;
                    content: "";
                    height: 8px;
                    left: 50%;
                    margin: -4px 0 0 -4px;
                    position: absolute;
                    top: 0;
                    -webkit-transform: rotate(45deg);
                    -moz-transform: rotate(45deg);
                    -ms-transform: rotate(45deg);
                    -o-transform: rotate(45deg);
                    transform: rotate(45deg);
                    width: 8px;
                }

            #divLogin #txtUsuario,
            #divLogin #txtSenha {
                margin: 0;
                -webkit-appearance: none;
                border: 1px solid #dcdcdc;
                padding: 12px 10px;
                width: 238px;
                outline: none;
            }

            #divLogin #txtUsuario {
                border-radius: 5px 5px 0 0;
                outline: none;
            }

            #divLogin #txtSenha {
                border-top: none;
                border-radius: 0px 0px 5px 5px;
                outline: none;
            }

            #divLogin #btnLogin {
                border: none;
                margin: 0;
                -webkit-appearance: none;
                background: #D4D4D4;
                border-radius: 3px;
                /*color: #fff;*/
                float: right;
                font-weight: bold;
                margin-top: 20px;
                padding: 12px 20px;
                cursor: pointer;
            }

                #divLogin #btnLogin:hover {
                    background: #66B3FF;
                }

        /* fim login */

        /*início tooltip*/
        .ui-tooltip {
            padding: 8px;
            position: absolute;
            z-index: 9999;
            -o-box-shadow: 0 0 5px #aaa;
            -moz-box-shadow: 0 0 5px #aaa;
            -webkit-box-shadow: 0 0 5px #aaa;
            box-shadow: 0 0 5px #aaa;
        }

        * html .ui-tooltip {
            background-image: none;
        }

        body .ui-tooltip {
            border: 1px solid #cde1f4;
            border-radius: 5px;
            background-color: lightyellow;
        }
        /*fim tooltip*/

        /* chrome input password yellow */
        input:-webkit-autofill {
            -webkit-box-shadow: 0 0 0 1000px white inset;
        }
    </style>

    <script type="text/javascript">

        $('#divAguarde').css('display', 'none');

        $(function () {
            $(document).tooltip();
        });

        function validaDados() {
            var m = $('#paramModulo').html();
            var u = $('#txtUsuario').val(),
                s = $('#txtSenha').val();

            $.ajax({
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                url: 'login.aspx/ValidaDados',
                data: '{m: "' + m + '", u: "' + u + '", s: "' + s + '"}',
                success:
                    function (response) {
                        var msg = response.d.Message;
                        if (msg == "OK") {
                            $('#divAguarde').css('display', 'inline');
                            $('#divMensagem').html('');
                            switch (m.toLowerCase()) {
                                case 'gm': url = '/gm/a001/modGM/home.aspx'; break;
                            }
                            document.location.href = url;
                        }
                        else
                            $('#divMensagem').html(msg);
                    }
            });
        }

    </script>

</head>
<body>

    <div style="text-align: left; margin: 0 auto">

        <form id="frmAllupa" runat="server">

            <div id="divCabecalhoContainer">
                <div id="divLogoContainer">
                    <div id="divLogoAllupa">
                        <img src="../Imagens/allupa_logotipo.png" style="border: 0px; position: relative; top: 15px" />
                    </div>
                </div>
            </div>

            <div id="divBodyContainer" style="margin-top:60px">
                <div id="divLogin">
                    <h3>LOGIN</h3>
                    <fieldset>
                        <legend></legend>
                        <input type="text" id="txtUsuario" placeholder="Usuário" class="txtLogin" />
                        <input type="password" id="txtSenha" placeholder="Senha" class="txtLogin" />
                        <input type="button" id="btnLogin" value="Login" onclick="validaDados()" />
                    </fieldset>
                </div>
                <br />
                <div id="divMensagem" class="mensagem" runat="server"></div>
            </div>
        </form>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            var msg = $('#paramMensagem').html();
            if (msg != "") {
                $('#divMensagem').html(msg);
            }
        });
    </script>

    <div id="divParametros" style="display: none" runat="server"></div>
    
    <div id="divAguarde" style="position: absolute; width: 200px; height: 100px; top: 50%; left: 50%; margin-left: -100px; margin-top: -100px; z-index: 5000; display: none; vertical-align: middle; text-align: center; background-color: lightgray; border: 1px solid gray; border-radius: 5px">
        <i class="fa fa-refresh fa-5x fa-spin" style="color: #10253F"></i>
        <br />
        <label class="CabecalhoPagina">Aguarde...</label>
    </div>
</body>
</html>
