using System;
using System.Linq;
using System.Threading;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;

namespace ChatTupla.TupleSpaceServer
{
    public class Servidor
    {
        private readonly string port;
        private readonly string ip;
        private SpaceRepository repository;
        public bool pingCompleted = false;
        public RemoteSpace remotespace;
        public Servidor(string ip, string port, SpaceRepository space)
        {
            this.ip = ip;
            this.port = port;
            this.repository = space;
            // instanciar space
            // var threaSpace = new Thread(() => this.StartSpace());
            // threaSpace.Start();
            // Thread.Sleep(100);
            this.remotespace = new RemoteSpace($"tcp://{this.ip}:{this.port}/chat?KEEP");
            // apagador de mensagens
            var threaMensagens = new Thread(() => this.apagaMensagens());
            threaMensagens.Start();
            // apagador de salas
            var threaSalas = new Thread(() => this.apagaSalas());
            threaSalas.Start();

        }

        public void StartSpace()
        {
            this.repository = new SpaceRepository();
            this.repository.AddGate($"tcp://{this.ip}:{this.port}?KEEP");
            this.repository.AddSpace("chat", new SequentialSpace());
            Console.ReadKey();
            // while(true){Thread.Sleep(100);}
        }

        public void apagaMensagens()
        {
            while(true){
                // percorrer todos os grupos
                var grupos = this.remotespace.QueryAll("Group", typeof(string));
                if (grupos.Count() > 0)
                {
                    foreach (var grupo in grupos)
                    {
                        if (grupo != null)
                        {
                            // pegar todas as mensagens daquele grupo
                            var mensagens = this.remotespace.QueryAll("Group",      "Sender",      "Message",     "Timestamp",   "IsWhisper",   "Receiver",
                                        (string)grupo[1], typeof(string), typeof(string), typeof(string), typeof(bool), typeof(string));
                            if (mensagens.Count() > 0)
                            {
                                foreach (var mensagem in mensagens)
                                {
                                    if (mensagem != null)
                                    {
                                        // converter de string para DateTime
                                        var date = DateTime.Parse((string)mensagem[9]);
                                        // se a mensagem for mais antiga que 5 min
                                        var aux = (DateTime.UtcNow-date).TotalMinutes;
                                        if ((DateTime.UtcNow - date).TotalMinutes >= 5)
                                        {
                                            // remover aquela mensagem
                                            this.remotespace.GetP("Group", "Sender", "Message", "Timestamp", "IsWhisper", "Receiver",
                                            (string)grupo[1], (string)mensagem[7], (string)mensagem[8], (string)mensagem[9], (bool)mensagem[10], (string)mensagem[11]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }
        private void checkOnline(string name)
        {
            // a requisicao para a tupla aguardará até que haja alguma tupla que se encaixe 
            var tupla = this.remotespace.Get("User", name, "ping", typeof(bool));
            pingCompleted = true;
        }
        private static bool WaitUntil(int numberOfMiliSeconds, Func<bool> condition)
        {
            int waited = 0;
            while (!condition() && waited < numberOfMiliSeconds)
            {
                Thread.Sleep(100);
                waited += 100;
            }

            return condition();
        }
        public void apagaSalas()
        {
            while(true){
                // percorrer todos os grupos
                var grupos = this.remotespace.QueryAll("Group", typeof(string));
                if (grupos.Count() > 0)
                {
                    foreach (var grupo in grupos)
                    {
                        if (grupo != null)
                        {
                            // pegar todas os usuarios daquele grupo
                            var usuarios = this.remotespace.QueryAll("Group", "User", (string)grupo[1], typeof(string));
                            int counter = usuarios.Count(); // "Enumeration yielded no results"
                            if(counter == 0){
                                // checar tupla de usuarios retirados para ver se 10 min se passaram
                                var log = this.remotespace.QueryP("Group", "User", "LastRemoved", (string)grupo[1], typeof(string));
                                if(log != null){
                                    // converter de string para DateTime
                                    var date = DateTime.Parse((string)log[4]);
                                    // se for mais antigo que 10 min
                                    if ((DateTime.UtcNow - date).TotalMinutes >= 10)
                                    {
                                        // excluir grupo e suas mensagens
                                        this.remotespace.GetP("Group", (string)grupo[1]);
                                        this.remotespace.GetAll("Group", "Sender", "Message", "Timestamp", "IsWhisper", "Receiver",
                                                        (string)grupo[1], typeof(string), typeof(string), typeof(string), typeof(bool), typeof(string));
                                        this.remotespace.GetP("Group", "User", "LastRemoved", (string)grupo[1], typeof(string));
                                    }
                                }
                            }
                            else{

                                foreach (var usuario in usuarios)
                                {
                                    if((string)usuario[0] != "Enumeration yielded no results"){
                                        // checar se o o usuario está online
                                        this.remotespace.Put("User", (string)usuario[3], "ping");
                                        // caso a resposta demore mais que 2s, assumir que esta offline e pegar a tupla 
                                        pingCompleted = false;
                                        var threadCheck = new Thread(() => this.checkOnline((string)usuario[3]));
                                        threadCheck.Start();
                                        if (!WaitUntil(2000, () => pingCompleted))
                                        {
                                            // offline
                                            // Remover velha tupla
                                            this.remotespace.GetP("User", (string)usuario[3]);
                                            // atualizar tupla de usuarios retirados
                                            this.remotespace.GetP("Group", "User", "LastRemoved", (string)grupo[1], typeof(string));
                                            this.remotespace.Put("Group", "User", "LastRemoved", (string)grupo[1], DateTime.UtcNow.ToString());
                                            counter -= 1;
                                        }
                                        else
                                        {
                                            // online
                                            // não fazer nada
                                        }
                                    }

                                    // checar tupla de usuarios retirados para ver se 10 min se passaram
                                    var log = this.remotespace.QueryP("Group", "User", "LastRemoved", (string)grupo[1], typeof(string));
                                    if(log != null){
                                        // converter de string para DateTime
                                        var date = DateTime.Parse((string)log[4]);
                                        // se for mais antigo que 10 min
                                        if ((DateTime.UtcNow - date).TotalMinutes >= 10)
                                        {
                                            // excluir grupo e suas mensagens
                                            this.remotespace.GetP("Group", (string)grupo[1]);
                                            this.remotespace.GetAll("Group", "Sender", "Message", "Timestamp", "IsWhisper", "Receiver",
                                                            (string)grupo[1], typeof(string), typeof(string), typeof(string), typeof(bool), typeof(string));
                                            this.remotespace.GetP("Group", "User", "LastRemoved", (string)grupo[1], typeof(string));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}