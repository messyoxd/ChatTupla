using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using System.Threading;
using ChatTupla.TupleSpaceClient;
using ChatTupla.Classes;
using dotSpace.Interfaces.Space;


namespace ChatTupla.GUI
{
    public class TelaPrincipal
    {
        public string MyName { get; set; }
        public string ServerPort { get; set; }
        public string ServerIP { get; set; }
        public TupleConnection _tupleConnection { get; set; }
        public HorizontalSplitPane horizontalSplitPane { get; set; }
        public VerticalStackPanel verticalStackPanel { get; set; }
        public Dictionary<string, List<Mensagem>> batePapo { get; set; }
        public ScrollViewer CurrentScrollViewer { get; set; }
        public Grid GridListarSalas { get; set; }
        public TextBox CurrentTextBox { get; set; }
        public Action<string> ShowErrors { get; set; }
        public bool ChatEnabled { get; set; }
        // public bool Estado = true;
        public string salaAtual = "";
        // public GrpcMensageiroCom _com { get; set; }
        // public Thread comThread { get; set; }
        public static Semaphore _pool;
        public TelaPrincipal(string myName, string myIP, string myPort, Action<string> showErrors)
        {
            ServerIP = myIP;
            ServerPort = myPort;
            MyName = myName;
            ShowErrors = showErrors;
            batePapo = new Dictionary<string, List<Mensagem>>();
            _pool = new Semaphore(1, 1);
            try
            {
                _tupleConnection = new TupleConnection(ServerIP, ServerPort, "chat", InsertMessage);
            }
            catch (System.Exception)
            {
                ShowErrors("Erro com o servidor! Nao foi possivel se conectar!");
                throw;
            }
            try
            {
                _tupleConnection.AddUser(MyName);
            }
            catch (System.Exception e)
            {
                ShowErrors(e.Message.ToString());
                throw;
            }

        }
        public VerticalStackPanel ReturnTelaPrincipal()
        {
            verticalStackPanel = new VerticalStackPanel
            {
                Spacing = 1
            };

            verticalStackPanel.Proportions.Add(new Proportion { Type = ProportionType.Auto });
            verticalStackPanel.Proportions.Add(new Proportion { Type = ProportionType.Fill });

            var horizontalMenu = new HorizontalMenu
            {
                VerticalAlignment = VerticalAlignment.Stretch
            };
            var menuItem = new MenuItem
            {
                Text = "Criar Sala"
            };
            menuItem.Selected += (s, ea) =>
            {
                // chamar a tela de criar sala
                verticalStackPanel.Widgets.RemoveAt(1);
                verticalStackPanel.Widgets.Add(carregarCriarSalaWidget());
            };
            var menuItem2 = new MenuItem
            {
                Text = "Mostrar lista de salas"
            };
            menuItem2.Selected += (s, ea) =>
            {
                // chamar a tela de listar salas
                verticalStackPanel.Widgets.RemoveAt(1);
                verticalStackPanel.Widgets.Add(carregarListarSalasWidget());
            };
            horizontalMenu.Items.Add(menuItem);
            horizontalMenu.Items.Add(menuItem2);

            var label = new Label
            {
                Text = "Bem vindo",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Id = "labelBemVindo"
            };

            verticalStackPanel.Widgets.Add(horizontalMenu);
            verticalStackPanel.Widgets.Add(label);
            return verticalStackPanel;
        }
        public Grid carregarChatWidget()
        {

            var gridChat = new Grid
            {
                RowSpacing = 8,
                ColumnSpacing = 8,
                Id = "chatBox"
            };
            gridChat.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            gridChat.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            gridChat.RowsProportions.Add(new Proportion(ProportionType.Auto));
            gridChat.RowsProportions.Add(new Proportion(ProportionType.Auto));

            var gridmensagens = new Grid
            {
                RowSpacing = 10,
                ColumnSpacing = 0,
                GridRowSpan = 10,
                Id = "chatMensagens"
            };
            gridChat.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
            gridChat.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
            gridChat.RowsProportions.Add(new Proportion(ProportionType.Fill));
            gridChat.RowsProportions.Add(new Proportion(ProportionType.Fill));

            ///////////// carregar mensagens
            var mensagens = _tupleConnection.GetAllGroupMessages(salaAtual);
            foreach (var item in mensagens)
            {
                var messageBox = createMessage(
                        (string)item[8],
                        (string)item[7],
                        (bool)item[10],
                        (string)item[11],
                        gridmensagens.Widgets.Count,
                        (string)item[7] == MyName
                    );
                if (messageBox != null)
                {
                    gridmensagens.RowsProportions.Add(new Proportion(ProportionType.Auto));
                    gridmensagens.Widgets.Add(messageBox);
                }
            }
            // comecar a receber mensagens
            _tupleConnection.ReceiveMessages(salaAtual);

            CurrentScrollViewer = new ScrollViewer
            {
                Left = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 350,
                Height = 400,

            };
            CurrentScrollViewer.Content = gridmensagens;

            CurrentTextBox = new TextBox
            {
                Left = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                GridRow = 1,
                Width = 400,
                Height = 24,
            };
            CurrentTextBox.TextChanged += (a, s) =>
            {
                if (CurrentTextBox.Text.Length + (MyName.Length + 2) > 33)
                {
                    CurrentTextBox.Text = CurrentTextBox.Text.Substring(0, 33 - (MyName.Length + 2));
                    CurrentTextBox.CursorPosition = 33;
                }
            };
            CurrentTextBox.KeyDown += (a, s) =>
            {
                if (s.Data.ToString() == "Enter" && CurrentTextBox.Text != "")
                {
                    // enviar mensagem
                    _tupleConnection.SendMessage(salaAtual, MyName, CurrentTextBox.Text);
                }
            };
            gridChat.Widgets.Add(CurrentTextBox);
            gridChat.Widgets.Add(CurrentScrollViewer);
            return gridChat;
        }
        // public MenuItem mudarDeEstado()
        // {
        //     var menuItemEstado = new MenuItem { };
        //     if (Estado)
        //     {
        //         menuItemEstado.Text = "Ficar Online";
        //     }
        //     else
        //     {
        //         comThread = new Thread(() => _com.IniciarServidor());
        //         comThread.Start();
        //         menuItemEstado.Text = "Ficar Offline";
        //     }
        //     menuItemEstado.Selected += (s, ea) =>
        //     {
        //         var menu = (HorizontalMenu)verticalStackPanel.Widgets.ElementAt(0);
        //         menu.Items.RemoveAt(1);
        //         menu.Items.Add(mudarDeEstado());
        //     };
        //     Estado = !Estado;
        //     return menuItemEstado;
        // }
        // public string getContatoAtual(){
        //     return contatoAtual;
        // }
        // public bool getEstado()
        // {
        //     return Estado;
        // }
        public Grid carregarCriarSalaWidget()
        {
            var gridCriarSala = new Grid
            {
                RowSpacing = 8,
                ColumnSpacing = 8
            };
            gridCriarSala.ColumnsProportions.Add(new Proportion());
            gridCriarSala.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
            gridCriarSala.ColumnsProportions.Add(new Proportion());
            // espaço
            gridCriarSala.RowsProportions.Add(new Proportion
            {
                Type = ProportionType.Pixels,
                Value = 120
            });
            //Nome da sala
            gridCriarSala.RowsProportions.Add(new Proportion(ProportionType.Auto));
            gridCriarSala.RowsProportions.Add(new Proportion(ProportionType.Auto));
            // erros
            gridCriarSala.RowsProportions.Add(new Proportion(ProportionType.Auto));
            // Botao OK
            gridCriarSala.RowsProportions.Add(new Proportion(ProportionType.Auto));

            var label = new Label
            {
                Text = "Digite o nome da sala",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 1,
                GridColumn = 1,
            };

            var textBox = new TextBox
            {
                Width = 400,
                Height = 24,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 2,
                GridColumn = 1
            };
            textBox.TextChanged += (b, ea) =>
            {
                if (textBox.Text.Length > 33)
                {
                    textBox.Text = textBox.Text.Substring(0, 33);
                    textBox.CursorPosition = 33;
                }
            };

            var labelErro = new Label
            {
                Text = "",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 7,
                GridColumn = 1,
                TextColor = Microsoft.Xna.Framework.Color.Crimson
            };

            var button = new TextButton
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 100,
                Text = "Ok",
                GridRow = 8,
                GridColumn = 1,
            };

            button.Click += (b, ea) =>
            {
                labelErro.Text = "";
                var NomeSala = "";
                if (string.IsNullOrEmpty(textBox.Text))
                    labelErro.Text += "Escreva um nome!\n";
                else
                    NomeSala = textBox.Text;

                if (string.IsNullOrEmpty(labelErro.Text))
                {
                    // adicionar tupla com o nome da sala
                    try
                    {
                        _tupleConnection.AddGroup(NomeSala);
                    }
                    catch (System.Exception e)
                    {
                        labelErro.Text += e.Message.ToString();
                    }
                    if (labelErro.Text == "")
                    {
                        // se não houve erros então entrar na sala
                        var error = false;
                        try
                        {

                            _tupleConnection.RemoveUserFromGroup(salaAtual, MyName);
                        }
                        catch (System.Exception)
                        {
                            // 
                        }
                        try
                        {
                            _tupleConnection.InsertUserIntoGroup(NomeSala, MyName);
                        }
                        catch (System.Exception e)
                        {
                            labelErro.Text += e.Message.ToString();
                            error = true;
                            // throw;
                        }
                        if (!error)
                        {
                            // mudar sala atual
                            salaAtual = NomeSala;
                            // mudar a tela pro chat da sala
                            verticalStackPanel.Widgets.RemoveAt(1);
                            verticalStackPanel.Widgets.Add(carregarChatWidget());
                        }
                    }
                }

            };

            gridCriarSala.Widgets.Add(label);
            gridCriarSala.Widgets.Add(textBox);
            gridCriarSala.Widgets.Add(labelErro);
            gridCriarSala.Widgets.Add(button);

            return gridCriarSala;

        }
        public Grid carregarListarSalasWidget()
        {
            GridListarSalas = new Grid
            {
                RowSpacing = 8,
                ColumnSpacing = 8,
                Id = "gridListarSalas",
                // ShowGridLines = true
            };

            var gridSalas = new Grid
            {
                RowSpacing = 55,
                ColumnSpacing = 0,
                GridRowSpan = 55,
                Id = "gridSalas"
            };

            ///////////// carregar mensagens
            var grupos = _tupleConnection.GetAllGroups();
            if (grupos.Count() > 0)
            {
                int counter = 0;
                foreach (var item in grupos)
                {
                    var gridAux = new Grid
                    {
                        RowSpacing = 55,
                        ColumnSpacing = 0,
                        GridRowSpan = 55,
                        GridRow = counter
                    };

                    gridAux.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
                    gridAux.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
                    gridAux.RowsProportions.Add(new Proportion(ProportionType.Auto));
                    gridAux.RowsProportions.Add(new Proportion(ProportionType.Auto));

                    gridAux.Widgets.Add(criarBotaoListarSala((string)item[1]));
                    gridAux.Widgets.Add(criarBotaoListarUsuariosSala((string)item[1]));

                    gridSalas.RowsProportions.Add(new Proportion(ProportionType.Auto));
                    gridSalas.Widgets.Add(gridAux);
                    counter+=1;
                }
                var scroll = new ScrollViewer
                {
                    Left = 30,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 350,
                    Height = 400,

                };
                scroll.Content = gridSalas;

                GridListarSalas.Widgets.Add(scroll);
            }
            else
            {
                GridListarSalas.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
                GridListarSalas.RowsProportions.Add(new Proportion
                {
                    Type = ProportionType.Pixels,
                    Value = 200
                });
                GridListarSalas.RowsProportions.Add(new Proportion(ProportionType.Auto));
                GridListarSalas.Widgets.Add(new Label
                {
                    Text = "Nenhuma sala!",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    GridColumn = 1,
                    GridRow = 1
                });
            }

            return GridListarSalas;

        }

        private bool checkParecido(List<Mensagem> antigas, Mensagem nova)
        {
            // se a nova mensagem parece alguma antiga
            foreach (var item in antigas)
            {
                if (item._sender == nova._sender &&
                    item._receiver == nova._receiver &&
                    item._timestamp == nova._timestamp
                )
                    return true;
            }
            return false;
        }
        public bool AdicionaMensagem(string group, string sender, string message, string timestamp, bool isWhisper, string receiver)
        {
            // checar se o grupo tem mensagens
            // _pool.WaitOne();
            List<Mensagem> value = null;
            if (batePapo.TryGetValue(group, out value))
            {
                // se já tem, checar se a mensagem já foi entregue
                if (!this.checkParecido(value, new Mensagem(message, timestamp, sender, receiver, isWhisper)))
                {
                    value.Add(new Mensagem(message, timestamp, sender, receiver, isWhisper));
                    // _pool.Release();
                    return true;
                }
            }
            else
            {
                // se não, entao criar nova chave no dicionario
                var lista = new List<Mensagem>();
                lista.Add(new Mensagem(message, timestamp, sender, receiver, isWhisper));
                batePapo.Add(group, lista);
                // _pool.Release();
                return true;
            }
            // _pool.Release();
            return false;

        }
        public void InsertMessage(string group, string sender, string message, string timestamp, bool isWhisper, string receiver)
        {

            // checar se a mensagem ja nao foi adicionada
            if (this.AdicionaMensagem(group, sender, message, timestamp, isWhisper, receiver))
            {
                // se conseguiu adicionar
                var grid = (Grid)CurrentScrollViewer.GetChild(0);
                var messageBox = createMessage(
                            message,
                            sender,
                            isWhisper,
                            receiver,
                            grid.Widgets.Count,
                            sender == MyName
                        );
                if (messageBox != null)
                {
                    grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
                    grid.Widgets.Add(messageBox);
                    CurrentTextBox.Text = "";
                }
            }
        }

        // public Grid findContatoTextButton(Contato contato)
        // {
        //     var scroll = (ScrollViewer)horizontalSplitPane.Widgets.ElementAt(0);
        //     var grid = (Grid)scroll.GetChild(0);
        //     Grid aux = null;
        //     TextButton aux2 = null;
        //     bool contador = false;
        //     foreach (var item in grid.Widgets)
        //     {
        //         aux = (Grid)item;
        //         aux2 = (TextButton)aux.Widgets.ElementAt(0);
        //         if (aux2.Text == contato.Name)
        //         {
        //             contador = true;
        //             break;
        //         }
        //     }
        //     if (contador)
        //         return aux;
        //     else
        //         return null;
        // }
        // public void setLabelPendentesText(Contato contato, string text)
        // {
        //     Grid contatoGrid = findContatoTextButton(contato);
        //     if (contatoGrid != null)
        //     {
        //         var label = (Label)contatoGrid.Widgets.ElementAt(1);
        //         label.Text = text;
        //     }
        // }

        public TextButton criarBotaoListarUsuariosSala(string grupo)
        {
            var usersListButton = new TextButton
            {
                Margin = new Thickness { Left = 50, Bottom = -15 },
                Text = "Listar Usuarios",
                Height = 50,
                Width = 70,
                GridColumn = 1
            };

            usersListButton.Click += (s, a) =>
            {
                // pegar usuarios do grupo
                var users = _tupleConnection.GetAllUsersFromGroup(grupo);
                // carregar o widget da sala
                if (verticalStackPanel.Widgets.Count > 1)
                    verticalStackPanel.Widgets.RemoveAt(1);
                verticalStackPanel.Widgets.Add(carregarListaUsuariosWidget(users));
            };
            return usersListButton;
        }
        public TextButton criarBotaoListarSala(string grupo)
        {
            var salaButton = new TextButton
            {
                Margin = new Thickness { Left = 50, Bottom = -15 },
                Text = grupo,
                Height = 50,
                Width = 300,
                GridColumn = 0
            };

            salaButton.Click += (s, a) =>
            {
                // mudar de sala
                _tupleConnection.ChangeGroup(salaAtual, grupo, MyName);
                salaAtual = grupo;
                // carregar o widget da sala
                verticalStackPanel.Widgets.RemoveAt(1);
                verticalStackPanel.Widgets.Add(carregarChatWidget());
                var grid = (Grid)verticalStackPanel.Widgets.ElementAt(1);
                var textbox = grid.Widgets.ElementAt(0);
                textbox.SetKeyboardFocus();
            };
            return salaButton;
        }
        public ScrollViewer carregarListaUsuariosWidget(IEnumerable<dotSpace.Interfaces.Space.ITuple> users)
        {
            var gridUsuarios = new Grid
            {
                RowSpacing = 20,
                ColumnSpacing = 0,
                GridRowSpan = 20,
                Id = "gridUsuarios"
            };
            var a = users.Count();
            if(users.Count() == 0){
                gridUsuarios.RowsProportions.Add(new Proportion(ProportionType.Auto));
                gridUsuarios.Widgets.Add(new TextButton
                {
                    Padding = new Thickness { Right = 25 },
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "Nenhum Usuario!",
                    Height = 20,
                    Width = 150,
                    GridRow = gridUsuarios.Widgets.Count,
                    Enabled = false,
                    ContentHorizontalAlignment = HorizontalAlignment.Center
                });
            }
            else{
                foreach (var item in users)
                {
                    gridUsuarios.RowsProportions.Add(new Proportion(ProportionType.Auto));
                    gridUsuarios.Widgets.Add(new TextButton
                    {
                        Padding = new Thickness { Right = 25 },
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = (string)item[3],
                        Height = 20,
                        Width = 150,
                        GridRow = gridUsuarios.Widgets.Count,
                        Enabled = false,
                        ContentHorizontalAlignment = HorizontalAlignment.Center
                    });
                }
            }
            var scroll = new ScrollViewer
            {
                Left = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 350,
                Height = 400,

            };
            scroll.Content = gridUsuarios;
            return scroll;
        }
        public TextButton createMessage(string message, string sender, bool IsWhisper, string receiver, int row, bool IsMyMessage)
        {
            var width = (message.Length + MyName.Length + 2) * 10 < 300 ? (message.Length + MyName.Length + 2) * 10 : 300;
            var height = 20;
            // TextColor = Microsoft.Xna.Framework.Color.Crimson
            if (message.Length + (sender.Length) > 33)
            {
                message = message.Substring(0, 33 - sender.Length);
            }
            if (IsWhisper)
            {
                if (IsMyMessage)
                {
                    return new TextButton
                    {
                        Padding = new Thickness { Right = 25 },
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Text = MyName + ": " + message,
                        TextColor = Microsoft.Xna.Framework.Color.DeepPink,
                        Height = height,
                        Width = width,
                        GridRow = row,
                        Enabled = false,
                        ContentHorizontalAlignment = HorizontalAlignment.Left
                    };
                }
                else if (receiver == MyName)
                {
                    return new TextButton
                    {
                        Padding = new Thickness { Right = 25 },
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Text = sender + ": " + message,
                        TextColor = Microsoft.Xna.Framework.Color.DeepPink,
                        Height = height,
                        Width = width,
                        GridRow = row,
                        Enabled = false,
                        ContentHorizontalAlignment = HorizontalAlignment.Left
                    };
                }
                else
                    return null;
            }
            else
            {
                if (IsMyMessage)
                {
                    return new TextButton
                    {
                        Padding = new Thickness { Right = 25 },
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Text = MyName + ": " + message,
                        Height = height,
                        Width = width,
                        GridRow = row,
                        Enabled = false,
                        ContentHorizontalAlignment = HorizontalAlignment.Left
                    };
                }
                else
                {
                    return new TextButton
                    {
                        Padding = new Thickness { Right = 25 },
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Text = sender + ": " + message,
                        Height = height,
                        Width = width,
                        GridRow = row,
                        Enabled = false,
                        ContentHorizontalAlignment = HorizontalAlignment.Left
                    };
                }
            }
        }
    }
}