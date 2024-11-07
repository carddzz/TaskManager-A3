using System;

namespace TaskManager
{
    public class Tarefa
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public DateTime Horario { get; set; } 
        public bool Concluida { get; set; }
    }
}
