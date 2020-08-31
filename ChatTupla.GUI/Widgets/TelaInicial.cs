using System.Reflection.Metadata.Ecma335;
using System.Collections.Generic;
using Myra.Graphics2D.UI;
using System;

namespace ChatTupla.GUI
{
    public class TelaInicial
    {
        public string Nome { get; set; }
        public string IP { get; set; }
        public string Porta { get; set; }
        public TelaPrincipal Tela { get; set; }
        public Label labelErro { get; set; }
        public Action<string> SetWindowTitlte { get; }
        public TelaInicial(Action<string> setWindowTitlte)
        {
            this.SetWindowTitlte = setWindowTitlte;
        }

        public Grid returnTelaPrincipal()
        {
            var grid = new Grid
            {
                RowSpacing = 8,
                ColumnSpacing = 8
            };
            grid.ColumnsProportions.Add(new Proportion());
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
            grid.ColumnsProportions.Add(new Proportion());
            // espaço
            grid.RowsProportions.Add(new Proportion
            {
                Type = ProportionType.Pixels,
                Value = 120
            });
            //Nome
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // ip
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // porta
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // erros
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // Botao OK
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            var label = new Label
            {
                Text = "Digite o seu nome",
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
                GridColumn = 1,
            };

            textBox.TextChanged += (b, ea) =>
            {
                if (textBox.Text.Length > 33)
                {
                    textBox.Text = textBox.Text.Substring(0, 33);
                    textBox.CursorPosition = 33;
                }
            };

            ///// IP
            var labelIP = new Label
            {
                Text = "Digite o IP do servidor",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 3,
                GridColumn = 1,
            };


            var textBoxIP = new TextBox
            {
                Width = 400,
                Height = 24,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 4,
                GridColumn = 1
            };
            textBoxIP.TextChanged += (b, ea) =>
            {
                if (textBoxIP.Text.Length > 33)
                {
                    textBoxIP.Text = textBox.Text.Substring(0, 33);
                    textBoxIP.CursorPosition = 33;
                }
            };
            ////////////// Porta
            var labelPorta = new Label
            {
                Text = "Digite a porta do servidor",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 5,
                GridColumn = 1,
            };


            var textBoxPorta = new TextBox
            {
                Width = 400,
                Height = 24,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 6,
                GridColumn = 1
            };
            textBoxPorta.TextChanged += (b, ea) =>
            {
                if (textBoxPorta.Text.Length > 33)
                {
                    textBoxPorta.Text = textBox.Text.Substring(0, 33);
                    textBoxPorta.CursorPosition = 33;
                }
            };
            ////////// erros
            labelErro = new Label
            {
                Text = "",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 7,
                GridColumn = 1,
                TextColor = Microsoft.Xna.Framework.Color.Crimson
            };
            ////////// Botao ok
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
                cleanErrors();

                if (string.IsNullOrEmpty(textBox.Text))
                    showErrors("Escreva um nome!");
                else
                    this.Nome = textBox.Text;

                if (string.IsNullOrEmpty(textBoxIP.Text))
                    showErrors("Escreva um IP!");
                else
                    this.IP = textBoxIP.Text;

                if (string.IsNullOrEmpty(textBoxPorta.Text))
                    showErrors("Escreva uma porta!");
                else
                    this.Porta = textBoxPorta.Text;

                if (string.IsNullOrEmpty(labelErro.Text))
                {
                    // deu certo, tentar conectar
                    try
                    {
                        this.Tela = new TelaPrincipal(this.Nome, this.IP, this.Porta, showErrors);
                    }
                    catch (System.Exception)
                    {
                        this.Tela = null;
                    }
                    if (this.Tela != null)
                    {
                        // var amigos = new List<Contato>{
                        //     new Contato("TURURU", "1", "1"),
                        // };
                        // trocar de tela
                        this.SetWindowTitlte("Chat Tupla - "+this.Nome);
                        grid.Widgets.Clear();
                        grid.RowsProportions.Clear();
                        grid.ColumnsProportions.Clear();
                        grid.RowsProportions.Add(new Proportion(ProportionType.Fill));
                        grid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

                        grid.Widgets.Add(this.Tela.ReturnTelaPrincipal());
                    }

                }

            };

            grid.Widgets.Add(label);
            grid.Widgets.Add(textBox);
            grid.Widgets.Add(labelIP);
            grid.Widgets.Add(textBoxIP);
            grid.Widgets.Add(labelPorta);
            grid.Widgets.Add(textBoxPorta);
            grid.Widgets.Add(labelErro);
            grid.Widgets.Add(button);

            return grid;
        }

        public void showErrors(string erros)
        {
            labelErro.Text += erros + "\n";
        }
        public void cleanErrors()
        {
            labelErro.Text = "";
        }
    }
}