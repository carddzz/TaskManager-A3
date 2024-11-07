using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TaskManager
{
    public partial class Form1 : Form
    {
        private TextBox textBoxDescricao;
        private Button buttonAdicionar;
        private ListBox listBoxTarefas;
        private Button buttonExcluir;
        private Button buttonMarcarConcluida;
        private DateTimePicker dateTimePickerHorario;
        private Label labelTitulo;

        private List<Tarefa> tarefas;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenu;

        public Form1()
        {
            this.Text = "Task Manager";
            this.Size = new Size(600, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Tenta carregar o ícone com tratamento de exceção
            try
            {
                this.Icon = new Icon("icones/notificacao.ico"); // Caminho do ícone
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar o ícone: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Icon = SystemIcons.Information; // Usa um ícone padrão, se falhar
            }

            // Configura o NotifyIcon
            notifyIcon = new NotifyIcon
            {
                Icon = this.Icon,
                Visible = true,
                Text = "Task Manager"
            };

            // Menu de contexto para o ícone na bandeja
            contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Restaurar", null, Restaurar_Click);
            contextMenu.Items.Add("Sair", null, Sair_Click);

            // Associando o menu de contexto ao NotifyIcon
            notifyIcon.ContextMenuStrip = contextMenu;

            // Bordas arredondadas
            int bordaArredondada = 30;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, bordaArredondada, bordaArredondada, 180, 90);
            path.AddArc(this.Width - bordaArredondada, 0, bordaArredondada, bordaArredondada, 270, 90);
            path.AddArc(this.Width - bordaArredondada, this.Height - bordaArredondada, bordaArredondada, bordaArredondada, 0, 90);
            path.AddArc(0, this.Height - bordaArredondada, bordaArredondada, bordaArredondada, 90, 90);
            path.CloseAllFigures();
            this.Region = new Region(path);

            tarefas = new List<Tarefa>();

            ConfigurarControles();
        }

        private void ConfigurarControles()
        {
            // Configurações de fonte
            Font fontePadrao = new Font("Segoe UI", 10, FontStyle.Regular);
            Font fonteTitulo = new Font("Segoe UI", 16, FontStyle.Bold);

            // Título
            labelTitulo = new Label
            {
                Text = "TaskManager",
                ForeColor = Color.White,
                Font = fonteTitulo,
                AutoSize = true,
                Location = new Point((this.Width - 150) / 2, 20)
            };
            this.Controls.Add(labelTitulo);

            int espacoSuperior = 60;

            textBoxDescricao = new TextBox
            {
                Location = new Point(20, espacoSuperior),
                Size = new Size(350, 30),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = fontePadrao
            };
            this.Controls.Add(textBoxDescricao);

            dateTimePickerHorario = new DateTimePicker
            {
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Location = new Point(20, espacoSuperior + 40),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                Font = fontePadrao
            };
            this.Controls.Add(dateTimePickerHorario);

            buttonAdicionar = new Button
            {
                Text = "Adicionar",
                Location = new Point(400, espacoSuperior),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(34, 177, 76),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = fontePadrao
            };
            buttonAdicionar.FlatAppearance.BorderSize = 0;
            buttonAdicionar.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 205, 50);
            this.Controls.Add(buttonAdicionar);
            buttonAdicionar.Click += new EventHandler(ButtonAdicionar_Click);

            listBoxTarefas = new ListBox
            {
                Location = new Point(20, espacoSuperior + 80),
                Size = new Size(530, 400),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = fontePadrao
            };
            this.Controls.Add(listBoxTarefas);

            buttonExcluir = new Button
            {
                Text = "Excluir",
                Location = new Point(20, espacoSuperior + 520),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(181, 11, 17),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = fontePadrao
            };
            buttonExcluir.FlatAppearance.BorderSize = 0;
            buttonExcluir.FlatAppearance.MouseOverBackColor = Color.FromArgb(212, 15, 21);
            this.Controls.Add(buttonExcluir);
            buttonExcluir.Click += new EventHandler(ButtonExcluir_Click);

            buttonMarcarConcluida = new Button
            {
                Text = "Marcar Concluída",
                Location = new Point(200, espacoSuperior + 520),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = fontePadrao
            };
            buttonMarcarConcluida.FlatAppearance.BorderSize = 0;
            buttonMarcarConcluida.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 160, 255);
            this.Controls.Add(buttonMarcarConcluida);
            buttonMarcarConcluida.Click += new EventHandler(ButtonMarcarConcluida_Click);
        }

        private void ButtonAdicionar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBoxDescricao.Text))
            {
                var tarefa = new Tarefa
                {
                    Id = tarefas.Count + 1,
                    Descricao = textBoxDescricao.Text,
                    Horario = dateTimePickerHorario.Value,
                    Concluida = false
                };
                tarefas.Add(tarefa);
                AtualizarListaTarefas();
                textBoxDescricao.Clear();
                dateTimePickerHorario.Value = DateTime.Now;
            }
        }

        private void ButtonExcluir_Click(object sender, EventArgs e)
        {
            if (listBoxTarefas.SelectedItem != null)
            {
                int indice = listBoxTarefas.SelectedIndex;
                tarefas.RemoveAt(indice);
                AtualizarListaTarefas();
            }
        }

        private void ButtonMarcarConcluida_Click(object sender, EventArgs e)
        {
            if (listBoxTarefas.SelectedItem != null)
            {
                int indice = listBoxTarefas.SelectedIndex;
                tarefas[indice].Concluida = true;
                AtualizarListaTarefas();
            }
        }

        private void AtualizarListaTarefas()
        {
            listBoxTarefas.Items.Clear();
            foreach (var tarefa in tarefas)
            {
                string status = tarefa.Concluida ? "✔️ Concluída" : "❗ Pendente";
                string horario = tarefa.Horario.ToString("HH:mm");
                listBoxTarefas.Items.Add($"{tarefa.Id}: {tarefa.Descricao} - {status} às {horario}");
            }
        }

        private void Restaurar_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Sair_Click(object sender, EventArgs e)
        {
            notifyIcon.Dispose(); // Destrói o NotifyIcon ao sair
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                e.Cancel = true;
            }
            else
            {
                notifyIcon.Dispose(); // Destrói o NotifyIcon
                base.OnFormClosing(e);
            }
        }
    }
}
